namespace UniversityManagement.Shared.DTOs.Responses;

public class GradeResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int Value { get; set; }
    public int Semester { get; set; }
    public DateOnly Date { get; set; }
}
