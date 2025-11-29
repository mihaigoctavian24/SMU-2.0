using Supabase;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using SupabaseClient = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class ProgramRepository : IProgramRepository
{
    private readonly SupabaseClient _supabaseClient;

    public ProgramRepository(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Program?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<Program>()
            .Where(p => p.Id == id)
            .Single();
        return response;
    }

    public async Task<List<Program>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Program>()
            .Get();
        return response.Models;
    }

    public async Task<List<Program>> GetByFacultyIdAsync(Guid facultyId)
    {
        var response = await _supabaseClient
            .From<Program>()
            .Where(p => p.FacultyId == facultyId)
            .Get();
        return response.Models;
    }

    public async Task<Program> CreateAsync(Program program)
    {
        program.Id = Guid.NewGuid();
        program.CreatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Program>()
            .Insert(program);
        return response.Models.First();
    }

    public async Task<Program> UpdateAsync(Program program)
    {
        var response = await _supabaseClient
            .From<Program>()
            .Update(program);
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Program>()
            .Where(p => p.Id == id)
            .Delete();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var program = await GetByIdAsync(id);
        return program != null;
    }
}
