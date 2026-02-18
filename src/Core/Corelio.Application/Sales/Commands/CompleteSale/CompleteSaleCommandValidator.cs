using FluentValidation;

namespace Corelio.Application.Sales.Commands.CompleteSale;

/// <summary>
/// Validator for CompleteSaleCommand.
/// </summary>
public class CompleteSaleCommandValidator : AbstractValidator<CompleteSaleCommand>
{
    public CompleteSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID is required.");

        RuleFor(x => x.Payments)
            .NotEmpty().WithMessage("At least one payment is required.");

        RuleForEach(x => x.Payments).ChildRules(p =>
        {
            p.RuleFor(i => i.Amount)
                .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");
        });
    }
}
