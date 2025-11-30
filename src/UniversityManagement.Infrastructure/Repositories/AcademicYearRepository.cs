using Supabase;
using static Supabase.Postgrest.Constants.Ordering;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using SupabaseClient = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class AcademicYearRepository : IAcademicYearRepository
{
    private readonly SupabaseClient _supabaseClient;

    public AcademicYearRepository(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<AcademicYear?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<AcademicYear>()
            .Where(a => a.Id == id)
            .Single();
        return response;
    }

    public async Task<List<AcademicYear>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<AcademicYear>()
            .Order("start_date", Descending)
            .Get();
        return response.Models;
    }

    public async Task<AcademicYear?> GetCurrentAsync()
    {
        var response = await _supabaseClient
            .From<AcademicYear>()
            .Where(a => a.IsCurrent == true)
            .Single();
        return response;
    }

    public async Task<AcademicYear> CreateAsync(AcademicYear academicYear)
    {
        academicYear.Id = Guid.NewGuid();
        academicYear.CreatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<AcademicYear>()
            .Insert(academicYear);
        return response.Models.First();
    }

    public async Task<AcademicYear> UpdateAsync(AcademicYear academicYear)
    {
        var response = await _supabaseClient
            .From<AcademicYear>()
            .Update(academicYear);
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<AcademicYear>()
            .Where(a => a.Id == id)
            .Delete();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var academicYear = await GetByIdAsync(id);
        return academicYear != null;
    }

    public async Task SetCurrentAsync(Guid id)
    {
        // First, set all academic years to non-current
        var allYears = await GetAllAsync();
        foreach (var year in allYears)
        {
            if (year.IsCurrent)
            {
                year.IsCurrent = false;
                await UpdateAsync(year);
            }
        }

        // Then set the specified one as current
        var targetYear = await GetByIdAsync(id);
        if (targetYear != null)
        {
            targetYear.IsCurrent = true;
            await UpdateAsync(targetYear);
        }
    }
}
