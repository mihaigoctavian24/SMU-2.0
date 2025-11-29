using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Shared.DTOs.Responses;

/// <summary>
/// Authentication response containing user info and tokens
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public UserInfo User { get; set; } = null!;
}

/// <summary>
/// User information DTO
/// </summary>
public class UserInfo
{
    /// <summary>
    /// User unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Supabase Auth ID
    /// </summary>
    public Guid SupabaseAuthId { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User role
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Role display name
    /// </summary>
    public string RoleDisplayName => Role.ToString();

    /// <summary>
    /// Additional profile information based on role
    /// </summary>
    public ProfileInfo? Profile { get; set; }
}

/// <summary>
/// Additional profile information
/// </summary>
public class ProfileInfo
{
    /// <summary>
    /// First name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Profile-specific ID (student_id, professor_id, etc.)
    /// </summary>
    public Guid? ProfileId { get; set; }

    /// <summary>
    /// Additional metadata (enrollment number, title, department, etc.)
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
