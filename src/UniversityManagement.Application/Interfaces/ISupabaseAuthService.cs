using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface ISupabaseAuthService
{
    /// <summary>
    /// Sign in a user with email and password
    /// </summary>
    Task<AuthResponse> SignInAsync(SignInRequest request);

    /// <summary>
    /// Sign up a new user
    /// </summary>
    Task<AuthResponse> SignUpAsync(SignUpRequest request);

    /// <summary>
    /// Sign out the current user
    /// </summary>
    Task SignOutAsync();

    /// <summary>
    /// Get the current authenticated user
    /// </summary>
    Task<User?> GetCurrentUserAsync();

    /// <summary>
    /// Get user by Supabase Auth ID
    /// </summary>
    Task<User?> GetUserByAuthIdAsync(Guid supabaseAuthId);

    /// <summary>
    /// Refresh access token
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Change user password
    /// </summary>
    Task ChangePasswordAsync(ChangePasswordRequest request);

    /// <summary>
    /// Send password reset email
    /// </summary>
    Task SendPasswordResetAsync(ResetPasswordRequest request);

    /// <summary>
    /// Update user role (admin only)
    /// </summary>
    Task UpdateUserRoleAsync(Guid userId, UserRole role);

    /// <summary>
    /// Get user info with profile details
    /// </summary>
    Task<UserInfo?> GetUserInfoAsync(Guid userId);
}
