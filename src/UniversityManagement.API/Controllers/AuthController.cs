using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

/// <summary>
/// Authentication and user management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISupabaseAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ISupabaseAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Sign in with email and password
    /// </summary>
    [HttpPost("signin")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> SignIn([FromBody] SignInRequest request)
    {
        try
        {
            var response = await _authService.SignInAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Sign in successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Sign in failed for {Email}", request.Email);
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("Invalid credentials", statusCode: 401));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign in for {Email}", request.Email);
            return StatusCode(500, ApiResponse<AuthResponse>.ErrorResponse("An error occurred during sign in", statusCode: 500));
        }
    }

    /// <summary>
    /// Sign up a new user
    /// </summary>
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> SignUp([FromBody] SignUpRequest request)
    {
        try
        {
            var response = await _authService.SignUpAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Registration successful", 201));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Sign up failed for {Email}", request.Email);
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse(ex.Message, statusCode: 400));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid sign up data for {Email}", request.Email);
            return BadRequest(ApiResponse<AuthResponse>.ErrorResponse(ex.Message, statusCode: 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign up for {Email}", request.Email);
            return StatusCode(500, ApiResponse<AuthResponse>.ErrorResponse("An error occurred during registration", statusCode: 500));
        }
    }

    /// <summary>
    /// Sign out current user
    /// </summary>
    [HttpPost("signout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> SignOut()
    {
        try
        {
            await _authService.SignOutAsync();
            return Ok(ApiResponse.SuccessResponse("Sign out successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign out");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred during sign out", statusCode: 500));
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetCurrentUser()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(ApiResponse<UserInfo>.ErrorResponse("User not authenticated", statusCode: 401));
            }

            var userInfo = await _authService.GetUserInfoAsync(user.Id);
            return Ok(ApiResponse<UserInfo>.SuccessResponse(userInfo!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, ApiResponse<UserInfo>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Token refreshed"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Token refresh failed");
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("Invalid refresh token", statusCode: 401));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, ApiResponse<AuthResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            await _authService.ChangePasswordAsync(request);
            return Ok(ApiResponse.SuccessResponse("Password changed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Password change failed");
            return Unauthorized(ApiResponse.ErrorResponse("Authentication failed", statusCode: 401));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            await _authService.SendPasswordResetAsync(request);
            return Ok(ApiResponse.SuccessResponse("Password reset email sent"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset for {Email}", request.Email);
            // Don't reveal if email exists or not for security
            return Ok(ApiResponse.SuccessResponse("If the email exists, a reset link has been sent"));
        }
    }

    /// <summary>
    /// Update user role (admin only)
    /// </summary>
    [HttpPut("users/{userId}/role")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> UpdateUserRole(Guid userId, [FromBody] UpdateUserRoleRequest request)
    {
        try
        {
            if (!Enum.TryParse<Domain.Enums.UserRole>(request.Role, true, out var role))
            {
                return BadRequest(ApiResponse.ErrorResponse("Invalid role", statusCode: 400));
            }

            await _authService.UpdateUserRoleAsync(userId, role);
            return Ok(ApiResponse.SuccessResponse("User role updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User {UserId} not found", userId);
            return NotFound(ApiResponse.ErrorResponse("User not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role for {UserId}", userId);
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }
}
