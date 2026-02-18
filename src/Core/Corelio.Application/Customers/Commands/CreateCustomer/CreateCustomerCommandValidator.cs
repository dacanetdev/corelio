using Corelio.Domain.Enums;
using FluentValidation;

namespace Corelio.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Validator for CreateCustomerCommand.
/// </summary>
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.")
            .When(x => x.CustomerType == CustomerType.Individual);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.")
            .When(x => x.CustomerType == CustomerType.Individual);

        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name is required for business customers.")
            .MaximumLength(200).WithMessage("Business name must not exceed 200 characters.")
            .When(x => x.CustomerType == CustomerType.Business);

        RuleFor(x => x.Rfc)
            .Matches(@"^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$")
            .WithMessage("RFC format is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.Rfc));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.CfdiUse)
            .MaximumLength(10).WithMessage("CFDI use code must not exceed 10 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.CfdiUse));

        RuleFor(x => x.TaxRegime)
            .MaximumLength(10).WithMessage("Tax regime must not exceed 10 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TaxRegime));
    }
}
