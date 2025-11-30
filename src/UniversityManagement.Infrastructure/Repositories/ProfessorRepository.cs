using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Professor entity
/// </summary>
public class ProfessorRepository : IProfessorRepository
{
    private readonly Supabase.Client _supabaseClient;

    public ProfessorRepository(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Professor?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<Professor>()
            .Where(p => p.Id == id)
            .Single();

        return response;
    }

    public async Task<Professor?> GetByUserIdAsync(Guid userId)
    {
        var response = await _supabaseClient
            .From<Professor>()
            .Where(p => p.UserId == userId)
            .Single();

        return response;
    }

    public async Task<List<Professor>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Professor>()
            .Get();

        return response.Models;
    }

    public async Task<List<Professor>> GetByDepartmentAsync(string department)
    {
        var allProfessors = await GetAllAsync();
        return allProfessors
            .Where(p => p.Department != null && 
                       p.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<List<Professor>> GetByFacultyAsync(Guid facultyId)
    {
        // For now, we'll need to join with courses or use a different approach
        // Since professors don't have direct faculty relationship in current schema
        // This is a placeholder that returns empty list
        // TODO: Implement when professor-faculty relationship is established
        return new List<Professor>();
    }

    public async Task<Professor> CreateAsync(Professor professor)
    {
        professor.Id = Guid.NewGuid();
        professor.CreatedAt = DateTimeOffset.UtcNow;
        professor.UpdatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Professor>()
            .Insert(professor);

        return response.Models.First();
    }

    public async Task<Professor> UpdateAsync(Professor professor)
    {
        professor.UpdatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Professor>()
            .Update(professor);

        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Professor>()
            .Where(p => p.Id == id)
            .Delete();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var professor = await GetByIdAsync(id);
        return professor != null;
    }

    public async Task<List<Professor>> SearchAsync(string searchTerm)
    {
        var allProfessors = await GetAllAsync();
        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        return allProfessors
            .Where(p => 
                p.FirstName.ToLowerInvariant().Contains(lowerSearchTerm) ||
                p.LastName.ToLowerInvariant().Contains(lowerSearchTerm) ||
                (p.Title != null && p.Title.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                (p.Department != null && p.Department.ToLowerInvariant().Contains(lowerSearchTerm)))
            .ToList();
    }
}
