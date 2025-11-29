using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id);
    Task<List<Group>> GetAllAsync();
    Task<List<Group>> GetBySeriesIdAsync(Guid seriesId);
    Task<Group> CreateAsync(Group group);
    Task<Group> UpdateAsync(Group group);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
