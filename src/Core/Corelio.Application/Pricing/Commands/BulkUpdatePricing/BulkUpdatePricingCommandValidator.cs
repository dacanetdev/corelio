using Corelio.Application.Common.Enums;
using FluentValidation;

namespace Corelio.Application.Pricing.Commands.BulkUpdatePricing;

/// <summary>
/// Validator for the BulkUpdatePricingCommand.
/// </summary>
public class BulkUpdatePricingCommandValidator : AbstractValidator<BulkUpdatePricingCommand>
{
    public BulkUpdatePricingCommandValidator()
    {
        RuleFor(x => x.ProductIds)
            .NotEmpty().WithMessage("At least one product ID is required.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Value must be greater than 0.");

        RuleFor(x => x.UpdateType)
            .IsInEnum().WithMessage("Invalid update type.");

        RuleFor(x => x.TierNumber)
            .NotNull().WithMessage("Tier number is required for SetNewMargin update type.")
            .When(x => x.UpdateType == PricingUpdateType.SetNewMargin);

        RuleFor(x => x.TierNumber)
            .InclusiveBetween(1, 5).WithMessage("Tier number must be between 1 and 5.")
            .When(x => x.TierNumber.HasValue);

        RuleFor(x => x.Value)
            .LessThan(100).WithMessage("Margin percentage must be less than 100.")
            .When(x => x.UpdateType == PricingUpdateType.SetNewMargin);
    }
}
