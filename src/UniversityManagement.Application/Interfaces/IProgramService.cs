using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IProgramService
{
    Task<ProgramResponse?> GetByIdAsync(Guid id);
    Task<List<ProgramResponse>> GetAllAsync();
    Task<List<ProgramResponse>> GetByFacultyIdAsync(Guid facultyId);
    Task<ProgramResponse> CreateAsync(CreateProgramRequest request);
    Task<ProgramResponse> UpdateAsync(Guid id, UpdateProgramRequest request);
    Task DeleteAsync(Guid id);
}
