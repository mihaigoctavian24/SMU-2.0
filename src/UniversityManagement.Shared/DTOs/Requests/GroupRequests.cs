using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Shared.DTOs.Requests;

/// <summary>
/// Request to create a new group
/// </summary>
public class CreateGroupRequest
{
    /// <summary>
    /// Series ID
    /// </summary>
    [Required(ErrorMessage = "Series ID is required")]
    public Guid SeriesId { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(50, ErrorMessage = "Group name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request to update an existing group
/// </summary>
public class UpdateGroupRequest
{
    /// <summary>
    /// Series ID
    /// </summary>
    [Required(ErrorMessage = "Series ID is required")]
    public Guid SeriesId { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(50, ErrorMessage = "Group name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
}
