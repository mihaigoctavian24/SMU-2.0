using Supabase;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly Client _supabaseClient;

    public UserRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<User>()
            .Where(u => u.Id == id)
            .Single();

        return response;
    }

    public async Task<User?> GetByAuthIdAsync(Guid supabaseAuthId)
    {
        var response = await _supabaseClient
            .From<User>()
            .Where(u => u.SupabaseAuthId == supabaseAuthId)
            .Single();

        return response;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var response = await _supabaseClient
            .From<User>()
            .Where(u => u.Email == email)
            .Single();

        return response;
    }

    public async Task<List<User>> GetAllAsync(UserRole? role = null)
    {
        var query = _supabaseClient.From<User>();

        if (role.HasValue)
        {
            var response = await query.Where(u => u.Role == role.Value).Get();
            return response.Models;
        }

        var allResponse = await query.Get();
        return allResponse.Models;
    }

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTimeOffset.UtcNow;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<User>()
            .Insert(user);

        return response.Models.First();
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var response = await _supabaseClient
            .From<User>()
            .Update(user);

        return response.Models.First();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient
            .From<User>()
            .Where(u => u.Id == id)
            .Delete();
    }

    public async Task UpdateRoleAsync(Guid userId, UserRole role)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        user.Role = role;
        await UpdateAsync(user);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        return user != null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await GetByEmailAsync(email);
        return user != null;
    }
}
