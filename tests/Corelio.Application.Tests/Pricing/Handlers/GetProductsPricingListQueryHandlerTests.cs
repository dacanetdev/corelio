using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.Application.Pricing.Queries.GetProductsPricingList;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Pricing.Handlers;

[Trait("Category", "Unit")]
public class GetProductsPricingListQueryHandlerTests
{
    private readonly Mock<IProductPricingRepository> _productPricingRepositoryMock;
    private readonly Mock<ITenantPricingConfigurationRepository> _configRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly GetProductsPricingListQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetProductsPricingListQueryHandlerTests()
    {
        _productPricingRepositoryMock = new Mock<IProductPricingRepository>();
        _configRepositoryMock = new Mock<ITenantPricingConfigurationRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new GetProductsPricingListQueryHandler(
            _productPricingRepositoryMock.Object,
            _configRepositoryMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ReturnsPagedResult()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var config = CreateDefaultConfig(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var products = new List<Product>
        {
            CreateProductWithPricing(_tenantId, "Product A", "SKU-A"),
            CreateProductWithPricing(_tenantId, "Product B", "SKU-B")
        };

        _productPricingRepositoryMock
            .Setup(x => x.GetProductsPricingListAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 2));

        var query = new GetProductsPricingListQuery(PageNumber: 1, PageSize: 20);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var query = new GetProductsPricingListQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _configRepositoryMock.Verify(
            x => x.GetWithTierDefinitionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantPricingConfiguration?)null);

        var query = new GetProductsPricingListQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("PricingConfig.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _productPricingRepositoryMock.Verify(
            x => x.GetProductsPricingListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_PassesToRepository()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var config = CreateDefaultConfig(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        _productPricingRepositoryMock
            .Setup(x => x.GetProductsPricingListAsync(1, 10, "cement", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        var query = new GetProductsPricingListQuery(PageNumber: 1, PageSize: 10, SearchTerm: "cement");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _productPricingRepositoryMock.Verify(
            x => x.GetProductsPricingListAsync(1, 10, "cement", null, It.IsAny<CancellationToken>()),
            Times.Once);
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

    private static Product CreateProductWithPricing(Guid tenantId, string name = "Test Product", string sku = "TEST-001")
    {
        var productId = Guid.NewGuid();
        return new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = name,
            Sku = sku,
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
