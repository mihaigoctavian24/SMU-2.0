namespace UniversityManagement.Shared.DTOs.Responses;

public record AcademicYearResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public int SemesterCount { get; init; }
}

public record SemesterResponse
{
    public Guid Id { get; init; }
    public Guid AcademicYearId { get; init; }
    public string AcademicYearName { get; init; } = string.Empty;
    public int Number { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
