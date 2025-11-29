using Supabase.Gotrue;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using DomainUser = UniversityManagement.Domain.Entities.User;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;
using Client = Supabase.Client;

namespace UniversityManagement.Infrastructure.Services;

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly Client _supabaseClient;
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;

    public SupabaseAuthService(
        Client supabaseClient,
        IUserRepository userRepository,
        IStudentRepository studentRepository)
    {
        _supabaseClient = supabaseClient;
        _userRepository = userRepository;
        _studentRepository = studentRepository;
    }

    public async Task<AuthResponse> SignInAsync(SignInRequest request)
    {
        var session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);
        
        if (session?.User == null)
        {
            throw new UnauthorizedAccessException("Authentication failed");
        }

        var user = await _userRepository.GetByAuthIdAsync(Guid.Parse(session.User.Id));
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found in system");
        }

        var userInfo = await BuildUserInfoAsync(user);

        var expiresAt = session.ExpiresAt();
        var expiresAtOffset = expiresAt != default(DateTime)
            ? new DateTimeOffset(expiresAt, TimeSpan.Zero)
            : DateTimeOffset.UtcNow.AddHours(1);

        return new AuthResponse
        {
            AccessToken = session.AccessToken ?? string.Empty,
            RefreshToken = session.RefreshToken,
            ExpiresAt = expiresAtOffset,
            User = userInfo
        };
    }

    public async Task<AuthResponse> SignUpAsync(SignUpRequest request)
    {
        // Validate role
        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            throw new ArgumentException($"Invalid role: {request.Role}");
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Sign up with Supabase Auth
        var session = await _supabaseClient.Auth.SignUp(request.Email, request.Password);
        
        if (session?.User == null)
        {
            throw new InvalidOperationException("Registration failed");
        }

        // Create user in our database
        var user = new DomainUser
        {
            Id = Guid.NewGuid(),
            SupabaseAuthId = Guid.Parse(session.User.Id),
            Email = request.Email,
            Role = userRole,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var userInfo = await BuildUserInfoAsync(user);

        var expiresAt = session.ExpiresAt();
        var expiresAtOffset = expiresAt != default(DateTime)
            ? new DateTimeOffset(expiresAt, TimeSpan.Zero)
            : DateTimeOffset.UtcNow.AddHours(1);

        return new AuthResponse
        {
            AccessToken = session.AccessToken ?? string.Empty,
            RefreshToken = session.RefreshToken,
            ExpiresAt = expiresAtOffset,
            User = userInfo
        };
    }

    public async Task SignOutAsync()
    {
        await _supabaseClient.Auth.SignOut();
    }

    public async Task<DomainUser?> GetCurrentUserAsync()
    {
        var authUser = _supabaseClient.Auth.CurrentUser;
        if (authUser == null) return null;

        var user = await _userRepository.GetByAuthIdAsync(Guid.Parse(authUser.Id));
        return user;
    }

    public async Task<DomainUser?> GetUserByAuthIdAsync(Guid supabaseAuthId)
    {
        return await _userRepository.GetByAuthIdAsync(supabaseAuthId);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var session = await _supabaseClient.Auth.RefreshSession();
        
        if (session?.User == null)
        {
            throw new UnauthorizedAccessException("Token refresh failed");
        }

        var user = await _userRepository.GetByAuthIdAsync(Guid.Parse(session.User.Id));
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var userInfo = await BuildUserInfoAsync(user);

        var expiresAt = session.ExpiresAt();
        var expiresAtOffset = expiresAt != default(DateTime)
            ? new DateTimeOffset(expiresAt, TimeSpan.Zero)
            : DateTimeOffset.UtcNow.AddHours(1);

        return new AuthResponse
        {
            AccessToken = session.AccessToken ?? string.Empty,
            RefreshToken = session.RefreshToken,
            ExpiresAt = expiresAtOffset,
            User = userInfo
        };
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = _supabaseClient.Auth.CurrentUser;
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        // Supabase handles password change through update user
        var attributes = new UserAttributes
        {
            Password = request.NewPassword
        };

        await _supabaseClient.Auth.Update(attributes);
    }

    public async Task SendPasswordResetAsync(ResetPasswordRequest request)
    {
        await _supabaseClient.Auth.ResetPasswordForEmail(request.Email);
    }

    public async Task UpdateUserRoleAsync(Guid userId, UserRole role)
    {
        await _userRepository.UpdateRoleAsync(userId, role);
    }

    public async Task<UserInfo?> GetUserInfoAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return await BuildUserInfoAsync(user);
    }

    private async Task<UserInfo> BuildUserInfoAsync(DomainUser user)
    {
        var userInfo = new UserInfo
        {
            Id = user.Id,
            SupabaseAuthId = user.SupabaseAuthId,
            Email = user.Email,
            Role = user.Role
        };

        // Load profile information based on role
        if (user.Role == UserRole.Student)
        {
            var students = await _studentRepository.GetAllAsync();
            var student = students.FirstOrDefault(s => s.UserId == user.Id);
            
            if (student != null)
            {
                userInfo.Profile = new ProfileInfo
                {
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    ProfileId = student.Id,
                    Metadata = new Dictionary<string, string>
                    {
                        ["EnrollmentNumber"] = student.EnrollmentNumber,
                        ["Status"] = student.Status.ToString()
                    }
                };
            }
        }
        // Add other role profile loading (Professor, etc.) as needed

        return userInfo;
    }
}
