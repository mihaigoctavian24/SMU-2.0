using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Analytics;
using Supabase;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Service implementation for rector analytics
/// </summary>
public class RectorAnalyticsService : IRectorAnalyticsService
{
    private readonly Client _supabaseClient;

    public RectorAnalyticsService(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<RectorDashboardDto> GetRectorDashboardAsync(Guid? academicYearId = null)
    {
        var universityKpis = await GetUniversityKpisAsync(academicYearId);
        var facultyComparisons = await GetFacultyComparisonAsync();
        var historicalPerformance = await GetHistoricalPerformanceAsync(5);
        var strategicKpis = await GetStrategicKpisAsync();

        var dashboard = new RectorDashboardDto
        {
            RectorId = Guid.Empty, // Would need to get from context
            RectorName = "Rector", // Would need to get from context
            UniversityKpis = universityKpis,
            FacultyComparisons = facultyComparisons,
            HistoricalPerformance = historicalPerformance,
            StrategicKpis = strategicKpis
        };

        return dashboard;
    }

    public async Task<UniversityKpisDto> GetUniversityKpisAsync(Guid? academicYearId = null)
    {
        // Call database function
        var parameters = academicYearId.HasValue 
            ? new Dictionary<string, object> { { "p_academic_year_id", academicYearId.Value } }
            : new Dictionary<string, object>();
        
        var result = await _supabaseClient.Rpc("get_university_kpis", parameters);
        
        if (result?.Models?.FirstOrDefault() != null)
        {
            var data = result.Models.First();
            
            return new UniversityKpisDto
            {
                TotalStudents = Convert.ToInt32(data.total_students ?? 0),
                TotalStudentsTrend = 0, // Calculate from historical
                TotalFaculties = Convert.ToInt32(data.total_faculties ?? 0),
                TotalPrograms = Convert.ToInt32(data.total_programs ?? 0),
                TotalProfessors = Convert.ToInt32(data.total_professors ?? 0),
                OverallAverageGpa = Convert.ToDecimal(data.overall_avg_gpa ?? 0),
                GpaTrend = 0,
                OverallPassRate = Convert.ToDecimal(data.overall_pass_rate ?? 0),
                PassRateTrend = 0,
                OverallRetentionRate = Convert.ToDecimal(data.overall_retention_rate ?? 0),
                RetentionRateTrend = 0,
                OverallGraduationRate = Convert.ToDecimal(data.overall_graduation_rate ?? 0),
                GraduationRateTrend = 0,
                OverallAttendanceRate = Convert.ToDecimal(data.overall_attendance_rate ?? 0),
                ActiveStudents = Convert.ToInt32(data.active_students ?? 0),
                GraduatedStudents = Convert.ToInt32(data.graduated_students ?? 0),
                PendingGradeApprovals = Convert.ToInt32(data.pending_grade_approvals ?? 0)
            };
        }
        
        return new UniversityKpisDto();
    }

    public async Task<List<FacultyComparisonDto>> GetFacultyComparisonAsync()
    {
        // Query materialized view
        var result = await _supabaseClient
            .From<dynamic>("mv_faculty_statistics")
            .Select("*")
            .Get();

        var comparisons = new List<FacultyComparisonDto>();
        
        if (result?.Models != null)
        {
            foreach (var item in result.Models)
            {
                comparisons.Add(new FacultyComparisonDto
                {
                    FacultyId = Guid.Parse(item.faculty_id?.ToString() ?? Guid.Empty.ToString()),
                    FacultyName = item.faculty_name?.ToString() ?? "",
                    FacultyShortName = item.faculty_short_name?.ToString() ?? "",
                    TotalStudents = Convert.ToInt32(item.total_students ?? 0),
                    TotalPrograms = Convert.ToInt32(item.total_programs ?? 0),
                    TotalProfessors = Convert.ToInt32(item.total_professors ?? 0),
                    AverageGpa = Convert.ToDecimal(item.avg_grade ?? 0),
                    PassRate = Convert.ToDecimal(item.avg_pass_rate ?? 0),
                    GraduationRate = CalculateGraduationRate(item),
                    RetentionRate = CalculateRetentionRate(item),
                    AttendanceRate = 0 // Not in materialized view
                });
            }
        }

        return comparisons;
    }

    public async Task<List<HistoricalPerformanceDto>> GetHistoricalPerformanceAsync(int yearsBack = 5)
    {
        // Call database function
        var result = await _supabaseClient.Rpc("get_enrollment_trends", 
            new { p_faculty_id = (Guid?)null, p_years_back = yearsBack });
        
        var performance = new List<HistoricalPerformanceDto>();
        
        if (result?.Models != null)
        {
            // Group by academic year (aggregate across all faculties)
            var grouped = result.Models
                .GroupBy(x => new { 
                    AcademicYear = x.academic_year?.ToString(),
                    YearStartDate = x.year_start_date
                });

            foreach (var group in grouped)
            {
                var totalEnrollments = group.Sum(x => Convert.ToInt32(x.total_enrollments ?? 0));
                var totalGraduated = group.Sum(x => Convert.ToInt32(x.graduated_students ?? 0));
                var avgGpa = group.Average(x => Convert.ToDecimal(x.avg_gpa ?? 0));
                var avgPassRate = group.Average(x => Convert.ToDecimal(x.pass_rate ?? 0));

                performance.Add(new HistoricalPerformanceDto
                {
                    AcademicYear = group.Key.AcademicYear ?? "",
                    YearStartDate = group.Key.YearStartDate != null 
                        ? DateTime.Parse(group.Key.YearStartDate.ToString()) 
                        : DateTime.Now,
                    TotalEnrollments = totalEnrollments,
                    AverageGpa = avgGpa,
                    PassRate = avgPassRate,
                    GraduationRate = totalEnrollments > 0 ? (totalGraduated * 100m / totalEnrollments) : 0,
                    RetentionRate = 0 // Would need additional calculation
                });
            }
        }

        return performance.OrderByDescending(p => p.YearStartDate).ToList();
    }

    public async Task<StrategicKpisDto> GetStrategicKpisAsync()
    {
        var universityKpis = await GetUniversityKpisAsync();
        
        // Calculate strategic metrics
        var enrollmentTarget = 20000; // This would come from system settings
        var currentEnrollment = universityKpis.TotalStudents;
        
        var qualityScore = CalculateQualityScore(
            universityKpis.OverallAverageGpa,
            universityKpis.OverallPassRate,
            universityKpis.OverallRetentionRate,
            universityKpis.OverallGraduationRate
        );

        var avgStudentsPerProfessor = universityKpis.TotalProfessors > 0
            ? (decimal)universityKpis.TotalStudents / universityKpis.TotalProfessors
            : 0;

        var strategicKpis = new StrategicKpisDto
        {
            EnrollmentTargetProgress = enrollmentTarget > 0 
                ? (currentEnrollment * 100m / enrollmentTarget) 
                : 0,
            EnrollmentTarget = enrollmentTarget,
            CurrentEnrollment = currentEnrollment,
            QualityScore = qualityScore,
            StudentSatisfactionScore = null, // Would need survey data
            AverageStudentsPerProfessor = avgStudentsPerProfessor,
            AverageClassSize = 30 // Would need actual calculation
        };

        return strategicKpis;
    }

    // Helper methods
    private decimal CalculateGraduationRate(dynamic facultyStats)
    {
        var graduated = Convert.ToInt32(facultyStats.graduated_students ?? 0);
        var total = Convert.ToInt32(facultyStats.total_students ?? 0);
        
        return total > 0 ? (graduated * 100m / total) : 0;
    }

    private decimal CalculateRetentionRate(dynamic facultyStats)
    {
        var active = Convert.ToInt32(facultyStats.active_students ?? 0);
        var graduated = Convert.ToInt32(facultyStats.graduated_students ?? 0);
        var total = Convert.ToInt32(facultyStats.total_students ?? 0);
        
        return total > 0 ? ((active + graduated) * 100m / total) : 0;
    }

    private decimal CalculateQualityScore(decimal gpa, decimal passRate, decimal retentionRate, decimal graduationRate)
    {
        // Weighted quality score (0-10 scale)
        // GPA normalized to 0-10: gpa itself
        // Pass rate, retention, graduation: 0-100 normalized to 0-10
        
        var gpaScore = gpa; // Already 0-10
        var passRateScore = passRate / 10m;
        var retentionScore = retentionRate / 10m;
        var graduationScore = graduationRate / 10m;
        
        // Weighted average
        var qualityScore = (gpaScore * 0.3m) + 
                          (passRateScore * 0.3m) + 
                          (retentionScore * 0.2m) + 
                          (graduationScore * 0.2m);
        
        return Math.Round(qualityScore, 2);
    }
}
