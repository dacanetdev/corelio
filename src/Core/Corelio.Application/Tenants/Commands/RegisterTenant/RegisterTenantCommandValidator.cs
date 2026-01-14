using FluentValidation;

namespace Corelio.Application.Tenants.Commands.RegisterTenant;

/// <summary>
/// Validator for the RegisterTenantCommand.
/// </summary>
public class RegisterTenantCommandValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantCommandValidator()
    {
        RuleFor(x => x.TenantName)
            .NotEmpty().WithMessage("Tenant name is required.")
            .MaximumLength(200).WithMessage("Tenant name must not exceed 200 characters.");

        RuleFor(x => x.RFC)
            .NotEmpty().WithMessage("RFC is required.")
            .Matches(@"^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$")
            .WithMessage("RFC must be a valid Mexican tax ID format (e.g., XAXX010101000).");

        RuleFor(x => x.Subdomain)
            .NotEmpty().WithMessage("Subdomain is required.")
            .MaximumLength(50).WithMessage("Subdomain must not exceed 50 characters.")
            .Matches(@"^[a-z0-9-]+$")
            .WithMessage("Subdomain can only contain lowercase letters, numbers, and hyphens.")
            .Must(subdomain => !IsReservedSubdomain(subdomain))
            .WithMessage("Subdomain is reserved and cannot be used.");

        RuleFor(x => x.OwnerEmail)
            .NotEmpty().WithMessage("Owner email is required.")
            .EmailAddress().WithMessage("Owner email must be a valid email address.")
            .MaximumLength(256).WithMessage("Owner email must not exceed 256 characters.");

        RuleFor(x => x.OwnerPassword)
            .NotEmpty().WithMessage("Owner password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.OwnerFirstName)
            .NotEmpty().WithMessage("Owner first name is required.")
            .MaximumLength(100).WithMessage("Owner first name must not exceed 100 characters.");

        RuleFor(x => x.OwnerLastName)
            .NotEmpty().WithMessage("Owner last name is required.")
            .MaximumLength(100).WithMessage("Owner last name must not exceed 100 characters.");
    }

    private static bool IsReservedSubdomain(string subdomain)
    {
        // List of reserved subdomains that cannot be used for tenant registration
        string[] reserved = ["www", "api", "admin", "app", "dashboard", "portal", "mail", "smtp", "ftp", "blog", "docs", "status", "support"];
        return reserved.Contains(subdomain.ToLowerInvariant());
    }
}
