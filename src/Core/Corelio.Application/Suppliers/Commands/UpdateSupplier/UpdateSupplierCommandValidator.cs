using FluentValidation;

namespace Corelio.Application.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Supplier ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters.");

        RuleFor(x => x.Rfc)
            .Matches(@"^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$")
            .WithMessage("RFC format is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.Rfc));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PaymentTermsDays)
            .GreaterThanOrEqualTo(0).WithMessage("Payment terms days must be 0 or greater.")
            .LessThanOrEqualTo(365).WithMessage("Payment terms days must not exceed 365.");
    }
}
