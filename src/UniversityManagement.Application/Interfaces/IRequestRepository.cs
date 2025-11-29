using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Application.Interfaces;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(Guid id);
    Task<IEnumerable<Request>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Request>> GetAllAsync();
    Task<IEnumerable<Request>> GetByStatusAsync(RequestStatus status);
    Task<Guid> CreateAsync(Request request);
    Task UpdateAsync(Request request);
    Task DeleteAsync(Guid id);
}
