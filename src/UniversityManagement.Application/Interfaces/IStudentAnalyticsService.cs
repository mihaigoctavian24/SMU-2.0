using UniversityManagement.Shared.DTOs.Analytics;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Service for student analytics and dashboard data
/// </summary>
public interface IStudentAnalyticsService
{
    /// <summary>
    /// Get complete student dashboard data
    /// </summary>
    Task<StudentDashboardDto> GetStudentDashboardAsync(Guid studentId);
    
    /// <summary>
    /// Get student GPA trend over semesters
    /// </summary>
    Task<List<GpaTrendDto>> GetGpaTrendAsync(Guid studentId);
    
    /// <summary>
    /// Get student grade distribution
    /// </summary>
    Task<List<GradeDistributionDto>> GetGradeDistributionAsync(Guid studentId);
    
    /// <summary>
    /// Get student attendance summary
    /// </summary>
    Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(Guid studentId, Guid? semesterId = null);
    
    /// <summary>
    /// Get student credit progress
    /// </summary>
    Task<CreditProgressDto> GetCreditProgressAsync(Guid studentId);
}

/// <summary>
/// Credit progress tracking
/// </summary>
public class CreditProgressDto
{
    public int CreditsEarned { get; set; }
    public int TotalCreditsAttempted { get; set; }
    public int TotalCreditsRequired { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<SemesterCreditDto> BySemester { get; set; } = new();
}

/// <summary>
/// Credit breakdown by semester
/// </summary>
public class SemesterCreditDto
{
    public Guid SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public int CreditsEarned { get; set; }
    public int CreditsAttempted { get; set; }
}
