using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class FacultyService : IFacultyService
{
    private readonly IFacultyRepository _facultyRepository;

    public FacultyService(IFacultyRepository facultyRepository)
    {
        _facultyRepository = facultyRepository;
    }

    public async Task<FacultyResponse?> GetByIdAsync(Guid id)
    {
        var faculty = await _facultyRepository.GetByIdAsync(id);
        return faculty == null ? null : MapToResponse(faculty);
    }

    public async Task<List<FacultyResponse>> GetAllAsync()
    {
        var faculties = await _facultyRepository.GetAllAsync();
        return faculties.Select(MapToResponse).ToList();
    }

    public async Task<FacultyResponse> CreateAsync(CreateFacultyRequest request)
    {
        var faculty = new Faculty
        {
            Name = request.Name,
            ShortName = request.ShortName,
            DeanId = request.DeanId
        };

        var created = await _facultyRepository.CreateAsync(faculty);
        return MapToResponse(created);
    }

    public async Task<FacultyResponse> UpdateAsync(Guid id, UpdateFacultyRequest request)
    {
        var faculty = await _facultyRepository.GetByIdAsync(id);
        if (faculty == null)
        {
            throw new KeyNotFoundException($"Faculty with ID {id} not found");
        }

        faculty.Name = request.Name;
        faculty.ShortName = request.ShortName;
        faculty.DeanId = request.DeanId;

        var updated = await _facultyRepository.UpdateAsync(faculty);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _facultyRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Faculty with ID {id} not found");
        }

        await _facultyRepository.DeleteAsync(id);
    }

    public async Task<List<FacultyResponse>> SearchAsync(string searchTerm)
    {
        var faculties = await _facultyRepository.SearchByNameAsync(searchTerm);
        return faculties.Select(MapToResponse).ToList();
    }

    private static FacultyResponse MapToResponse(Faculty faculty)
    {
        return new FacultyResponse
        {
            Id = faculty.Id,
            Name = faculty.Name,
            ShortName = faculty.ShortName,
            DeanId = faculty.DeanId,
            CreatedAt = faculty.CreatedAt,
            UpdatedAt = faculty.UpdatedAt,
            ProgramCount = 0 // TODO: Load from related programs
        };
    }
}
