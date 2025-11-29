using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Shared.DTOs.Requests;

/// <summary>
/// Request to create a new faculty
/// </summary>
public class CreateFacultyRequest
{
    /// <summary>
    /// Faculty full name
    /// </summary>
    [Required(ErrorMessage = "Faculty name is required")]
    [StringLength(255, ErrorMessage = "Faculty name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Faculty short name/acronym
    /// </summary>
    [StringLength(50, ErrorMessage = "Short name cannot exceed 50 characters")]
    public string? ShortName { get; set; }

    /// <summary>
    /// Dean user ID (optional)
    /// </summary>
    public Guid? DeanId { get; set; }
}

/// <summary>
/// Request to update an existing faculty
/// </summary>
public class UpdateFacultyRequest
{
    /// <summary>
    /// Faculty full name
    /// </summary>
    [Required(ErrorMessage = "Faculty name is required")]
    [StringLength(255, ErrorMessage = "Faculty name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Faculty short name/acronym
    /// </summary>
    [StringLength(50, ErrorMessage = "Short name cannot exceed 50 characters")]
    public string? ShortName { get; set; }

    /// <summary>
    /// Dean user ID (optional)
    /// </summary>
    public Guid? DeanId { get; set; }
}
