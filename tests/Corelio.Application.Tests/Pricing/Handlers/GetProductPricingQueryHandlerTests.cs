using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Queries.GetProductPricing;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Pricing.Handlers;

[Trait("Category", "Unit")]
public class GetProductPricingQueryHandlerTests
{
    private readonly Mock<IProductPricingRepository> _productPricingRepositoryMock;
    private readonly Mock<ITenantPricingConfigurationRepository> _configRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly GetProductPricingQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetProductPricingQueryHandlerTests()
    {
        _productPricingRepositoryMock = new Mock<IProductPricingRepository>();
        _configRepositoryMock = new Mock<ITenantPricingConfigurationRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new GetProductPricingQueryHandler(
            _productPricingRepositoryMock.Object,
            _configRepositoryMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidProduct_ReturnsProductPricing()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var product = CreateProductWithPricing(_tenantId);
        var config = CreateDefaultConfig(_tenantId);

        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var query = new GetProductPricingQuery(product.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.ProductId.Should().Be(product.Id);
        result.Value.ProductName.Should().Be("Test Product");
        result.Value.Sku.Should().Be("TEST-001");
        result.Value.ListPrice.Should().Be(1000m);
        result.Value.NetCost.Should().Be(837.90m);
        result.Value.IvaEnabled.Should().BeTrue();
        result.Value.Discounts.Should().HaveCount(3);
        result.Value.MarginPrices.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var query = new GetProductPricingQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _productPricingRepositoryMock.Verify(
            x => x.GetProductPricingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
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

        var query = new GetProductPricingQuery(productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _configRepositoryMock.Verify(
            x => x.GetWithTierDefinitionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
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

        var query = new GetProductPricingQuery(product.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("PricingConfig.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
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
