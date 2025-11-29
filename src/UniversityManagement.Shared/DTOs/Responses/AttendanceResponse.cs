namespace UniversityManagement.Shared.DTOs.Responses;

public class AttendanceResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public bool IsPresent { get; set; }
}
