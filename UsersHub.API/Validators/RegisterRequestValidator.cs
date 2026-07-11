using FluentValidation;
using UsersHub.API.DTOs;

namespace UsersHub.API.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.LastName)
    .NotEmpty()
    .WithMessage("Last Name is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");
    }
}