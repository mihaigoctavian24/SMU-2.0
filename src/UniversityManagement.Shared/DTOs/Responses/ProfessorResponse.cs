namespace UniversityManagement.Shared.DTOs.Responses;

/// <summary>
/// Response DTO for Professor entity
/// </summary>
public class ProfessorResponse
{
    /// <summary>
    /// Professor ID
    /// </summary>
    public Guid Id { get; set; }

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
    /// Full name (FirstName + LastName)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Academic title (e.g., Dr., Prof., Assoc. Prof.)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Full title with name (e.g., "Dr. John Doe")
    /// </summary>
    public string TitleWithName => string.IsNullOrWhiteSpace(Title) ? FullName : $"{Title} {FullName}";

    /// <summary>
    /// Department name
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email address (from User entity)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Number of courses taught
    /// </summary>
    public int CourseCount { get; set; }

    /// <summary>
    /// When the professor record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the professor record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
