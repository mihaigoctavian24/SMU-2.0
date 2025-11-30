using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IAcademicYearService
{
    Task<AcademicYearResponse?> GetByIdAsync(Guid id);
    Task<List<AcademicYearResponse>> GetAllAsync();
    Task<AcademicYearResponse?> GetCurrentAsync();
    Task<AcademicYearResponse> CreateAsync(CreateAcademicYearRequest request);
    Task<AcademicYearResponse> UpdateAsync(Guid id, UpdateAcademicYearRequest request);
    Task DeleteAsync(Guid id);
    Task<AcademicYearResponse> SetCurrentAsync(Guid id);
}
