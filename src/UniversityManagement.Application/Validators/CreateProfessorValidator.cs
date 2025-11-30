using FluentValidation;
using UniversityManagement.Shared.DTOs.Requests;

namespace UniversityManagement.Application.Validators;

/// <summary>
/// Validator for CreateProfessorRequest
/// </summary>
public class CreateProfessorValidator : AbstractValidator<CreateProfessorRequest>
{
    public CreateProfessorValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.Title)
            .MaximumLength(50)
            .WithMessage("Title must not exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Department)
            .MaximumLength(200)
            .WithMessage("Department must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Department));

        RuleFor(x => x.Phone)
            .Matches(@"^[0-9\s\-\+\(\)]+$")
            .WithMessage("Phone number must contain only digits, spaces, and common phone symbols")
            .MinimumLength(10)
            .WithMessage("Phone number must be at least 10 characters")
            .MaximumLength(20)
            .WithMessage("Phone number must not exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
