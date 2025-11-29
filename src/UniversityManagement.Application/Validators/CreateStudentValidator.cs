using FluentValidation;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.Application.Validators;

public class CreateStudentValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentValidator(IStudentRepository studentRepository)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(async (email, cancellation) => !await studentRepository.ExistsByEmailAsync(email))
            .WithMessage("Email already exists.");

        RuleFor(x => x.Cnp)
            .NotEmpty().WithMessage("CNP is required.")
            .Length(13).WithMessage("CNP must be 13 characters.")
            .MustAsync(async (cnp, cancellation) => !await studentRepository.ExistsByCnpAsync(cnp!))
            .WithMessage("CNP already exists.")
            .When(x => !string.IsNullOrEmpty(x.Cnp));

        RuleFor(x => x.Phone)
            .MaximumLength(20);
    }
}
