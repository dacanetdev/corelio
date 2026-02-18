using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Commands.UpdateProductPricing;
using Corelio.Application.Pricing.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Pricing.Handlers;

[Trait("Category", "Unit")]
public class UpdateProductPricingCommandHandlerTests
{
    private readonly Mock<IProductPricingRepository> _productPricingRepositoryMock;
    private readonly Mock<ITenantPricingConfigurationRepository> _configRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly UpdateProductPricingCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public UpdateProductPricingCommandHandlerTests()
    {
        _productPricingRepositoryMock = new Mock<IProductPricingRepository>();
        _configRepositoryMock = new Mock<ITenantPricingConfigurationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new UpdateProductPricingCommandHandler(
            _productPricingRepositoryMock.Object,
            _configRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_UpdatesProductPricing()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var product = CreateProductWithPricing(_tenantId);
        var config = CreateDefaultConfig(_tenantId);

        // First call: validation lookup; second call: reload for response
        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var command = new UpdateProductPricingCommand(
            ProductId: product.Id,
            ListPrice: 1000m,
            IvaEnabled: true,
            Discounts:
            [
                new UpdateProductDiscountDto(1, 10m),
                new UpdateProductDiscountDto(2, 5m),
                new UpdateProductDiscountDto(3, 2m)
            ],
            MarginPrices:
            [
                new UpdateProductMarginPriceDto(1, 30m, null),
                new UpdateProductMarginPriceDto(2, 25m, null),
                new UpdateProductMarginPriceDto(3, 20m, null)
            ]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _productPricingRepositoryMock.Verify(
            x => x.UpdateProductPricingAsync(
                product.Id,
                It.IsAny<List<ProductDiscount>>(),
                It.IsAny<List<ProductMarginPrice>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = new UpdateProductPricingCommand(
            ProductId: Guid.NewGuid(),
            ListPrice: 1000m,
            IvaEnabled: true,
            Discounts: [new UpdateProductDiscountDto(1, 10m)],
            MarginPrices: [new UpdateProductMarginPriceDto(1, 30m, null)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _productPricingRepositoryMock.Verify(
            x => x.UpdateProductPricingAsync(It.IsAny<Guid>(), It.IsAny<List<ProductDiscount>>(), It.IsAny<List<ProductMarginPrice>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new UpdateProductPricingCommand(
            ProductId: productId,
            ListPrice: 1000m,
            IvaEnabled: true,
            Discounts: [new UpdateProductDiscountDto(1, 10m)],
            MarginPrices: [new UpdateProductMarginPriceDto(1, 30m, null)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var product = CreateProductWithPricing(_tenantId);
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantPricingConfiguration?)null);

        var command = new UpdateProductPricingCommand(
            ProductId: product.Id,
            ListPrice: 1000m,
            IvaEnabled: true,
            Discounts: [new UpdateProductDiscountDto(1, 10m)],
            MarginPrices: [new UpdateProductMarginPriceDto(1, 30m, null)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("PricingConfig.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_CalculatesNetCostFromDiscounts()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var product = CreateProductWithPricing(_tenantId);
        var config = CreateDefaultConfig(_tenantId);

        Product? capturedProduct = null;
        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product)
            .Callback(() =>
            {
                // Capture state after handler mutates it (before second call)
                if (capturedProduct is null)
                {
                    capturedProduct = product;
                }
            });

        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Discounts: 10%, 5%, 2% on ListPrice 1000
        // NetCost = 1000 * 0.90 * 0.95 * 0.98 = 837.90
        var command = new UpdateProductPricingCommand(
            ProductId: product.Id,
            ListPrice: 1000m,
            IvaEnabled: true,
            Discounts:
            [
                new UpdateProductDiscountDto(1, 10m),
                new UpdateProductDiscountDto(2, 5m),
                new UpdateProductDiscountDto(3, 2m)
            ],
            MarginPrices:
            [
                new UpdateProductMarginPriceDto(1, 30m, null)
            ]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // After the handler processes the command, the product's NetCost should be updated
        product.NetCost.Should().Be(837.90m);
    }

    private static TenantPricingConfiguration CreateDefaultConfig(Guid tenantId) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        DiscountTierCount = 3,
        MarginTierCount = 3,
        DefaultIvaEnabled = true,
        IvaPercentage = 16.00m,
        DiscountTierDefinitions =
        [
            new() { TierNumber = 1, TierName = "Descuento 1", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 2, TierName = "Descuento 2", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 3, TierName = "Descuento 3", IsActive = true, TenantId = tenantId }
        ],
        MarginTierDefinitions =
        [
            new() { TierNumber = 1, TierName = "Publico", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 2, TierName = "Mayoreo", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 3, TierName = "Especial", IsActive = true, TenantId = tenantId }
        ]
    };

    private static Product CreateProductWithPricing(Guid tenantId)
    {
        var productId = Guid.NewGuid();
        return new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "Test Product",
            Sku = "TEST-001",
            ListPrice = 1000m,
            NetCost = 837.90m,
            IvaEnabled = true,
            SalePrice = 100m,
            Discounts =
            [
                new() { ProductId = productId, TierNumber = 1, DiscountPercentage = 10m, TenantId = tenantId },
                new() { ProductId = productId, TierNumber = 2, DiscountPercentage = 5m, TenantId = tenantId },
                new() { ProductId = productId, TierNumber = 3, DiscountPercentage = 2m, TenantId = tenantId }
            ],
            MarginPrices =
            [
                new() { ProductId = productId, TierNumber = 1, MarginPercentage = 30m, SalePrice = 1197.00m, PriceWithIva = 1388.52m, TenantId = tenantId },
                new() { ProductId = productId, TierNumber = 2, MarginPercentage = 25m, SalePrice = 1117.20m, PriceWithIva = 1295.95m, TenantId = tenantId },
                new() { ProductId = productId, TierNumber = 3, MarginPercentage = 20m, SalePrice = 1047.38m, PriceWithIva = 1214.96m, TenantId = tenantId }
            ]
        };
    }
}
