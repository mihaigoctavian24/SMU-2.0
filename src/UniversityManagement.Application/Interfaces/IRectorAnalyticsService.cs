using UniversityManagement.Shared.DTOs.Analytics;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Service for rector analytics and university-wide dashboard data
/// </summary>
public interface IRectorAnalyticsService
{
    /// <summary>
    /// Get complete rector dashboard data
    /// </summary>
    Task<RectorDashboardDto> GetRectorDashboardAsync(Guid? academicYearId = null);
    
    /// <summary>
    /// Get university-wide KPI metrics
    /// </summary>
    Task<UniversityKpisDto> GetUniversityKpisAsync(Guid? academicYearId = null);
    
    /// <summary>
    /// Get faculty comparison data
    /// </summary>
    Task<List<FacultyComparisonDto>> GetFacultyComparisonAsync();
    
    /// <summary>
    /// Get historical performance trends
    /// </summary>
    Task<List<HistoricalPerformanceDto>> GetHistoricalPerformanceAsync(int yearsBack = 5);
    
    /// <summary>
    /// Get strategic KPIs
    /// </summary>
    Task<StrategicKpisDto> GetStrategicKpisAsync();
}
