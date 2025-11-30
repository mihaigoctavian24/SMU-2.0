using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface IAcademicYearRepository
{
    Task<AcademicYear?> GetByIdAsync(Guid id);
    Task<List<AcademicYear>> GetAllAsync();
    Task<AcademicYear?> GetCurrentAsync();
    Task<AcademicYear> CreateAsync(AcademicYear academicYear);
    Task<AcademicYear> UpdateAsync(AcademicYear academicYear);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task SetCurrentAsync(Guid id);
}
