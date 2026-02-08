using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Commands.CalculatePrices;
using FluentAssertions;

namespace Corelio.Application.Tests.Pricing.Handlers;

[Trait("Category", "Unit")]
public class CalculatePricesCommandHandlerTests
{
    private readonly CalculatePricesCommandHandler _handler;

    public CalculatePricesCommandHandlerTests()
    {
        _handler = new CalculatePricesCommandHandler();
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsCalculatedPrices()
    {
        // Arrange
        // ListPrice 1000, discounts [10, 5, 2] -> NetCost = 1000 * 0.90 * 0.95 * 0.98 = 837.90
        var command = new CalculatePricesCommand(
            ListPrice: 1000m,
            Discounts: [10m, 5m, 2m],
            IvaEnabled: true,
            IvaPercentage: 16.00m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.NetCost.Should().Be(837.90m);
        result.Value.SamplePrices.Should().HaveCount(9);

        // Verify sample prices at margins [10, 15, 20, 25, 30, 35, 40, 45, 50]
        var samplePrices = result.Value.SamplePrices;

        samplePrices[0].MarginPercentage.Should().Be(10m);
        samplePrices[1].MarginPercentage.Should().Be(15m);
        samplePrices[2].MarginPercentage.Should().Be(20m);
        samplePrices[3].MarginPercentage.Should().Be(25m);
        samplePrices[4].MarginPercentage.Should().Be(30m);
        samplePrices[5].MarginPercentage.Should().Be(35m);
        samplePrices[6].MarginPercentage.Should().Be(40m);
        samplePrices[7].MarginPercentage.Should().Be(45m);
        samplePrices[8].MarginPercentage.Should().Be(50m);

        // NetCost = 837.90
        // SalePrice at 10% margin = 837.90 / (1 - 0.10) = 837.90 / 0.90 = 931.00
        samplePrices[0].SalePrice.Should().Be(931.00m);
        // PriceWithIva at 10% margin = 931.00 * 1.16 = 1079.96
        samplePrices[0].PriceWithIva.Should().Be(1079.96m);

        // SalePrice at 30% margin = 837.90 / (1 - 0.30) = 837.90 / 0.70 = 1197.00
        samplePrices[4].SalePrice.Should().Be(1197.00m);

        // SalePrice at 50% margin = 837.90 / (1 - 0.50) = 837.90 / 0.50 = 1675.80
        samplePrices[8].SalePrice.Should().Be(1675.80m);
    }

    [Fact]
    public async Task Handle_WithIvaDisabled_PriceWithIvaEqualsSalePrice()
    {
        // Arrange
        var command = new CalculatePricesCommand(
            ListPrice: 500m,
            Discounts: [10m],
            IvaEnabled: false,
            IvaPercentage: 16.00m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        foreach (var samplePrice in result.Value!.SamplePrices)
        {
            samplePrice.PriceWithIva.Should().Be(samplePrice.SalePrice,
                because: "when IVA is disabled, PriceWithIva should equal SalePrice");
        }
    }

    [Fact]
    public async Task Handle_CalculatesCorrectNetCost()
    {
        // Arrange
        // Cascading discounts: 1000 * 0.80 * 0.90 * 0.95 = 684.00
        var command = new CalculatePricesCommand(
            ListPrice: 1000m,
            Discounts: [20m, 10m, 5m],
            IvaEnabled: true,
            IvaPercentage: 16.00m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.NetCost.Should().Be(684.00m);
    }
}
