using Corelio.Application.Pricing.Commands.UpdateTenantPricingConfig;
using Corelio.Application.Pricing.Common;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Corelio.Application.Tests.Pricing.Validators;

[Trait("Category", "Unit")]
public class UpdateTenantPricingConfigCommandValidatorTests
{
    private readonly UpdateTenantPricingConfigCommandValidator _validator = new();

    private static UpdateTenantPricingConfigCommand CreateValidCommand() => new(
        DiscountTierCount: 3,
        MarginTierCount: 3,
        DefaultIvaEnabled: true,
        IvaPercentage: 16.00m,
        DiscountTiers:
        [
            new DiscountTierDto(1, "Mayoreo", true),
            new DiscountTierDto(2, "Medio Mayoreo", true),
            new DiscountTierDto(3, "Menudeo", true)
        ],
        MarginTiers:
        [
            new MarginTierDto(1, "Bajo", true),
            new MarginTierDto(2, "Medio", true),
            new MarginTierDto(3, "Alto", true)
        ]);

    [Fact]
    public void ValidCommand_ShouldNotHaveAnyValidationErrors()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DiscountTierCount_BelowOne_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { DiscountTierCount = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DiscountTierCount);
    }

    [Fact]
    public void DiscountTierCount_AboveSix_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { DiscountTierCount = 7 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DiscountTierCount);
    }

    [Fact]
    public void MarginTierCount_BelowOne_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { MarginTierCount = 0 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MarginTierCount);
    }

    [Fact]
    public void MarginTierCount_AboveFive_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { MarginTierCount = 6 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MarginTierCount);
    }

    [Fact]
    public void IvaPercentage_BelowZero_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { IvaPercentage = -1m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.IvaPercentage);
    }

    [Fact]
    public void IvaPercentage_AboveHundred_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { IvaPercentage = 101m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.IvaPercentage);
    }

    [Fact]
    public void DiscountTiers_CountMismatch_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            DiscountTierCount = 2,
            DiscountTiers =
            [
                new DiscountTierDto(1, "Mayoreo", true),
                new DiscountTierDto(2, "Medio Mayoreo", true),
                new DiscountTierDto(3, "Menudeo", true)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DiscountTiers);
    }

    [Fact]
    public void MarginTiers_CountMismatch_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            MarginTierCount = 2,
            MarginTiers =
            [
                new MarginTierDto(1, "Bajo", true),
                new MarginTierDto(2, "Medio", true),
                new MarginTierDto(3, "Alto", true)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MarginTiers);
    }

    [Fact]
    public void DiscountTiers_DuplicateTierNumbers_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            DiscountTierCount = 3,
            DiscountTiers =
            [
                new DiscountTierDto(1, "Mayoreo", true),
                new DiscountTierDto(1, "Duplicado", true),
                new DiscountTierDto(3, "Menudeo", true)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DiscountTiers);
    }

    [Fact]
    public void MarginTiers_DuplicateTierNumbers_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            MarginTierCount = 3,
            MarginTiers =
            [
                new MarginTierDto(1, "Bajo", true),
                new MarginTierDto(1, "Duplicado", true),
                new MarginTierDto(3, "Alto", true)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MarginTiers);
    }

    [Fact]
    public void DiscountTier_EmptyName_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            DiscountTiers =
            [
                new DiscountTierDto(1, "", true),
                new DiscountTierDto(2, "Medio Mayoreo", true),
                new DiscountTierDto(3, "Menudeo", true)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("DiscountTiers[0].TierName");
    }

    [Fact]
    public void DiscountTier_NameExceedsFiftyChars_ShouldHaveValidationError()
    {
        var longName = new string('A', 51);
        var command = CreateValidCommand() with
        {
            DiscountTiers =
            [
                new DiscountTierDto(1, longName, true),
                new DiscountTierDto(2, "Medio Mayoreo", true),
                new DiscountTierDto(3, "Menudeo", true)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("DiscountTiers[0].TierName");
    }
}
