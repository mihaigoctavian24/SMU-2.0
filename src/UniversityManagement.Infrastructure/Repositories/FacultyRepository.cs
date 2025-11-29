using Supabase;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using SupabaseClient = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class FacultyRepository : IFacultyRepository
{
    private readonly SupabaseClient _supabaseClient;

    public FacultyRepository(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Faculty?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<Faculty>()
            .Where(f => f.Id == id)
            .Single();

        return response;
    }

    public async Task<List<Faculty>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Faculty>()
            .Get();

        return response.Models;
    }

    public async Task<Faculty> CreateAsync(Faculty faculty)
    {
        faculty.Id = Guid.NewGuid();
        faculty.CreatedAt = DateTimeOffset.UtcNow;
        faculty.UpdatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Faculty>()
            .Insert(faculty);

        return response.Models.First();
    }

    public async Task<Faculty> UpdateAsync(Faculty faculty)
    {
        faculty.UpdatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Faculty>()
            .Update(faculty);

        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Faculty>()
            .Where(f => f.Id == id)
            .Delete();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var faculty = await GetByIdAsync(id);
        return faculty != null;
    }

    public async Task<List<Faculty>> SearchByNameAsync(string searchTerm)
    {
        // Use simple text search since Postgrest constants may not be accessible
        var allFaculties = await GetAllAsync();
        return allFaculties
            .Where(f => f.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
