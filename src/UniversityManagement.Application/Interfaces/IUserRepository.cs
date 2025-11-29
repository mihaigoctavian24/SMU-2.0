using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Application.Interfaces;

/// <summary>
/// Repository for User entity operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get user by Supabase Auth ID
    /// </summary>
    Task<User?> GetByAuthIdAsync(Guid supabaseAuthId);

    /// <summary>
    /// Get user by email
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Get all users with optional role filter
    /// </summary>
    Task<List<User>> GetAllAsync(UserRole? role = null);

    /// <summary>
    /// Create a new user
    /// </summary>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Update existing user
    /// </summary>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Delete user
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Update user role
    /// </summary>
    Task UpdateRoleAsync(Guid userId, UserRole role);

    /// <summary>
    /// Check if user exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Check if email is already registered
    /// </summary>
    Task<bool> EmailExistsAsync(string email);
}
