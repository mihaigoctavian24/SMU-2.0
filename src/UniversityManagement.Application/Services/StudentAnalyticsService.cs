using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Analytics;
using Supabase;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Service implementation for student analytics
/// </summary>
public class StudentAnalyticsService : IStudentAnalyticsService
{
    private readonly Client _supabaseClient;
    private readonly IStudentRepository _studentRepository;

    public StudentAnalyticsService(Client supabaseClient, IStudentRepository studentRepository)
    {
        _supabaseClient = supabaseClient;
        _studentRepository = studentRepository;
    }

    public async Task<StudentDashboardDto> GetStudentDashboardAsync(Guid studentId)
    {
        // Query the v_student_performance view
        var performanceResult = await _supabaseClient
            .From<dynamic>("v_student_performance")
            .Select("*")
            .Filter("student_id", Supabase.Postgrest.Constants.Operator.Equals, studentId.ToString())
            .Single();

        if (performanceResult == null)
        {
            throw new Exception($"Student {studentId} not found in performance view");
        }

        var performance = performanceResult;

        // Get GPA trend for trend calculation
        var gpaTrend = await GetGpaTrendAsync(studentId);
        decimal? previousGpa = gpaTrend.Count > 1 ? gpaTrend[^2].Gpa : null;
        decimal gpaTrendPercent = previousGpa.HasValue && previousGpa.Value > 0
            ? ((performance.current_gpa - previousGpa.Value) / previousGpa.Value) * 100
            : 0;

        // Get recent activities (grades, attendance)
        var recentActivities = await GetRecentActivitiesAsync(studentId);

        // Build dashboard DTO
        var dashboard = new StudentDashboardDto
        {
            StudentId = studentId,
            StudentName = performance.student_name?.ToString() ?? "",
            EnrollmentNumber = performance.enrollment_number?.ToString() ?? "",
            ProgramName = performance.program_name?.ToString() ?? "",
            FacultyName = performance.faculty_name?.ToString() ?? "",
            YearOfStudy = Convert.ToInt32(performance.year_of_study ?? 0),
            
            CurrentGpa = Convert.ToDecimal(performance.current_gpa ?? 0),
            PreviousGpa = previousGpa,
            GpaTrend = gpaTrendPercent,
            
            AttendanceRate = Convert.ToDecimal(performance.attendance_rate ?? 0),
            
            CreditsEarned = Convert.ToInt32(performance.credits_earned ?? 0),
            TotalCreditsAttempted = Convert.ToInt32(performance.total_credits_attempted ?? 0),
            TotalCreditsRequired = 180, // TODO: Get from program
            CreditProgress = Convert.ToInt32(performance.credits_earned ?? 0) * 100m / 180m,
            
            RiskLevel = performance.risk_level?.ToString() ?? "on_track",
            
            EnrolledCoursesCount = Convert.ToInt32(performance.enrolled_courses_count ?? 0),
            PassedCoursesCount = Convert.ToInt32(performance.grade_10_count ?? 0) +
                                Convert.ToInt32(performance.grade_9_count ?? 0) +
                                Convert.ToInt32(performance.grade_8_count ?? 0) +
                                Convert.ToInt32(performance.grade_7_count ?? 0) +
                                Convert.ToInt32(performance.grade_6_count ?? 0) +
                                Convert.ToInt32(performance.grade_5_count ?? 0),
            FailedCoursesCount = Convert.ToInt32(performance.grade_fail_count ?? 0),
            
            AttendanceBreakdown = new AttendanceBreakdownDto
            {
                PresentCount = Convert.ToInt32(performance.present_count ?? 0),
                AbsentCount = Convert.ToInt32(performance.absent_count ?? 0),
                ExcusedCount = Convert.ToInt32(performance.excused_count ?? 0),
                LateCount = Convert.ToInt32(performance.late_count ?? 0),
                TotalSessions = Convert.ToInt32(performance.present_count ?? 0) +
                               Convert.ToInt32(performance.absent_count ?? 0) +
                               Convert.ToInt32(performance.excused_count ?? 0) +
                               Convert.ToInt32(performance.late_count ?? 0),
                PresentPercentage = Convert.ToDecimal(performance.attendance_rate ?? 0),
                AbsentPercentage = CalculatePercentage(performance.absent_count, performance.present_count, 
                                                       performance.absent_count, performance.excused_count, performance.late_count),
                ExcusedPercentage = CalculatePercentage(performance.excused_count, performance.present_count,
                                                        performance.absent_count, performance.excused_count, performance.late_count),
                LatePercentage = CalculatePercentage(performance.late_count, performance.present_count,
                                                     performance.absent_count, performance.excused_count, performance.late_count)
            },
            
            GradeDistribution = new GradeDistributionSummaryDto
            {
                Grade10Count = Convert.ToInt32(performance.grade_10_count ?? 0),
                Grade9Count = Convert.ToInt32(performance.grade_9_count ?? 0),
                Grade8Count = Convert.ToInt32(performance.grade_8_count ?? 0),
                Grade7Count = Convert.ToInt32(performance.grade_7_count ?? 0),
                Grade6Count = Convert.ToInt32(performance.grade_6_count ?? 0),
                Grade5Count = Convert.ToInt32(performance.grade_5_count ?? 0),
                GradeFailCount = Convert.ToInt32(performance.grade_fail_count ?? 0)
            },
            
            RecentActivities = recentActivities
        };

        return dashboard;
    }

    public async Task<List<GpaTrendDto>> GetGpaTrendAsync(Guid studentId)
    {
        // Call database function
        var result = await _supabaseClient.Rpc("get_student_gpa_trend", new { p_student_id = studentId });
        
        var trends = new List<GpaTrendDto>();
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                trends.Add(new GpaTrendDto
                {
                    SemesterName = item.semester_name?.ToString() ?? "",
                    AcademicYear = item.academic_year?.ToString() ?? "",
                    Gpa = Convert.ToDecimal(item.gpa ?? 0),
                    SemesterOrder = Convert.ToInt32(item.semester_order ?? 0),
                    SemesterId = Guid.Parse(item.semester_id?.ToString() ?? Guid.Empty.ToString()),
                    TotalCredits = Convert.ToInt32(item.total_credits ?? 0),
                    CoursesPassed = Convert.ToInt32(item.courses_passed ?? 0),
                    CoursesFailed = Convert.ToInt32(item.courses_failed ?? 0)
                });
            }
        }
        
        return trends;
    }

    public async Task<List<GradeDistributionDto>> GetGradeDistributionAsync(Guid studentId)
    {
        // Call database function
        var result = await _supabaseClient.Rpc("get_student_grade_distribution", new { p_student_id = studentId });
        
        var distributions = new List<GradeDistributionDto>();
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                var courses = new List<string>();
                if (item.courses != null)
                {
                    courses = ((IEnumerable<dynamic>)item.courses).Select(c => c.ToString()).ToList();
                }
                
                distributions.Add(new GradeDistributionDto
                {
                    GradeValue = Convert.ToInt32(item.grade_value ?? 0),
                    GradeCount = Convert.ToInt32(item.grade_count ?? 0),
                    Percentage = Convert.ToDecimal(item.percentage ?? 0),
                    Courses = courses
                });
            }
        }
        
        return distributions;
    }

    public async Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(Guid studentId, Guid? semesterId = null)
    {
        // Call database function
        var parameters = new Dictionary<string, object>
        {
            { "p_student_id", studentId }
        };
        
        if (semesterId.HasValue)
        {
            parameters.Add("p_semester_id", semesterId.Value);
        }
        
        var result = await _supabaseClient.Rpc("get_student_attendance_summary", parameters);
        
        if (result?.Models?.FirstOrDefault() != null)
        {
            var data = result.Models.First();
            
            var summary = new AttendanceSummaryDto
            {
                TotalSessions = Convert.ToInt32(data.total_sessions ?? 0),
                PresentCount = Convert.ToInt32(data.present_count ?? 0),
                AbsentCount = Convert.ToInt32(data.absent_count ?? 0),
                ExcusedCount = Convert.ToInt32(data.excused_count ?? 0),
                LateCount = Convert.ToInt32(data.late_count ?? 0),
                AttendanceRate = Convert.ToDecimal(data.attendance_rate ?? 0),
                ByCourseType = new Dictionary<string, CourseTypeAttendanceDto>()
            };
            
            // Parse JSONB by_course_type
            if (data.by_course_type != null)
            {
                // This would need proper JSON parsing
                // For now, leaving empty - would be implemented based on Supabase response format
            }
            
            return summary;
        }
        
        return new AttendanceSummaryDto();
    }

    public async Task<CreditProgressDto> GetCreditProgressAsync(Guid studentId)
    {
        // This could reuse GPA trend data or query separately
        var gpaTrend = await GetGpaTrendAsync(studentId);
        
        var progress = new CreditProgressDto
        {
            CreditsEarned = gpaTrend.Sum(t => t.CoursesPassed * 6), // Approximate
            TotalCreditsAttempted = gpaTrend.Sum(t => t.TotalCredits),
            TotalCreditsRequired = 180, // TODO: Get from program
            ProgressPercentage = 0,
            BySemester = gpaTrend.Select(t => new SemesterCreditDto
            {
                SemesterId = t.SemesterId,
                SemesterName = t.SemesterName,
                CreditsEarned = t.CoursesPassed * 6, // Approximate
                CreditsAttempted = t.TotalCredits
            }).ToList()
        };
        
        progress.ProgressPercentage = progress.TotalCreditsRequired > 0
            ? (progress.CreditsEarned * 100m / progress.TotalCreditsRequired)
            : 0;
        
        return progress;
    }

    // Helper methods
    private async Task<List<RecentActivityDto>> GetRecentActivitiesAsync(Guid studentId)
    {
        var activities = new List<RecentActivityDto>();
        
        // Get recent grades
        var gradesResult = await _supabaseClient
            .From<dynamic>("grades")
            .Select("*, courses(name)")
            .Filter("student_id", Supabase.Postgrest.Constants.Operator.Equals, studentId.ToString())
            .Order("submitted_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Limit(5)
            .Get();
        
        if (gradesResult?.Models != null)
        {
            foreach (var grade in gradesResult.Models.Take(3))
            {
                activities.Add(new RecentActivityDto
                {
                    Type = "grade",
                    Title = "New Grade",
                    Description = $"Grade {grade.value} for {grade.courses?.name ?? "Unknown Course"}",
                    Timestamp = DateTime.Parse(grade.submitted_at?.ToString() ?? DateTime.Now.ToString()),
                    Icon = "mdi-school",
                    Color = Convert.ToInt32(grade.value ?? 0) >= 5 ? "success" : "error"
                });
            }
        }
        
        return activities.OrderByDescending(a => a.Timestamp).Take(5).ToList();
    }

    private decimal CalculatePercentage(dynamic? value, params dynamic?[] allValues)
    {
        var total = 0;
        foreach (var val in allValues)
        {
            total += Convert.ToInt32(val ?? 0);
        }
        
        if (total == 0) return 0;
        
        return (Convert.ToDecimal(value ?? 0) / total) * 100;
    }
}
