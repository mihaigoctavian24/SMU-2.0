namespace UniversityManagement.Shared.DTOs.Requests;

public record CreateAcademicYearRequest
{
    public required string Name { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public bool IsCurrent { get; init; } = false;
}

public record UpdateAcademicYearRequest
{
    public required string Name { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}

public record CreateSemesterRequest
{
    public required Guid AcademicYearId { get; init; }
    public required int Number { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}

public record UpdateSemesterRequest
{
    public required Guid AcademicYearId { get; init; }
    public required int Number { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
}
