using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Service for Faculty business logic
/// </summary>
public interface IFacultyService
{
    /// <summary>
    /// Get faculty by ID
    /// </summary>
    Task<FacultyResponse?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all faculties
    /// </summary>
    Task<List<FacultyResponse>> GetAllAsync();

    /// <summary>
    /// Create a new faculty
    /// </summary>
    Task<FacultyResponse> CreateAsync(CreateFacultyRequest request);

    /// <summary>
    /// Update existing faculty
    /// </summary>
    Task<FacultyResponse> UpdateAsync(Guid id, UpdateFacultyRequest request);

    /// <summary>
    /// Delete faculty
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Search faculties by name
    /// </summary>
    Task<List<FacultyResponse>> SearchAsync(string searchTerm);
}
