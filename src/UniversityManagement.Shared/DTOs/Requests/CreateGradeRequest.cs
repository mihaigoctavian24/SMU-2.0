namespace UniversityManagement.Shared.DTOs.Requests;

public class CreateGradeRequest
{
    public Guid StudentId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int Value { get; set; }
    public int Semester { get; set; }
    public DateOnly Date { get; set; }
}
