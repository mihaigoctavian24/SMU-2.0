using UniversityManagement.Shared.DTOs.Analytics;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Service for exporting analytics reports in various formats
/// </summary>
public interface IAnalyticsExportService
{
    /// <summary>
    /// Export student dashboard to specified format
    /// </summary>
    Task<byte[]> ExportStudentDashboardAsync(Guid studentId, ExportFormat format);
    
    /// <summary>
    /// Export professor dashboard to specified format
    /// </summary>
    Task<byte[]> ExportProfessorDashboardAsync(Guid professorId, ExportFormat format);
    
    /// <summary>
    /// Export dean dashboard to specified format
    /// </summary>
    Task<byte[]> ExportDeanDashboardAsync(Guid deanId, ExportFormat format);
    
    /// <summary>
    /// Export rector dashboard to specified format
    /// </summary>
    Task<byte[]> ExportRectorDashboardAsync(Guid rectorId, ExportFormat format);
    
    /// <summary>
    /// Export custom analytics report
    /// </summary>
    Task<byte[]> ExportCustomReportAsync(ExportRequestDto request);
}
