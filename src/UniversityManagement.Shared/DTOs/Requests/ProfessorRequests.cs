namespace UniversityManagement.Shared.DTOs.Requests;

/// <summary>
/// Request for creating a new professor
/// </summary>
public class CreateProfessorRequest
{
    /// <summary>
    /// User ID associated with the professor
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Professor's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Professor's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Academic title (e.g., Dr., Prof., Assoc. Prof.)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Department name
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Faculty ID (optional for now, can be used for faculty association)
    /// </summary>
    public Guid? FacultyId { get; set; }
}

/// <summary>
/// Request for updating an existing professor
/// </summary>
public class UpdateProfessorRequest
{
    /// <summary>
    /// Professor's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Professor's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Academic title (e.g., Dr., Prof., Assoc. Prof.)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Department name
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Faculty ID (optional for now, can be used for faculty association)
    /// </summary>
    public Guid? FacultyId { get; set; }
}
