using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByUserIdAsync(Guid userId);
    Task<Guid> CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByCnpAsync(string cnp);
    Task<bool> ExistsByEnrollmentNumberAsync(string enrollmentNumber);
}
