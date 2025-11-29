using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IGroupService
{
    Task<GroupResponse?> GetByIdAsync(Guid id);
    Task<List<GroupResponse>> GetAllAsync();
    Task<List<GroupResponse>> GetBySeriesIdAsync(Guid seriesId);
    Task<GroupResponse> CreateAsync(CreateGroupRequest request);
    Task<GroupResponse> UpdateAsync(Guid id, UpdateGroupRequest request);
    Task DeleteAsync(Guid id);
}
