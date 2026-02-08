using FluentValidation;

namespace Corelio.Application.Pricing.Commands.UpdateProductPricing;

/// <summary>
/// Validator for the UpdateProductPricingCommand.
/// </summary>
public class UpdateProductPricingCommandValidator : AbstractValidator<UpdateProductPricingCommand>
{
    public UpdateProductPricingCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.ListPrice)
            .GreaterThanOrEqualTo(0).WithMessage("List price must be 0 or greater.")
            .When(x => x.ListPrice.HasValue);

        RuleFor(x => x.Discounts)
            .NotNull().WithMessage("Discounts list is required.");

        RuleFor(x => x.MarginPrices)
            .NotNull().WithMessage("Margin prices list is required.");

        RuleForEach(x => x.Discounts).ChildRules(discount =>
        {
            discount.RuleFor(d => d.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");
        }).When(x => x.Discounts is not null);

        RuleForEach(x => x.MarginPrices).ChildRules(margin =>
        {
            margin.RuleFor(m => m.MarginPercentage)
                .InclusiveBetween(0, 100).WithMessage("Margin percentage must be between 0 and 100.")
                .When(m => m.MarginPercentage.HasValue);

            margin.RuleFor(m => m.SalePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Sale price must be 0 or greater.")
                .When(m => m.SalePrice.HasValue);

            margin.RuleFor(m => m)
                .Must(m => m.MarginPercentage.HasValue || m.SalePrice.HasValue)
                .WithMessage("Each margin tier must have either a margin percentage or a sale price.");
        }).When(x => x.MarginPrices is not null);
    }
}
