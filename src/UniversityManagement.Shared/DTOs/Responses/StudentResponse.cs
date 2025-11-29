using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Shared.DTOs.Responses;

public class StudentResponse
{
    public Guid Id { get; set; }
    public string EnrollmentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cnp { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? GroupName { get; set; }
    public StudentStatus Status { get; set; }
    public DateOnly EnrolledAt { get; set; }
}
