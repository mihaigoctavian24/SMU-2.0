using Supabase.Gotrue;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using DomainUser = UniversityManagement.Domain.Entities.User;
using UniversityManagement.Domain.Enums;
using Client = Supabase.Client;

namespace UniversityManagement.Infrastructure.Services;

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly Client _supabaseClient;

    public SupabaseAuthService(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<string> SignInAsync(string email, string password)
    {
        var session = await _supabaseClient.Auth.SignIn(email, password);
        return session?.AccessToken ?? throw new Exception("Authentication failed");
    }

    public async Task<string> SignUpAsync(string email, string password)
    {
        var session = await _supabaseClient.Auth.SignUp(email, password);
        return session?.User?.Id ?? throw new Exception("Registration failed");
    }

    public async Task SignOutAsync()
    {
        await _supabaseClient.Auth.SignOut();
    }

    public async Task<DomainUser?> GetCurrentUserAsync()
    {
        var user = _supabaseClient.Auth.CurrentUser;
        if (user == null) return null;

        // Map Supabase User to Domain User
        // Note: In a real app, we would fetch the User profile from our 'users' table
        // For now, we return a basic mapping based on metadata if available
        
        return new DomainUser
        {
            SupabaseAuthId = Guid.Parse(user.Id),
            Email = user.Email ?? string.Empty,
            Role = UserRole.Student, // Defaulting to Student for now
            CreatedAt = new DateTimeOffset(user.CreatedAt),
            UpdatedAt = user.UpdatedAt.HasValue ? new DateTimeOffset(user.UpdatedAt.Value) : DateTimeOffset.UtcNow
        };
    }
}
