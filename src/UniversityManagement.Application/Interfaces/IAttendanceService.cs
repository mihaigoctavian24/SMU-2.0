using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IAttendanceService
{
    Task<IEnumerable<AttendanceResponse>> GetAllAsync();
    Task<IEnumerable<AttendanceResponse>> GetByStudentIdAsync(Guid studentId);
    Task<AttendanceResponse?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateAttendanceRequest request);
    Task UpdateAsync(Guid id, CreateAttendanceRequest request);
    Task DeleteAsync(Guid id);
}
