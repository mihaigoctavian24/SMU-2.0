using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Middleware;

/// <summary>
/// Filter to handle FluentValidation exceptions globally
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse.ErrorResponse(
                message: "Validation failed",
                errors: errors,
                statusCode: 400
            );

            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}
