using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class AcademicYearService : IAcademicYearService
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ISemesterRepository _semesterRepository;

    public AcademicYearService(
        IAcademicYearRepository academicYearRepository,
        ISemesterRepository semesterRepository)
    {
        _academicYearRepository = academicYearRepository;
        _semesterRepository = semesterRepository;
    }

    public async Task<AcademicYearResponse?> GetByIdAsync(Guid id)
    {
        var academicYear = await _academicYearRepository.GetByIdAsync(id);
        if (academicYear == null) return null;

        var semesters = await _semesterRepository.GetByAcademicYearIdAsync(id);
        return MapToResponse(academicYear, semesters.Count);
    }

    public async Task<List<AcademicYearResponse>> GetAllAsync()
    {
        var academicYears = await _academicYearRepository.GetAllAsync();
        var responses = new List<AcademicYearResponse>();

        foreach (var academicYear in academicYears)
        {
            var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYear.Id);
            responses.Add(MapToResponse(academicYear, semesters.Count));
        }

        return responses;
    }

    public async Task<AcademicYearResponse?> GetCurrentAsync()
    {
        var academicYear = await _academicYearRepository.GetCurrentAsync();
        if (academicYear == null) return null;

        var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYear.Id);
        return MapToResponse(academicYear, semesters.Count);
    }

    public async Task<AcademicYearResponse> CreateAsync(CreateAcademicYearRequest request)
    {
        var academicYear = new AcademicYear
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent
        };

        var created = await _academicYearRepository.CreateAsync(academicYear);
        
        // If this is marked as current, set it as current
        if (request.IsCurrent)
        {
            await _academicYearRepository.SetCurrentAsync(created.Id);
        }

        return MapToResponse(created, 0);
    }

    public async Task<AcademicYearResponse> UpdateAsync(Guid id, UpdateAcademicYearRequest request)
    {
        var academicYear = await _academicYearRepository.GetByIdAsync(id);
        if (academicYear == null)
        {
            throw new KeyNotFoundException($"Academic year with ID {id} not found");
        }

        academicYear.Name = request.Name;
        academicYear.StartDate = request.StartDate;
        academicYear.EndDate = request.EndDate;

        var updated = await _academicYearRepository.UpdateAsync(academicYear);
        var semesters = await _semesterRepository.GetByAcademicYearIdAsync(id);
        return MapToResponse(updated, semesters.Count);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _academicYearRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Academic year with ID {id} not found");
        }

        await _academicYearRepository.DeleteAsync(id);
    }

    public async Task<AcademicYearResponse> SetCurrentAsync(Guid id)
    {
        var academicYear = await _academicYearRepository.GetByIdAsync(id);
        if (academicYear == null)
        {
            throw new KeyNotFoundException($"Academic year with ID {id} not found");
        }

        await _academicYearRepository.SetCurrentAsync(id);
        
        var updated = await _academicYearRepository.GetByIdAsync(id);
        var semesters = await _semesterRepository.GetByAcademicYearIdAsync(id);
        return MapToResponse(updated!, semesters.Count);
    }

    private static AcademicYearResponse MapToResponse(AcademicYear academicYear, int semesterCount)
    {
        return new AcademicYearResponse
        {
            Id = academicYear.Id,
            Name = academicYear.Name,
            StartDate = academicYear.StartDate,
            EndDate = academicYear.EndDate,
            IsCurrent = academicYear.IsCurrent,
            CreatedAt = academicYear.CreatedAt,
            SemesterCount = semesterCount
        };
    }
}
