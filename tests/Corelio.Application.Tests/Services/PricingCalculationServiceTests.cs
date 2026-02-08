using Corelio.Application.Services;
using FluentAssertions;

namespace Corelio.Application.Tests.Services;

[Trait("Category", "Unit")]
public class PricingCalculationServiceTests
{
    // CalculateNetCost tests

    [Fact]
    public void CalculateNetCost_WithSingleDiscount_AppliesCorrectly()
    {
        // Arrange
        var listPrice = 1000m;
        var discounts = new List<decimal> { 10 };

        // Act
        var result = PricingCalculationService.CalculateNetCost(listPrice, discounts);

        // Assert
        result.Should().Be(900.00m);
    }

    [Fact]
    public void CalculateNetCost_WithCascadingDiscounts_AppliesSequentially()
    {
        // Arrange: 1000 × 0.90 × 0.95 × 0.98 = 837.90
        var listPrice = 1000m;
        var discounts = new List<decimal> { 10, 5, 2 };

        // Act
        var result = PricingCalculationService.CalculateNetCost(listPrice, discounts);

        // Assert
        result.Should().Be(837.90m);
    }

    [Fact]
    public void CalculateNetCost_WithEmptyDiscounts_ReturnsListPrice()
    {
        // Arrange
        var listPrice = 500m;
        var discounts = new List<decimal>();

        // Act
        var result = PricingCalculationService.CalculateNetCost(listPrice, discounts);

        // Assert
        result.Should().Be(500.00m);
    }

    [Fact]
    public void CalculateNetCost_WithZeroDiscount_ReturnsListPrice()
    {
        // Arrange
        var listPrice = 750m;
        var discounts = new List<decimal> { 0 };

        // Act
        var result = PricingCalculationService.CalculateNetCost(listPrice, discounts);

        // Assert
        result.Should().Be(750.00m);
    }

    [Fact]
    public void CalculateNetCost_With100PercentDiscount_ReturnsZero()
    {
        // Arrange
        var listPrice = 1000m;
        var discounts = new List<decimal> { 100 };

        // Act
        var result = PricingCalculationService.CalculateNetCost(listPrice, discounts);

        // Assert
        result.Should().Be(0.00m);
    }

    [Fact]
    public void CalculateNetCost_RoundsToTwoDecimalPlaces()
    {
        // Arrange: 100 × 0.97 × 0.97 = 94.09 (rounds to 94.09)
        var listPrice = 100m;
        var discounts = new List<decimal> { 3, 3 };

        // Act
        var result = PricingCalculationService.CalculateNetCost(listPrice, discounts);

        // Assert
        result.Should().Be(94.09m);
    }

    // CalculateSalePriceFromMargin tests

    [Fact]
    public void CalculateSalePriceFromMargin_WithValidMargin_CalculatesCorrectly()
    {
        // Arrange: 800 / (1 - 0.25) = 800 / 0.75 = 1066.67
        var netCost = 800m;
        var marginPercentage = 25m;

        // Act
        var result = PricingCalculationService.CalculateSalePriceFromMargin(netCost, marginPercentage);

        // Assert
        result.Should().Be(1066.67m);
    }

    [Fact]
    public void CalculateSalePriceFromMargin_WithZeroMargin_ReturnsNetCost()
    {
        // Arrange
        var netCost = 500m;
        var marginPercentage = 0m;

        // Act
        var result = PricingCalculationService.CalculateSalePriceFromMargin(netCost, marginPercentage);

        // Assert
        result.Should().Be(500.00m);
    }

    [Fact]
    public void CalculateSalePriceFromMargin_With100PercentMargin_ThrowsArgumentException()
    {
        // Act
        var act = () => PricingCalculationService.CalculateSalePriceFromMargin(500m, 100m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("marginPercentage");
    }

    [Fact]
    public void CalculateSalePriceFromMargin_WithMarginOver100_ThrowsArgumentException()
    {
        // Act
        var act = () => PricingCalculationService.CalculateSalePriceFromMargin(500m, 150m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("marginPercentage");
    }

    // CalculateMarginFromSalePrice tests

    [Fact]
    public void CalculateMarginFromSalePrice_WithValidPrices_CalculatesCorrectly()
    {
        // Arrange: ((1000 - 800) / 1000) × 100 = 20%
        var netCost = 800m;
        var salePrice = 1000m;

        // Act
        var result = PricingCalculationService.CalculateMarginFromSalePrice(netCost, salePrice);

        // Assert
        result.Should().Be(20.00m);
    }

    [Fact]
    public void CalculateMarginFromSalePrice_WithZeroSalePrice_ThrowsArgumentException()
    {
        // Act
        var act = () => PricingCalculationService.CalculateMarginFromSalePrice(500m, 0m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("salePrice");
    }

    // ApplyIva tests

    [Fact]
    public void ApplyIva_WithDefaultIva_Applies16Percent()
    {
        // Arrange: 1000 × 1.16 = 1160.00
        var salePrice = 1000m;

        // Act
        var result = PricingCalculationService.ApplyIva(salePrice);

        // Assert
        result.Should().Be(1160.00m);
    }

    [Fact]
    public void ApplyIva_WithCustomIva_AppliesCorrectly()
    {
        // Arrange: 1000 × 1.08 = 1080.00
        var salePrice = 1000m;

        // Act
        var result = PricingCalculationService.ApplyIva(salePrice, 8m);

        // Assert
        result.Should().Be(1080.00m);
    }

    // RemoveIva tests

    [Fact]
    public void RemoveIva_WithDefaultIva_Removes16Percent()
    {
        // Arrange: 1160 / 1.16 = 1000.00
        var priceWithIva = 1160m;

        // Act
        var result = PricingCalculationService.RemoveIva(priceWithIva);

        // Assert
        result.Should().Be(1000.00m);
    }

    [Fact]
    public void RemoveIva_WithCustomIva_RemovesCorrectly()
    {
        // Arrange: 1080 / 1.08 = 1000.00
        var priceWithIva = 1080m;

        // Act
        var result = PricingCalculationService.RemoveIva(priceWithIva, 8m);

        // Assert
        result.Should().Be(1000.00m);
    }
}
