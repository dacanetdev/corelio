using FluentValidation;

namespace Corelio.Application.Authentication.Commands.Login;

/// <summary>
/// Validator for the LoginCommand.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.TenantSubdomain)
            .MaximumLength(50).WithMessage("Tenant subdomain must not exceed 50 characters.")
            .Matches(@"^[a-z0-9-]*$").WithMessage("Tenant subdomain can only contain lowercase letters, numbers, and hyphens.")
            .When(x => !string.IsNullOrEmpty(x.TenantSubdomain));
    }
}
