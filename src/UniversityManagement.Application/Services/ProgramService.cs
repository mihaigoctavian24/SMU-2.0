using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class ProgramService : IProgramService
{
    private readonly IProgramRepository _programRepository;
    private readonly IFacultyRepository _facultyRepository;

    public ProgramService(IProgramRepository programRepository, IFacultyRepository facultyRepository)
    {
        _programRepository = programRepository;
        _facultyRepository = facultyRepository;
    }

    public async Task<ProgramResponse?> GetByIdAsync(Guid id)
    {
        var program = await _programRepository.GetByIdAsync(id);
        if (program == null) return null;

        var faculty = await _facultyRepository.GetByIdAsync(program.FacultyId);
        return MapToResponse(program, faculty?.Name ?? "Unknown");
    }

    public async Task<List<ProgramResponse>> GetAllAsync()
    {
        var programs = await _programRepository.GetAllAsync();
        var responses = new List<ProgramResponse>();

        foreach (var program in programs)
        {
            var faculty = await _facultyRepository.GetByIdAsync(program.FacultyId);
            responses.Add(MapToResponse(program, faculty?.Name ?? "Unknown"));
        }

        return responses;
    }

    public async Task<List<ProgramResponse>> GetByFacultyIdAsync(Guid facultyId)
    {
        var programs = await _programRepository.GetByFacultyIdAsync(facultyId);
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        var facultyName = faculty?.Name ?? "Unknown";

        return programs.Select(p => MapToResponse(p, facultyName)).ToList();
    }

    public async Task<ProgramResponse> CreateAsync(CreateProgramRequest request)
    {
        var facultyExists = await _facultyRepository.ExistsAsync(request.FacultyId);
        if (!facultyExists)
        {
            throw new KeyNotFoundException($"Faculty with ID {request.FacultyId} not found");
        }

        var program = new Program
        {
            FacultyId = request.FacultyId,
            Name = request.Name,
            DegreeLevel = request.DegreeLevel,
            DurationYears = request.DurationYears
        };

        var created = await _programRepository.CreateAsync(program);
        var faculty = await _facultyRepository.GetByIdAsync(created.FacultyId);
        return MapToResponse(created, faculty?.Name ?? "Unknown");
    }

    public async Task<ProgramResponse> UpdateAsync(Guid id, UpdateProgramRequest request)
    {
        var program = await _programRepository.GetByIdAsync(id);
        if (program == null)
        {
            throw new KeyNotFoundException($"Program with ID {id} not found");
        }

        var facultyExists = await _facultyRepository.ExistsAsync(request.FacultyId);
        if (!facultyExists)
        {
            throw new KeyNotFoundException($"Faculty with ID {request.FacultyId} not found");
        }

        program.FacultyId = request.FacultyId;
        program.Name = request.Name;
        program.DegreeLevel = request.DegreeLevel;
        program.DurationYears = request.DurationYears;

        var updated = await _programRepository.UpdateAsync(program);
        var faculty = await _facultyRepository.GetByIdAsync(updated.FacultyId);
        return MapToResponse(updated, faculty?.Name ?? "Unknown");
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _programRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Program with ID {id} not found");
        }

        await _programRepository.DeleteAsync(id);
    }

    private static ProgramResponse MapToResponse(Program program, string facultyName)
    {
        return new ProgramResponse
        {
            Id = program.Id,
            FacultyId = program.FacultyId,
            FacultyName = facultyName,
            Name = program.Name,
            DegreeLevel = program.DegreeLevel,
            DurationYears = program.DurationYears,
            CreatedAt = program.CreatedAt,
            SeriesCount = 0
        };
    }
}
