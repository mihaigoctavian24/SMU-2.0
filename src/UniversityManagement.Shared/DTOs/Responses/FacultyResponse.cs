namespace UniversityManagement.Shared.DTOs.Responses;

/// <summary>
/// Faculty response DTO
/// </summary>
public class FacultyResponse
{
    /// <summary>
    /// Faculty unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Faculty full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Faculty short name/acronym
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// Dean user ID
    /// </summary>
    public Guid? DeanId { get; set; }

    /// <summary>
    /// Dean name (if available)
    /// </summary>
    public string? DeanName { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Number of programs in this faculty
    /// </summary>
    public int ProgramCount { get; set; }
}
