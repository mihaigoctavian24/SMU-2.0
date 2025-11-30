using UniversityManagement.Shared.DTOs.Analytics;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Service for dean analytics and dashboard data
/// </summary>
public interface IDeanAnalyticsService
{
    /// <summary>
    /// Get complete dean dashboard data
    /// </summary>
    Task<DeanDashboardDto> GetDeanDashboardAsync(Guid facultyId);
    
    /// <summary>
    /// Get program comparison metrics within faculty
    /// </summary>
    Task<List<ProgramMetricsDto>> GetProgramComparisonAsync(Guid facultyId);
    
    /// <summary>
    /// Get at-risk students for intervention
    /// </summary>
    Task<List<AtRiskStudentDto>> GetAtRiskStudentsAsync(Guid facultyId, string? riskLevel = null);
    
    /// <summary>
    /// Get grade approval queue
    /// </summary>
    Task<GradeApprovalQueueDto> GetGradeApprovalQueueAsync(Guid facultyId);
    
    /// <summary>
    /// Get enrollment trends for faculty
    /// </summary>
    Task<List<EnrollmentTrendDto>> GetEnrollmentTrendsAsync(Guid facultyId, int yearsBack = 5);
}
