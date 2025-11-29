using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Shared.DTOs.Requests;

/// <summary>
/// Request to create a new academic program
/// </summary>
public class CreateProgramRequest
{
    /// <summary>
    /// Faculty ID
    /// </summary>
    [Required(ErrorMessage = "Faculty ID is required")]
    public Guid FacultyId { get; set; }

    /// <summary>
    /// Program name
    /// </summary>
    [Required(ErrorMessage = "Program name is required")]
    [StringLength(255, ErrorMessage = "Program name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Degree level (bachelor, master, phd)
    /// </summary>
    [Required(ErrorMessage = "Degree level is required")]
    public string DegreeLevel { get; set; } = string.Empty;

    /// <summary>
    /// Program duration in years
    /// </summary>
    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 10, ErrorMessage = "Duration must be between 1 and 10 years")]
    public int DurationYears { get; set; }
}

/// <summary>
/// Request to update an existing academic program
/// </summary>
public class UpdateProgramRequest
{
    /// <summary>
    /// Faculty ID
    /// </summary>
    [Required(ErrorMessage = "Faculty ID is required")]
    public Guid FacultyId { get; set; }

    /// <summary>
    /// Program name
    /// </summary>
    [Required(ErrorMessage = "Program name is required")]
    [StringLength(255, ErrorMessage = "Program name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Degree level (bachelor, master, phd)
    /// </summary>
    [Required(ErrorMessage = "Degree level is required")]
    public string DegreeLevel { get; set; } = string.Empty;

    /// <summary>
    /// Program duration in years
    /// </summary>
    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 10, ErrorMessage = "Duration must be between 1 and 10 years")]
    public int DurationYears { get; set; }
}
