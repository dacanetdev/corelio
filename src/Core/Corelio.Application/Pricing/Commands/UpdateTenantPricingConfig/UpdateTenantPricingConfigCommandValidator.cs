using FluentValidation;

namespace Corelio.Application.Pricing.Commands.UpdateTenantPricingConfig;

/// <summary>
/// Validator for the UpdateTenantPricingConfigCommand.
/// </summary>
public class UpdateTenantPricingConfigCommandValidator : AbstractValidator<UpdateTenantPricingConfigCommand>
{
    public UpdateTenantPricingConfigCommandValidator()
    {
        RuleFor(x => x.DiscountTierCount)
            .InclusiveBetween(1, 6).WithMessage("Discount tier count must be between 1 and 6.");

        RuleFor(x => x.MarginTierCount)
            .InclusiveBetween(1, 5).WithMessage("Margin tier count must be between 1 and 5.");

        RuleFor(x => x.IvaPercentage)
            .InclusiveBetween(0, 100).WithMessage("IVA percentage must be between 0 and 100.");

        RuleFor(x => x.DiscountTiers)
            .NotNull().WithMessage("Discount tiers are required.")
            .Must((cmd, tiers) => tiers.Count == cmd.DiscountTierCount)
            .WithMessage("Number of discount tiers must match DiscountTierCount.");

        RuleFor(x => x.MarginTiers)
            .NotNull().WithMessage("Margin tiers are required.")
            .Must((cmd, tiers) => tiers.Count == cmd.MarginTierCount)
            .WithMessage("Number of margin tiers must match MarginTierCount.");

        RuleFor(x => x.DiscountTiers)
            .Must(tiers => tiers.Select(t => t.TierNumber).Distinct().Count() == tiers.Count)
            .WithMessage("Discount tier numbers must be unique.")
            .When(x => x.DiscountTiers is not null && x.DiscountTiers.Count > 0);

        RuleFor(x => x.MarginTiers)
            .Must(tiers => tiers.Select(t => t.TierNumber).Distinct().Count() == tiers.Count)
            .WithMessage("Margin tier numbers must be unique.")
            .When(x => x.MarginTiers is not null && x.MarginTiers.Count > 0);

        RuleForEach(x => x.DiscountTiers).ChildRules(tier =>
        {
            tier.RuleFor(t => t.TierName)
                .NotEmpty().WithMessage("Tier name is required.")
                .MaximumLength(50).WithMessage("Tier name must not exceed 50 characters.");
        }).When(x => x.DiscountTiers is not null);

        RuleForEach(x => x.MarginTiers).ChildRules(tier =>
        {
            tier.RuleFor(t => t.TierName)
                .NotEmpty().WithMessage("Tier name is required.")
                .MaximumLength(50).WithMessage("Tier name must not exceed 50 characters.");
        }).When(x => x.MarginTiers is not null);
    }
}
