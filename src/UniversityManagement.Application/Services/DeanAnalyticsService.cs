using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Analytics;
using Supabase;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Service implementation for dean analytics
/// </summary>
public class DeanAnalyticsService : IDeanAnalyticsService
{
    private readonly Client _supabaseClient;

    public DeanAnalyticsService(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<DeanDashboardDto> GetDeanDashboardAsync(Guid facultyId)
    {
        // Get faculty statistics from materialized view
        var facultyStats = await _supabaseClient
            .From<dynamic>("mv_faculty_statistics")
            .Select("*")
            .Filter("faculty_id", Supabase.Postgrest.Constants.Operator.Equals, facultyId.ToString())
            .Single();

        if (facultyStats == null)
        {
            throw new Exception($"Faculty {facultyId} not found");
        }

        // Get program comparison
        var programMetrics = await GetProgramComparisonAsync(facultyId);

        // Get at-risk students
        var atRiskStudents = await GetAtRiskStudentsAsync(facultyId, "high");

        // Get grade approval queue
        var gradeApprovalQueue = await GetGradeApprovalQueueAsync(facultyId);

        // Get enrollment trends
        var enrollmentTrends = await GetEnrollmentTrendsAsync(facultyId, 5);

        var dashboard = new DeanDashboardDto
        {
            FacultyId = facultyId,
            FacultyName = facultyStats.faculty_name?.ToString() ?? "",
            
            TotalStudents = Convert.ToInt32(facultyStats.total_students ?? 0),
            TotalStudentsTrend = 0, // Calculate from trends
            OverallPassRate = Convert.ToDecimal(facultyStats.avg_pass_rate ?? 0),
            PassRateTrend = 0,
            AverageGpa = Convert.ToDecimal(facultyStats.avg_grade ?? 0),
            GpaTrend = 0,
            GraduationRate = CalculateGraduationRate(facultyStats),
            GraduationRateTrend = 0,
            
            ProgramMetrics = programMetrics,
            AtRiskStudents = atRiskStudents.Take(20).ToList(),
            GradeApprovalQueue = gradeApprovalQueue,
            EnrollmentTrends = enrollmentTrends
        };

        return dashboard;
    }

    public async Task<List<ProgramMetricsDto>> GetProgramComparisonAsync(Guid facultyId)
    {
        // Call database function
        var result = await _supabaseClient.Rpc("get_program_comparison_metrics", 
            new { p_faculty_id = facultyId });
        
        var metrics = new List<ProgramMetricsDto>();
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                metrics.Add(new ProgramMetricsDto
                {
                    ProgramId = Guid.Parse(item.program_id?.ToString() ?? Guid.Empty.ToString()),
                    ProgramName = item.program_name?.ToString() ?? "",
                    TotalStudents = Convert.ToInt32(item.total_students ?? 0),
                    AverageGpa = Convert.ToDecimal(item.avg_gpa ?? 0),
                    PassRate = Convert.ToDecimal(item.pass_rate ?? 0),
                    RetentionRate = Convert.ToDecimal(item.retention_rate ?? 0),
                    AttendanceRate = Convert.ToDecimal(item.attendance_rate ?? 0),
                    GraduationRate = Convert.ToDecimal(item.graduation_rate ?? 0)
                });
            }
        }
        
        return metrics;
    }

    public async Task<List<AtRiskStudentDto>> GetAtRiskStudentsAsync(Guid facultyId, string? riskLevel = null)
    {
        // Call database function
        var parameters = new Dictionary<string, object>
        {
            { "p_faculty_id", facultyId },
            { "p_risk_level", riskLevel ?? "all" }
        };
        
        var result = await _supabaseClient.Rpc("get_at_risk_students_by_faculty", parameters);
        
        var students = new List<AtRiskStudentDto>();
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                students.Add(new AtRiskStudentDto
                {
                    StudentId = Guid.Parse(item.student_id?.ToString() ?? Guid.Empty.ToString()),
                    StudentName = item.student_name?.ToString() ?? "",
                    EnrollmentNumber = item.enrollment_number?.ToString() ?? "",
                    ProgramName = item.program_name?.ToString() ?? "",
                    YearOfStudy = Convert.ToInt32(item.year_of_study ?? 0),
                    CurrentGpa = Convert.ToDecimal(item.current_gpa ?? 0),
                    AttendanceRate = Convert.ToDecimal(item.attendance_rate ?? 0),
                    FailedCourses = Convert.ToInt32(item.failed_courses ?? 0),
                    RiskLevel = item.risk_level?.ToString() ?? "medium",
                    LastActivityDate = item.last_activity_date != null 
                        ? DateTime.Parse(item.last_activity_date.ToString()) 
                        : null,
                    ContactEmail = "", // Would need to join with users
                    ContactPhone = ""
                });
            }
        }
        
        return students;
    }

    public async Task<GradeApprovalQueueDto> GetGradeApprovalQueueAsync(Guid facultyId)
    {
        // Query v_grade_approval_queue view
        var result = await _supabaseClient
            .From<dynamic>("v_grade_approval_queue")
            .Select("*")
            .Filter("faculty_id", Supabase.Postgrest.Constants.Operator.Equals, facultyId.ToString())
            .Order("submitted_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();

        var queue = new GradeApprovalQueueDto
        {
            PendingCount = 0,
            InReviewCount = 0,
            ApprovedCount = 0,
            ContestedCount = 0,
            Items = new List<GradeApprovalItemDto>()
        };

        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                var status = item.grade_status?.ToString() ?? "";
                
                if (status == "submitted") queue.PendingCount++;
                else if (status == "contested") queue.ContestedCount++;
                else if (status == "approved") queue.ApprovedCount++;

                queue.Items.Add(new GradeApprovalItemDto
                {
                    GradeId = Guid.Parse(item.grade_id?.ToString() ?? Guid.Empty.ToString()),
                    StudentName = item.student_name?.ToString() ?? "",
                    EnrollmentNumber = item.enrollment_number?.ToString() ?? "",
                    CourseName = item.course_name?.ToString() ?? "",
                    CourseCode = item.course_code?.ToString() ?? "",
                    ProfessorName = item.professor_name?.ToString() ?? "",
                    GradeValue = Convert.ToInt32(item.grade_value ?? 0),
                    GradeType = item.grade_type?.ToString() ?? "",
                    Status = status,
                    SubmittedAt = DateTime.Parse(item.submitted_at?.ToString() ?? DateTime.Now.ToString()),
                    DaysPending = Convert.ToInt32(item.days_pending ?? 0),
                    HasContest = Convert.ToInt32(item.contest_count ?? 0) > 0
                });
            }
        }

        return queue;
    }

    public async Task<List<EnrollmentTrendDto>> GetEnrollmentTrendsAsync(Guid facultyId, int yearsBack = 5)
    {
        // Call database function
        var result = await _supabaseClient.Rpc("get_enrollment_trends", 
            new { p_faculty_id = facultyId, p_years_back = yearsBack });
        
        var trends = new List<EnrollmentTrendDto>();
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                trends.Add(new EnrollmentTrendDto
                {
                    AcademicYear = item.academic_year?.ToString() ?? "",
                    YearStartDate = DateTime.Parse(item.year_start_date?.ToString() ?? DateTime.Now.ToString()),
                    TotalEnrollments = Convert.ToInt32(item.total_enrollments ?? 0),
                    ActiveStudents = Convert.ToInt32(item.active_students ?? 0),
                    GraduatedStudents = Convert.ToInt32(item.graduated_students ?? 0),
                    AverageGpa = Convert.ToDecimal(item.avg_gpa ?? 0),
                    PassRate = Convert.ToDecimal(item.pass_rate ?? 0)
                });
            }
        }
        
        return trends;
    }

    private decimal CalculateGraduationRate(dynamic facultyStats)
    {
        var graduated = Convert.ToInt32(facultyStats.graduated_students ?? 0);
        var total = Convert.ToInt32(facultyStats.total_students ?? 0);
        
        return total > 0 ? (graduated * 100m / total) : 0;
    }
}
