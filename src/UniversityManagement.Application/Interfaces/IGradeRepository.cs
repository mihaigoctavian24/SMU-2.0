using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(Guid id);
    Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Grade>> GetAllAsync();
    Task<Guid> CreateAsync(Grade grade);
    Task UpdateAsync(Grade grade);
    Task DeleteAsync(Guid id);
}
