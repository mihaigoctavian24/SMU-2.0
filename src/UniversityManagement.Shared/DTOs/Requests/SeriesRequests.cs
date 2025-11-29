using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Shared.DTOs.Requests;

/// <summary>
/// Request to create a new series
/// </summary>
public class CreateSeriesRequest
{
    /// <summary>
    /// Program ID
    /// </summary>
    [Required(ErrorMessage = "Program ID is required")]
    public Guid ProgramId { get; set; }

    /// <summary>
    /// Series name
    /// </summary>
    [Required(ErrorMessage = "Series name is required")]
    [StringLength(50, ErrorMessage = "Series name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Year of study
    /// </summary>
    [Required(ErrorMessage = "Year of study is required")]
    [Range(1, 10, ErrorMessage = "Year of study must be between 1 and 10")]
    public int YearOfStudy { get; set; }
}

/// <summary>
/// Request to update an existing series
/// </summary>
public class UpdateSeriesRequest
{
    /// <summary>
    /// Program ID
    /// </summary>
    [Required(ErrorMessage = "Program ID is required")]
    public Guid ProgramId { get; set; }

    /// <summary>
    /// Series name
    /// </summary>
    [Required(ErrorMessage = "Series name is required")]
    [StringLength(50, ErrorMessage = "Series name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Year of study
    /// </summary>
    [Required(ErrorMessage = "Year of study is required")]
    [Range(1, 10, ErrorMessage = "Year of study must be between 1 and 10")]
    public int YearOfStudy { get; set; }
}
