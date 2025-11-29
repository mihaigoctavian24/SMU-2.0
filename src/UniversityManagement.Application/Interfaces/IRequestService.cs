using UniversityManagement.Domain.Enums;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IRequestService
{
    Task<IEnumerable<RequestResponse>> GetAllAsync();
    Task<IEnumerable<RequestResponse>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<RequestResponse>> GetByStatusAsync(RequestStatus status);
    Task<RequestResponse?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateRequestRequest request);
    Task UpdateStatusAsync(Guid id, UpdateRequestStatusRequest request);
    Task DeleteAsync(Guid id);
}
