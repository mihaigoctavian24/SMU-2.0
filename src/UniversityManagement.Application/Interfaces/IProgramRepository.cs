using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface IProgramRepository
{
    Task<Program?> GetByIdAsync(Guid id);
    Task<List<Program>> GetAllAsync();
    Task<List<Program>> GetByFacultyIdAsync(Guid facultyId);
    Task<Program> CreateAsync(Program program);
    Task<Program> UpdateAsync(Program program);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
