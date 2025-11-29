using Supabase;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using SupabaseClient = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly SupabaseClient _supabaseClient;

    public GroupRepository(SupabaseClient supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Group?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<Group>()
            .Where(g => g.Id == id)
            .Single();
        return response;
    }

    public async Task<List<Group>> GetAllAsync()
    {
        var response = await _supabaseClient
            .From<Group>()
            .Get();
        return response.Models;
    }

    public async Task<List<Group>> GetBySeriesIdAsync(Guid seriesId)
    {
        var response = await _supabaseClient
            .From<Group>()
            .Where(g => g.SeriesId == seriesId)
            .Get();
        return response.Models;
    }

    public async Task<Group> CreateAsync(Group group)
    {
        group.Id = Guid.NewGuid();
        group.CreatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<Group>()
            .Insert(group);
        return response.Models.First();
    }

    public async Task<Group> UpdateAsync(Group group)
    {
        var response = await _supabaseClient
            .From<Group>()
            .Update(group);
        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<Group>()
            .Where(g => g.Id == id)
            .Delete();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var group = await GetByIdAsync(id);
        return group != null;
    }
}
