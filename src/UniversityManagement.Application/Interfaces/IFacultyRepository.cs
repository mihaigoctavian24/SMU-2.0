using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Repository for Faculty entity operations
/// </summary>
public interface IFacultyRepository
{
    /// <summary>
    /// Get faculty by ID
    /// </summary>
    Task<Faculty?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all faculties
    /// </summary>
    Task<List<Faculty>> GetAllAsync();

    /// <summary>
    /// Create a new faculty
    /// </summary>
    Task<Faculty> CreateAsync(Faculty faculty);

    /// <summary>
    /// Update existing faculty
    /// </summary>
    Task<Faculty> UpdateAsync(Faculty faculty);

    /// <summary>
    /// Delete faculty
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Check if faculty exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Search faculties by name
    /// </summary>
    Task<List<Faculty>> SearchByNameAsync(string searchTerm);
}
