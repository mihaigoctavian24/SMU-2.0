using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Repository for Professor entity operations
/// </summary>
public interface IProfessorRepository
{
    /// <summary>
    /// Get professor by ID
    /// </summary>
    Task<Professor?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get professor by User ID
    /// </summary>
    Task<Professor?> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Get all professors
    /// </summary>
    Task<List<Professor>> GetAllAsync();

    /// <summary>
    /// Get professors by department
    /// </summary>
    Task<List<Professor>> GetByDepartmentAsync(string department);

    /// <summary>
    /// Get professors by faculty
    /// </summary>
    Task<List<Professor>> GetByFacultyAsync(Guid facultyId);

    /// <summary>
    /// Create a new professor
    /// </summary>
    Task<Professor> CreateAsync(Professor professor);

    /// <summary>
    /// Update existing professor
    /// </summary>
    Task<Professor> UpdateAsync(Professor professor);

    /// <summary>
    /// Delete professor
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Check if professor exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Search professors by name or title
    /// </summary>
    Task<List<Professor>> SearchAsync(string searchTerm);
}
