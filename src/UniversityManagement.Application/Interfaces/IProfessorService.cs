using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Service for Professor business logic
/// </summary>
public interface IProfessorService
{
    /// <summary>
    /// Get professor by ID
    /// </summary>
    Task<ProfessorResponse?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get professor by User ID
    /// </summary>
    Task<ProfessorResponse?> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Get all professors
    /// </summary>
    Task<List<ProfessorResponse>> GetAllAsync();

    /// <summary>
    /// Get professors by department
    /// </summary>
    Task<List<ProfessorResponse>> GetByDepartmentAsync(string department);

    /// <summary>
    /// Get professors by faculty
    /// </summary>
    Task<List<ProfessorResponse>> GetByFacultyAsync(Guid facultyId);

    /// <summary>
    /// Create a new professor
    /// </summary>
    Task<ProfessorResponse> CreateAsync(CreateProfessorRequest request);

    /// <summary>
    /// Update existing professor
    /// </summary>
    Task<ProfessorResponse> UpdateAsync(Guid id, UpdateProfessorRequest request);

    /// <summary>
    /// Delete professor
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Search professors by name or title
    /// </summary>
    Task<List<ProfessorResponse>> SearchAsync(string searchTerm);
}
