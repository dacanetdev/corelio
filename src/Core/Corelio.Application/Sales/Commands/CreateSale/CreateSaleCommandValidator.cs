using FluentValidation;

namespace Corelio.Application.Sales.Commands.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A sale must have at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be zero or greater.");

            item.RuleFor(i => i.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");
        });
    }
}
