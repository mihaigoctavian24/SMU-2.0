using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Middleware;

/// <summary>
/// Global exception handling filter
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

        var statusCode = context.Exception switch
        {
            UnauthorizedAccessException => 401,
            ArgumentNullException => 400,
            ArgumentException => 400,
            KeyNotFoundException => 404,
            InvalidOperationException => 400,
            _ => 500
        };

        var errors = new List<string>();
        
        if (_environment.IsDevelopment())
        {
            // In development, include detailed error information
            errors.Add(context.Exception.Message);
            if (context.Exception.InnerException != null)
            {
                errors.Add($"Inner: {context.Exception.InnerException.Message}");
            }
            errors.Add(context.Exception.StackTrace ?? "No stack trace available");
        }
        else
        {
            // In production, use generic messages
            errors.Add(statusCode switch
            {
                401 => "Unauthorized access",
                404 => "Resource not found",
                400 => "Bad request",
                _ => "An error occurred while processing your request"
            });
        }

        var response = ApiResponse.ErrorResponse(
            message: "An error occurred",
            errors: errors,
            statusCode: statusCode
        );

        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
