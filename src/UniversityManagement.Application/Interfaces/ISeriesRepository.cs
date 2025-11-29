using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface ISeriesRepository
{
    Task<Series?> GetByIdAsync(Guid id);
    Task<List<Series>> GetAllAsync();
    Task<List<Series>> GetByProgramIdAsync(Guid programId);
    Task<Series> CreateAsync(Series series);
    Task<Series> UpdateAsync(Series series);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
