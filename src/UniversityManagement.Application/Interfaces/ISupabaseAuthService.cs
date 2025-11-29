using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface ISupabaseAuthService
{
    Task<string> SignInAsync(string email, string password);
    Task<string> SignUpAsync(string email, string password);
    Task SignOutAsync();
    Task<User?> GetCurrentUserAsync();
}
