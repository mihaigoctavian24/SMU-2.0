using Supabase;
using static Supabase.Postgrest.Constants.Ordering;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using SupabaseClient = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class SemesterRepository : ISemesterRepository
{
    private readonly SupabaseClient _supabaseClient;

    public SemesterRepository(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Semester?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<Semester>()
            .Where(s => s.Id == id)
            .Single();
        return response;
    }

    public async Task<List<Semester>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Semester>()
            .Order("start_date", Ascending)
            .Get();
        return response.Models;
    }

    public async Task<List<Semester>> GetByAcademicYearIdAsync(Guid academicYearId)
    {
        var response = await _supabaseClient
            .From<Semester>()
            .Where(s => s.AcademicYearId == academicYearId)
            .Order("number", Ascending)
            .Get();
        return response.Models;
    }

    public async Task<Semester> CreateAsync(Semester semester)
    {
        semester.Id = Guid.NewGuid();
        semester.CreatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Semester>()
            .Insert(semester);
        return response.Models.First();
    }

    public async Task<Semester> UpdateAsync(Semester semester)
    {
        var response = await _supabaseClient
            .From<Semester>()
            .Update(semester);
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Semester>()
            .Where(s => s.Id == id)
            .Delete();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var semester = await GetByIdAsync(id);
        return semester != null;
    }
}
