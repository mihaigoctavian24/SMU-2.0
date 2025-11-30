using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Analytics;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/analytics/export")]
[Authorize]
public class AnalyticsExportController : ControllerBase
{
    private readonly IAnalyticsExportService _exportService;
    private readonly ILogger<AnalyticsExportController> _logger;

    public AnalyticsExportController(
        IAnalyticsExportService exportService,
        ILogger<AnalyticsExportController> logger)
    {
        _exportService = exportService;
        _logger = logger;
    }

    /// <summary>
    /// Export student dashboard report
    /// </summary>
    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "student,dean,rector,admin")]
    public async Task<IActionResult> ExportStudentDashboard(Guid studentId, [FromQuery] ExportFormat format = ExportFormat.Pdf)
    {
        try
        {
            var fileBytes = await _exportService.ExportStudentDashboardAsync(studentId, format);
            var contentType = GetContentType(format);
            var fileName = $"Student_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}{GetFileExtension(format)}";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting student dashboard for {StudentId}", studentId);
            return StatusCode(500, new { error = "Failed to export student dashboard" });
        }
    }

    /// <summary>
    /// Export professor dashboard report
    /// </summary>
    [HttpGet("professor/{professorId}")]
    [Authorize(Roles = "professor,dean,rector,admin")]
    public async Task<IActionResult> ExportProfessorDashboard(Guid professorId, [FromQuery] ExportFormat format = ExportFormat.Pdf)
    {
        try
        {
            var fileBytes = await _exportService.ExportProfessorDashboardAsync(professorId, format);
            var contentType = GetContentType(format);
            var fileName = $"Professor_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}{GetFileExtension(format)}";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting professor dashboard for {ProfessorId}", professorId);
            return StatusCode(500, new { error = "Failed to export professor dashboard" });
        }
    }

    /// <summary>
    /// Export dean dashboard report
    /// </summary>
    [HttpGet("dean/{deanId}")]
    [Authorize(Roles = "dean,rector,admin")]
    public async Task<IActionResult> ExportDeanDashboard(Guid deanId, [FromQuery] ExportFormat format = ExportFormat.Pdf)
    {
        try
        {
            var fileBytes = await _exportService.ExportDeanDashboardAsync(deanId, format);
            var contentType = GetContentType(format);
            var fileName = $"Dean_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}{GetFileExtension(format)}";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting dean dashboard for {DeanId}", deanId);
            return StatusCode(500, new { error = "Failed to export dean dashboard" });
        }
    }

    /// <summary>
    /// Export rector dashboard report
    /// </summary>
    [HttpGet("rector/{rectorId}")]
    [Authorize(Roles = "rector,admin")]
    public async Task<IActionResult> ExportRectorDashboard(Guid rectorId, [FromQuery] ExportFormat format = ExportFormat.Pdf)
    {
        try
        {
            var fileBytes = await _exportService.ExportRectorDashboardAsync(rectorId, format);
            var contentType = GetContentType(format);
            var fileName = $"Rector_Report_{DateTime.UtcNow:yyyyMMdd_HHmmss}{GetFileExtension(format)}";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting rector dashboard for {RectorId}", rectorId);
            return StatusCode(500, new { error = "Failed to export rector dashboard" });
        }
    }

    /// <summary>
    /// Export custom analytics report
    /// </summary>
    [HttpPost("custom")]
    [Authorize(Roles = "dean,rector,admin")]
    public async Task<IActionResult> ExportCustomReport([FromBody] ExportRequestDto request)
    {
        try
        {
            var fileBytes = await _exportService.ExportCustomReportAsync(request);
            var contentType = GetContentType(request.Format);
            var fileName = $"Custom_Report_{request.ReportType}_{DateTime.UtcNow:yyyyMMdd_HHmmss}{GetFileExtension(request.Format)}";
            
            return File(fileBytes, contentType, fileName);
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, new { error = "Custom reports feature is not yet implemented" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting custom report");
            return StatusCode(500, new { error = "Failed to export custom report" });
        }
    }

    private static string GetContentType(ExportFormat format) => format switch
    {
        ExportFormat.Pdf => "application/pdf",
        ExportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        ExportFormat.Csv => "text/csv",
        _ => "application/octet-stream"
    };

    private static string GetFileExtension(ExportFormat format) => format switch
    {
        ExportFormat.Pdf => ".pdf",
        ExportFormat.Excel => ".xlsx",
        ExportFormat.Csv => ".csv",
        _ => ".dat"
    };
}
