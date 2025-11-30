using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/analytics/professor")]
[Authorize(Roles = "professor,dean,rector,admin")]
public class ProfessorAnalyticsController : ControllerBase
{
    private readonly IProfessorAnalyticsService _analyticsService;
    private readonly ILogger<ProfessorAnalyticsController> _logger;

    public ProfessorAnalyticsController(
        IProfessorAnalyticsService analyticsService,
        ILogger<ProfessorAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get complete professor dashboard data
    /// </summary>
    [HttpGet("{professorId}/dashboard")]
    public async Task<IActionResult> GetDashboard(Guid professorId, [FromQuery] Guid? academicYearId = null)
    {
        try
        {
            var dashboard = await _analyticsService.GetProfessorDashboardAsync(professorId, academicYearId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting professor dashboard for {ProfessorId}", professorId);
            return StatusCode(500, new { error = "Failed to retrieve dashboard data" });
        }
    }

    /// <summary>
    /// Get course performance metrics for professor
    /// </summary>
    [HttpGet("{professorId}/course-performance")]
    public async Task<IActionResult> GetCoursePerformance(Guid professorId, [FromQuery] Guid? academicYearId = null)
    {
        try
        {
            var performance = await _analyticsService.GetCoursePerformanceAsync(professorId, academicYearId);
            return Ok(performance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course performance for {ProfessorId}", professorId);
            return StatusCode(500, new { error = "Failed to retrieve course performance" });
        }
    }

    /// <summary>
    /// Get grade distribution for a specific course
    /// </summary>
    [HttpGet("course/{courseInstanceId}/grade-distribution")]
    public async Task<IActionResult> GetCourseGradeDistribution(Guid courseInstanceId)
    {
        try
        {
            var distribution = await _analyticsService.GetCourseGradeDistributionAsync(courseInstanceId);
            return Ok(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grade distribution for course {CourseInstanceId}", courseInstanceId);
            return StatusCode(500, new { error = "Failed to retrieve grade distribution" });
        }
    }

    /// <summary>
    /// Get pending tasks for professor
    /// </summary>
    [HttpGet("{professorId}/pending-tasks")]
    public async Task<IActionResult> GetPendingTasks(Guid professorId)
    {
        try
        {
            var tasks = await _analyticsService.GetPendingTasksAsync(professorId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending tasks for {ProfessorId}", professorId);
            return StatusCode(500, new { error = "Failed to retrieve pending tasks" });
        }
    }

    /// <summary>
    /// Get current professor's dashboard
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "professor")]
    public async Task<IActionResult> GetMyDashboard([FromQuery] Guid? academicYearId = null)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            // Would need to get professor ID from user ID
            var dashboard = await _analyticsService.GetProfessorDashboardAsync(userId, academicYearId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard for current professor");
            return StatusCode(500, new { error = "Failed to retrieve dashboard data" });
        }
    }
}
