namespace UniversityManagement.Shared.DTOs.Requests;

public class CreateAttendanceRequest
{
    public Guid StudentId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public bool IsPresent { get; set; }
}
