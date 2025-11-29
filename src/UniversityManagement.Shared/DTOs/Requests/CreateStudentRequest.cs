namespace UniversityManagement.Shared.DTOs.Requests;

public class CreateStudentRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cnp { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateOnly? BirthDate { get; set; }
    public Guid? GroupId { get; set; }
}
