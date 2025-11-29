using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IGradeService
{
    Task<IEnumerable<GradeResponse>> GetAllAsync();
    Task<IEnumerable<GradeResponse>> GetByStudentIdAsync(Guid studentId);
    Task<GradeResponse?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateGradeRequest request);
    Task UpdateAsync(Guid id, CreateGradeRequest request);
    Task DeleteAsync(Guid id);
}
