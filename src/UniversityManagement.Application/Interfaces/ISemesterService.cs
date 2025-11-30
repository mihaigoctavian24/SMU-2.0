using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface ISemesterService
{
    Task<SemesterResponse?> GetByIdAsync(Guid id);
    Task<List<SemesterResponse>> GetAllAsync();
    Task<List<SemesterResponse>> GetByAcademicYearIdAsync(Guid academicYearId);
    Task<SemesterResponse> CreateAsync(CreateSemesterRequest request);
    Task<SemesterResponse> UpdateAsync(Guid id, UpdateSemesterRequest request);
    Task DeleteAsync(Guid id);
}
