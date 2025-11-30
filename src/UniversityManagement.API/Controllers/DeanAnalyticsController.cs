using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/analytics/dean")]
[Authorize(Roles = "dean,rector,admin")]
public class DeanAnalyticsController : ControllerBase
{
    private readonly IDeanAnalyticsService _analyticsService;
    private readonly ILogger<DeanAnalyticsController> _logger;

    public DeanAnalyticsController(
        IDeanAnalyticsService analyticsService,
        ILogger<DeanAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get complete dean dashboard data
    /// </summary>
    [HttpGet("{facultyId}/dashboard")]
    public async Task<IActionResult> GetDashboard(Guid facultyId)
    {
        try
        {
            var dashboard = await _analyticsService.GetDeanDashboardAsync(facultyId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dean dashboard for faculty {FacultyId}", facultyId);
            return StatusCode(500, new { error = "Failed to retrieve dashboard data" });
        }
    }

    /// <summary>
    /// Get program comparison metrics within faculty
    /// </summary>
    [HttpGet("{facultyId}/program-comparison")]
    public async Task<IActionResult> GetProgramComparison(Guid facultyId)
    {
        try
        {
            var metrics = await _analyticsService.GetProgramComparisonAsync(facultyId);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting program comparison for faculty {FacultyId}", facultyId);
            return StatusCode(500, new { error = "Failed to retrieve program comparison" });
        }
    }

    /// <summary>
    /// Get at-risk students for intervention
    /// </summary>
    [HttpGet("{facultyId}/at-risk-students")]
    public async Task<IActionResult> GetAtRiskStudents(Guid facultyId, [FromQuery] string? riskLevel = null)
    {
        try
        {
            var students = await _analyticsService.GetAtRiskStudentsAsync(facultyId, riskLevel);
            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting at-risk students for faculty {FacultyId}", facultyId);
            return StatusCode(500, new { error = "Failed to retrieve at-risk students" });
        }
    }

    /// <summary>
    /// Get grade approval queue
    /// </summary>
    [HttpGet("{facultyId}/grade-approval-queue")]
    public async Task<IActionResult> GetGradeApprovalQueue(Guid facultyId)
    {
        try
        {
            var queue = await _analyticsService.GetGradeApprovalQueueAsync(facultyId);
            return Ok(queue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grade approval queue for faculty {FacultyId}", facultyId);
            return StatusCode(500, new { error = "Failed to retrieve grade approval queue" });
        }
    }

    /// <summary>
    /// Get enrollment trends for faculty
    /// </summary>
    [HttpGet("{facultyId}/enrollment-trends")]
    public async Task<IActionResult> GetEnrollmentTrends(Guid facultyId, [FromQuery] int yearsBack = 5)
    {
        try
        {
            var trends = await _analyticsService.GetEnrollmentTrendsAsync(facultyId, yearsBack);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enrollment trends for faculty {FacultyId}", facultyId);
            return StatusCode(500, new { error = "Failed to retrieve enrollment trends" });
        }
    }
}
