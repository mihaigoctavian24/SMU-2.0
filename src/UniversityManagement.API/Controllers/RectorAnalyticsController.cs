using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/analytics/rector")]
[Authorize(Roles = "rector,admin")]
public class RectorAnalyticsController : ControllerBase
{
    private readonly IRectorAnalyticsService _analyticsService;
    private readonly ILogger<RectorAnalyticsController> _logger;

    public RectorAnalyticsController(
        IRectorAnalyticsService analyticsService,
        ILogger<RectorAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get complete rector dashboard data
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid? academicYearId = null)
    {
        try
        {
            var dashboard = await _analyticsService.GetRectorDashboardAsync(academicYearId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rector dashboard");
            return StatusCode(500, new { error = "Failed to retrieve dashboard data" });
        }
    }

    /// <summary>
    /// Get university-wide KPI metrics
    /// </summary>
    [HttpGet("university-kpis")]
    public async Task<IActionResult> GetUniversityKpis([FromQuery] Guid? academicYearId = null)
    {
        try
        {
            var kpis = await _analyticsService.GetUniversityKpisAsync(academicYearId);
            return Ok(kpis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting university KPIs");
            return StatusCode(500, new { error = "Failed to retrieve university KPIs" });
        }
    }

    /// <summary>
    /// Get faculty comparison data
    /// </summary>
    [HttpGet("faculty-comparison")]
    public async Task<IActionResult> GetFacultyComparison()
    {
        try
        {
            var comparison = await _analyticsService.GetFacultyComparisonAsync();
            return Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting faculty comparison");
            return StatusCode(500, new { error = "Failed to retrieve faculty comparison" });
        }
    }

    /// <summary>
    /// Get historical performance trends
    /// </summary>
    [HttpGet("historical-performance")]
    public async Task<IActionResult> GetHistoricalPerformance([FromQuery] int yearsBack = 5)
    {
        try
        {
            var performance = await _analyticsService.GetHistoricalPerformanceAsync(yearsBack);
            return Ok(performance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting historical performance");
            return StatusCode(500, new { error = "Failed to retrieve historical performance" });
        }
    }

    /// <summary>
    /// Get strategic KPIs
    /// </summary>
    [HttpGet("strategic-kpis")]
    public async Task<IActionResult> GetStrategicKpis()
    {
        try
        {
            var kpis = await _analyticsService.GetStrategicKpisAsync();
            return Ok(kpis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting strategic KPIs");
            return StatusCode(500, new { error = "Failed to retrieve strategic KPIs" });
        }
    }
}
