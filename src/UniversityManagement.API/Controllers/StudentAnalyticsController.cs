using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/analytics/student")]
[Authorize(Roles = "student,dean,rector,admin")]
public class StudentAnalyticsController : ControllerBase
{
    private readonly IStudentAnalyticsService _analyticsService;
    private readonly ILogger<StudentAnalyticsController> _logger;

    public StudentAnalyticsController(
        IStudentAnalyticsService analyticsService,
        ILogger<StudentAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get complete student dashboard data
    /// </summary>
    [HttpGet("{studentId}/dashboard")]
    public async Task<IActionResult> GetDashboard(Guid studentId)
    {
        try
        {
            var dashboard = await _analyticsService.GetStudentDashboardAsync(studentId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student dashboard for {StudentId}", studentId);
            return StatusCode(500, new { error = "Failed to retrieve dashboard data" });
        }
    }

    /// <summary>
    /// Get student GPA trend over semesters
    /// </summary>
    [HttpGet("{studentId}/gpa-trend")]
    public async Task<IActionResult> GetGpaTrend(Guid studentId)
    {
        try
        {
            var trend = await _analyticsService.GetGpaTrendAsync(studentId);
            return Ok(trend);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GPA trend for {StudentId}", studentId);
            return StatusCode(500, new { error = "Failed to retrieve GPA trend" });
        }
    }

    /// <summary>
    /// Get student grade distribution
    /// </summary>
    [HttpGet("{studentId}/grade-distribution")]
    public async Task<IActionResult> GetGradeDistribution(Guid studentId)
    {
        try
        {
            var distribution = await _analyticsService.GetGradeDistributionAsync(studentId);
            return Ok(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grade distribution for {StudentId}", studentId);
            return StatusCode(500, new { error = "Failed to retrieve grade distribution" });
        }
    }

    /// <summary>
    /// Get student attendance summary
    /// </summary>
    [HttpGet("{studentId}/attendance-summary")]
    public async Task<IActionResult> GetAttendanceSummary(Guid studentId, [FromQuery] Guid? semesterId = null)
    {
        try
        {
            var summary = await _analyticsService.GetAttendanceSummaryAsync(studentId, semesterId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attendance summary for {StudentId}", studentId);
            return StatusCode(500, new { error = "Failed to retrieve attendance summary" });
        }
    }

    /// <summary>
    /// Get student credit progress
    /// </summary>
    [HttpGet("{studentId}/credit-progress")]
    public async Task<IActionResult> GetCreditProgress(Guid studentId)
    {
        try
        {
            var progress = await _analyticsService.GetCreditProgressAsync(studentId);
            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credit progress for {StudentId}", studentId);
            return StatusCode(500, new { error = "Failed to retrieve credit progress" });
        }
    }

    /// <summary>
    /// Get current user's dashboard (for logged-in students)
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> GetMyDashboard()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            // Would need to get student ID from user ID
            // For now, using userId as studentId (would need proper mapping)
            var dashboard = await _analyticsService.GetStudentDashboardAsync(userId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard for current user");
            return StatusCode(500, new { error = "Failed to retrieve dashboard data" });
        }
    }
}
