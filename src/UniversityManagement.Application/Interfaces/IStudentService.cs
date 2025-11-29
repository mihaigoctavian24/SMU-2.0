using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IStudentService
{
    Task<StudentResponse?> GetByIdAsync(Guid id);
    Task<StudentResponse?> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<StudentResponse>> GetAllAsync();
    Task<Guid> CreateAsync(CreateStudentRequest request);
    Task UpdateAsync(Guid id, CreateStudentRequest request);
    Task DeleteAsync(Guid id);
}
