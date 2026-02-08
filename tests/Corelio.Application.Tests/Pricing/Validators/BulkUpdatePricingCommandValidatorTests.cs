using Corelio.Application.Common.Enums;
using Corelio.Application.Pricing.Commands.BulkUpdatePricing;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Corelio.Application.Tests.Pricing.Validators;

[Trait("Category", "Unit")]
public class BulkUpdatePricingCommandValidatorTests
{
    private readonly BulkUpdatePricingCommandValidator _validator = new();

    private static BulkUpdatePricingCommand CreateValidCommand() => new(
        ProductIds: [Guid.NewGuid(), Guid.NewGuid()],
        UpdateType: PricingUpdateType.PercentageIncrease,
        Value: 10.0m);

    [Fact]
    public void ValidCommand_ShouldNotHaveAnyValidationErrors()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyProductIds_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { ProductIds = [] };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProductIds);
    }

    [Fact]
    public void Value_ZeroOrLess_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { Value = 0m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Value);
    }

    [Fact]
    public void InvalidUpdateType_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { UpdateType = (PricingUpdateType)99 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateType);
    }

    [Fact]
    public void SetNewMargin_WithoutTierNumber_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            UpdateType = PricingUpdateType.SetNewMargin,
            Value = 25.0m,
            TierNumber = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TierNumber);
    }

    [Fact]
    public void TierNumber_BelowOne_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            UpdateType = PricingUpdateType.SetNewMargin,
            Value = 25.0m,
            TierNumber = 0
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TierNumber);
    }

    [Fact]
    public void TierNumber_AboveFive_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            UpdateType = PricingUpdateType.SetNewMargin,
            Value = 25.0m,
            TierNumber = 6
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TierNumber);
    }

    [Fact]
    public void SetNewMargin_WithValueAboveOrEqualHundred_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            UpdateType = PricingUpdateType.SetNewMargin,
            Value = 100.0m,
            TierNumber = 3
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Value);
    }
}
