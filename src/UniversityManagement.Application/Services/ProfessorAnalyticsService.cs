using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Analytics;
using Supabase;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Service implementation for professor analytics
/// </summary>
public class ProfessorAnalyticsService : IProfessorAnalyticsService
{
    private readonly Client _supabaseClient;

    public ProfessorAnalyticsService(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<ProfessorDashboardDto> GetProfessorDashboardAsync(Guid professorId, Guid? academicYearId = null)
    {
        // Get professor info from v_professor_performance view
        var performanceResult = await _supabaseClient
            .From<dynamic>("v_professor_performance")
            .Select("*")
            .Filter("professor_id", Supabase.Postgrest.Constants.Operator.Equals, professorId.ToString())
            .Single();

        if (performanceResult == null)
        {
            throw new Exception($"Professor {professorId} not found");
        }

        var performance = performanceResult;

        // Get course performance details
        var coursePerformances = await GetCoursePerformanceAsync(professorId, academicYearId);

        // Get pending tasks
        var pendingTasks = await GetPendingTasksAsync(professorId);

        // Build grade distribution across all courses
        var gradeDistribution = new Dictionary<int, int>();
        foreach (var course in coursePerformances)
        {
            var distribution = await GetCourseGradeDistributionAsync(course.CourseId);
            foreach (var grade in distribution)
            {
                if (gradeDistribution.ContainsKey(grade.GradeValue))
                    gradeDistribution[grade.GradeValue] += grade.GradeCount;
                else
                    gradeDistribution[grade.GradeValue] = grade.GradeCount;
            }
        }

        var dashboard = new ProfessorDashboardDto
        {
            ProfessorId = professorId,
            ProfessorName = performance.professor_name?.ToString() ?? "",
            Title = performance.title?.ToString() ?? "",
            
            TotalCoursesTeaching = Convert.ToInt32(performance.total_courses_teaching ?? 0),
            TotalStudents = Convert.ToInt32(performance.total_students ?? 0),
            PendingGrades = Convert.ToInt32(performance.pending_grades_count ?? 0),
            AverageClassPerformance = Convert.ToDecimal(performance.avg_grade_given ?? 0),
            
            CoursePerformances = coursePerformances,
            PendingTasks = pendingTasks,
            OverallGradeDistribution = gradeDistribution
        };

        return dashboard;
    }

    public async Task<List<CoursePerformanceDto>> GetCoursePerformanceAsync(Guid professorId, Guid? academicYearId = null)
    {
        // Call database function
        var parameters = new Dictionary<string, object>
        {
            { "p_professor_id", professorId }
        };
        
        if (academicYearId.HasValue)
        {
            parameters.Add("p_academic_year_id", academicYearId.Value);
        }
        
        var result = await _supabaseClient.Rpc("get_professor_course_performance", parameters);
        
        var performances = new List<CoursePerformanceDto>();
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                performances.Add(new CoursePerformanceDto
                {
                    CourseId = Guid.Parse(item.course_id?.ToString() ?? Guid.Empty.ToString()),
                    CourseName = item.course_name?.ToString() ?? "",
                    CourseCode = item.course_code?.ToString() ?? "",
                    SemesterNumber = Convert.ToInt32(item.semester_number ?? 0),
                    TotalStudents = Convert.ToInt32(item.total_students ?? 0),
                    AverageGrade = Convert.ToDecimal(item.avg_grade ?? 0),
                    PassRate = Convert.ToDecimal(item.pass_rate ?? 0),
                    AttendanceRate = Convert.ToDecimal(item.attendance_rate ?? 0),
                    PendingGrades = Convert.ToInt32(item.pending_grades ?? 0),
                    GradeContests = Convert.ToInt32(item.grade_contests ?? 0)
                });
            }
        }
        
        return performances;
    }

    public async Task<List<GradeDistributionDto>> GetCourseGradeDistributionAsync(Guid courseInstanceId)
    {
        // Call database function
        var result = await _supabaseClient.Rpc("get_course_grade_distribution", 
            new { p_course_instance_id = courseInstanceId });
        
        var distributions = new List<GradeDistributionDto>();
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                distributions.Add(new GradeDistributionDto
                {
                    GradeValue = Convert.ToInt32(item.grade_value ?? 0),
                    GradeCount = Convert.ToInt32(item.student_count ?? 0),
                    Percentage = Convert.ToDecimal(item.percentage ?? 0),
                    Courses = new List<string>() // Not needed for single course
                });
            }
        }
        
        return distributions;
    }

    public async Task<PendingTasksDto> GetPendingTasksAsync(Guid professorId)
    {
        // Get pending grades
        var gradesResult = await _supabaseClient
            .From<dynamic>("grades")
            .Select("*, course_instances(*, courses(name))")
            .Filter("status", Supabase.Postgrest.Constants.Operator.Equals, "draft")
            .Filter("course_instances.professor_id", Supabase.Postgrest.Constants.Operator.Equals, professorId.ToString())
            .Get();

        var pendingGradesCount = gradesResult?.Models?.Count ?? 0;

        // Get pending contests
        var contestsResult = await _supabaseClient
            .From<dynamic>("grade_contests")
            .Select("*, grades(*, course_instances(professor_id, courses(name)))")
            .Filter("status", Supabase.Postgrest.Constants.Operator.Equals, "pending")
            .Get();

        var professorContests = contestsResult?.Models?.Where(c => 
            c.grades?.course_instances?.professor_id?.ToString() == professorId.ToString())?.Count() ?? 0;

        var tasks = new PendingTasksDto
        {
            GradesToSubmit = pendingGradesCount,
            ContestsToReview = professorContests,
            AttendanceToMark = 0, // Would need separate query
            Items = new List<PendingTaskItemDto>()
        };

        if (pendingGradesCount > 0)
        {
            tasks.Items.Add(new PendingTaskItemDto
            {
                Type = "grades",
                Description = "Submit final grades",
                CourseName = "Multiple courses",
                Count = pendingGradesCount,
                Priority = "high"
            });
        }

        if (professorContests > 0)
        {
            tasks.Items.Add(new PendingTaskItemDto
            {
                Type = "contests",
                Description = "Review grade contests",
                CourseName = "Multiple courses",
                Count = professorContests,
                Priority = "high"
            });
        }

        return tasks;
    }
}
