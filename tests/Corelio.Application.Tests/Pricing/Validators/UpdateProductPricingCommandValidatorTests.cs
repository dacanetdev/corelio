using Corelio.Application.Pricing.Commands.UpdateProductPricing;
using Corelio.Application.Pricing.Common;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Corelio.Application.Tests.Pricing.Validators;

[Trait("Category", "Unit")]
public class UpdateProductPricingCommandValidatorTests
{
    private readonly UpdateProductPricingCommandValidator _validator = new();

    private static UpdateProductPricingCommand CreateValidCommand() => new(
        ProductId: Guid.NewGuid(),
        ListPrice: 150.00m,
        IvaEnabled: true,
        Discounts:
        [
            new UpdateProductDiscountDto(1, 10.0m),
            new UpdateProductDiscountDto(2, 5.0m)
        ],
        MarginPrices:
        [
            new UpdateProductMarginPriceDto(1, 30.0m, null),
            new UpdateProductMarginPriceDto(2, null, 200.00m)
        ]);

    [Fact]
    public void ValidCommand_ShouldNotHaveAnyValidationErrors()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyProductId_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { ProductId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void NegativeListPrice_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { ListPrice = -1.00m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ListPrice);
    }

    [Fact]
    public void NullDiscounts_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { Discounts = null! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Discounts);
    }

    [Fact]
    public void NullMarginPrices_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with { MarginPrices = null! };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MarginPrices);
    }

    [Fact]
    public void DiscountPercentage_BelowZero_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            Discounts =
            [
                new UpdateProductDiscountDto(1, -5.0m),
                new UpdateProductDiscountDto(2, 5.0m)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Discounts[0].DiscountPercentage");
    }

    [Fact]
    public void DiscountPercentage_AboveHundred_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            Discounts =
            [
                new UpdateProductDiscountDto(1, 101.0m),
                new UpdateProductDiscountDto(2, 5.0m)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Discounts[0].DiscountPercentage");
    }

    [Fact]
    public void MarginPercentage_AboveHundred_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            MarginPrices =
            [
                new UpdateProductMarginPriceDto(1, 150.0m, null),
                new UpdateProductMarginPriceDto(2, null, 200.00m)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("MarginPrices[0].MarginPercentage");
    }

    [Fact]
    public void NegativeSalePrice_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            MarginPrices =
            [
                new UpdateProductMarginPriceDto(1, 30.0m, null),
                new UpdateProductMarginPriceDto(2, null, -50.00m)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("MarginPrices[1].SalePrice");
    }

    [Fact]
    public void MarginPrice_WithNeitherMarginPercentageNorSalePrice_ShouldHaveValidationError()
    {
        var command = CreateValidCommand() with
        {
            MarginPrices =
            [
                new UpdateProductMarginPriceDto(1, null, null),
                new UpdateProductMarginPriceDto(2, null, 200.00m)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("MarginPrices[0]");
    }
}
