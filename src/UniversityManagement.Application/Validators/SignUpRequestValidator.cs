using FluentValidation;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Shared.DTOs.Requests;

namespace UniversityManagement.Application.Validators;

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    public SignUpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(BeAValidRole).WithMessage("Invalid role");
    }

    private bool BeAValidRole(string role)
    {
        return Enum.TryParse<UserRole>(role, true, out _);
    }
}
