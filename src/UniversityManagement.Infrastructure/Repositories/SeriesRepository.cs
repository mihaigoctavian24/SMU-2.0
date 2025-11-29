using Supabase;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using SupabaseClient = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly SupabaseClient _supabaseClient;

    public SeriesRepository(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Series?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<Series>()
            .Where(s => s.Id == id)
            .Single();
        return response;
    }

    public async Task<List<Series>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Series>()
            .Get();
        return response.Models;
    }

    public async Task<List<Series>> GetByProgramIdAsync(Guid programId)
    {
        var response = await _supabaseClient
            .From<Series>()
            .Where(s => s.ProgramId == programId)
            .Get();
        return response.Models;
    }

    public async Task<Series> CreateAsync(Series series)
    {
        series.Id = Guid.NewGuid();
        series.CreatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Series>()
            .Insert(series);
        return response.Models.First();
    }

    public async Task<Series> UpdateAsync(Series series)
    {
        var response = await _supabaseClient
            .From<Series>()
            .Update(series);
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Series>()
            .Where(s => s.Id == id)
            .Delete();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var series = await GetByIdAsync(id);
        return series != null;
    }
}
