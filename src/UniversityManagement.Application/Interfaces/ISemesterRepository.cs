using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface ISemesterRepository
{
    Task<Semester?> GetByIdAsync(Guid id);
    Task<List<Semester>> GetAllAsync();
    Task<List<Semester>> GetByAcademicYearIdAsync(Guid academicYearId);
    Task<Semester> CreateAsync(Semester semester);
    Task<Semester> UpdateAsync(Semester semester);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
