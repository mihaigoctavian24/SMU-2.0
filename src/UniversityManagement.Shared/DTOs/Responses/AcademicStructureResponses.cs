namespace UniversityManagement.Shared.DTOs.Responses;

public record ProgramResponse
{
    public Guid Id { get; init; }
    public Guid FacultyId { get; init; }
    public string FacultyName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string DegreeLevel { get; init; } = string.Empty;
    public int DurationYears { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public int SeriesCount { get; init; }
}

public record SeriesResponse
{
    public Guid Id { get; init; }
    public Guid ProgramId { get; init; }
    public string ProgramName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int YearOfStudy { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public int GroupCount { get; init; }
}

public record GroupResponse
{
    public Guid Id { get; init; }
    public Guid SeriesId { get; init; }
    public string SeriesName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public int StudentCount { get; init; }
}
