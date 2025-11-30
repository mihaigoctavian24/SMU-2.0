namespace UniversityManagement.Shared.DTOs.Analytics;

/// <summary>
/// Export request parameters for analytics reports
/// </summary>
public class ExportRequestDto
{
    public ExportFormat Format { get; set; } = ExportFormat.Pdf;
    public string ReportType { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new();
}

/// <summary>
/// Export format options
/// </summary>
public enum ExportFormat
{
    Pdf,
    Excel,
    Csv
}

/// <summary>
/// Export result with file information
/// </summary>
public class ExportResultDto
{
    public bool Success { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
