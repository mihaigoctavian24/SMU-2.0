using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly IAcademicYearRepository _academicYearRepository;

    public SemesterService(
        ISemesterRepository semesterRepository,
        IAcademicYearRepository academicYearRepository)
    {
        _semesterRepository = semesterRepository;
        _academicYearRepository = academicYearRepository;
    }

    public async Task<SemesterResponse?> GetByIdAsync(Guid id)
    {
        var semester = await _semesterRepository.GetByIdAsync(id);
        if (semester == null) return null;

        var academicYear = await _academicYearRepository.GetByIdAsync(semester.AcademicYearId);
        return MapToResponse(semester, academicYear?.Name ?? "Unknown");
    }

    public async Task<List<SemesterResponse>> GetAllAsync()
    {
        var semesters = await _semesterRepository.GetAllAsync();
        var responses = new List<SemesterResponse>();

        foreach (var semester in semesters)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(semester.AcademicYearId);
            responses.Add(MapToResponse(semester, academicYear?.Name ?? "Unknown"));
        }

        return responses;
    }

    public async Task<List<SemesterResponse>> GetByAcademicYearIdAsync(Guid academicYearId)
    {
        var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYearId);
        var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
        var academicYearName = academicYear?.Name ?? "Unknown";

        return semesters.Select(s => MapToResponse(s, academicYearName)).ToList();
    }

    public async Task<SemesterResponse> CreateAsync(CreateSemesterRequest request)
    {
        var academicYearExists = await _academicYearRepository.ExistsAsync(request.AcademicYearId);
        if (!academicYearExists)
        {
            throw new KeyNotFoundException($"Academic year with ID {request.AcademicYearId} not found");
        }

        var semester = new Semester
        {
            AcademicYearId = request.AcademicYearId,
            Number = request.Number,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var created = await _semesterRepository.CreateAsync(semester);
        var academicYear = await _academicYearRepository.GetByIdAsync(created.AcademicYearId);
        return MapToResponse(created, academicYear?.Name ?? "Unknown");
    }

    public async Task<SemesterResponse> UpdateAsync(Guid id, UpdateSemesterRequest request)
    {
        var semester = await _semesterRepository.GetByIdAsync(id);
        if (semester == null)
        {
            throw new KeyNotFoundException($"Semester with ID {id} not found");
        }

        var academicYearExists = await _academicYearRepository.ExistsAsync(request.AcademicYearId);
        if (!academicYearExists)
        {
            throw new KeyNotFoundException($"Academic year with ID {request.AcademicYearId} not found");
        }

        semester.AcademicYearId = request.AcademicYearId;
        semester.Number = request.Number;
        semester.StartDate = request.StartDate;
        semester.EndDate = request.EndDate;

        var updated = await _semesterRepository.UpdateAsync(semester);
        var academicYear = await _academicYearRepository.GetByIdAsync(updated.AcademicYearId);
        return MapToResponse(updated, academicYear?.Name ?? "Unknown");
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _semesterRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Semester with ID {id} not found");
        }

        await _semesterRepository.DeleteAsync(id);
    }

    private static SemesterResponse MapToResponse(Semester semester, string academicYearName)
    {
        return new SemesterResponse
        {
            Id = semester.Id,
            AcademicYearId = semester.AcademicYearId,
            AcademicYearName = academicYearName,
            Number = semester.Number,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            CreatedAt = semester.CreatedAt
        };
    }
}
