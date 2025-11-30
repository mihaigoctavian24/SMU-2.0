using UniversityManagement.Shared.DTOs.Analytics;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Service for professor analytics and dashboard data
/// </summary>
public interface IProfessorAnalyticsService
{
    /// <summary>
    /// Get complete professor dashboard data
    /// </summary>
    Task<ProfessorDashboardDto> GetProfessorDashboardAsync(Guid professorId, Guid? academicYearId = null);
    
    /// <summary>
    /// Get course performance metrics for professor
    /// </summary>
    Task<List<CoursePerformanceDto>> GetCoursePerformanceAsync(Guid professorId, Guid? academicYearId = null);
    
    /// <summary>
    /// Get grade distribution for a specific course
    /// </summary>
    Task<List<GradeDistributionDto>> GetCourseGradeDistributionAsync(Guid courseInstanceId);
    
    /// <summary>
    /// Get pending tasks for professor
    /// </summary>
    Task<PendingTasksDto> GetPendingTasksAsync(Guid professorId);
}
