using Corelio.Application.Common.Enums;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Commands.BulkUpdatePricing;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Pricing.Handlers;

[Trait("Category", "Unit")]
public class BulkUpdatePricingCommandHandlerTests
{
    private readonly Mock<IProductPricingRepository> _productPricingRepositoryMock;
    private readonly Mock<ITenantPricingConfigurationRepository> _configRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly BulkUpdatePricingCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public BulkUpdatePricingCommandHandlerTests()
    {
        _productPricingRepositoryMock = new Mock<IProductPricingRepository>();
        _configRepositoryMock = new Mock<ITenantPricingConfigurationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new BulkUpdatePricingCommandHandler(
            _productPricingRepositoryMock.Object,
            _configRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithPercentageIncrease_UpdatesAllProducts()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var config = CreateDefaultConfig(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var product1 = CreateProductWithPricing(_tenantId, "Product A", "SKU-A");
        var product2 = CreateProductWithPricing(_tenantId, "Product B", "SKU-B");

        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(product1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);
        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(product2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product2);

        var command = new BulkUpdatePricingCommand(
            ProductIds: [product1.Id, product2.Id],
            UpdateType: PricingUpdateType.PercentageIncrease,
            Value: 10m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2);
        _productPricingRepositoryMock.Verify(
            x => x.UpdateProductPricingAsync(
                It.IsAny<Guid>(),
                It.IsAny<List<ProductDiscount>>(),
                It.IsAny<List<ProductMarginPrice>>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = new BulkUpdatePricingCommand(
            ProductIds: [Guid.NewGuid()],
            UpdateType: PricingUpdateType.PercentageIncrease,
            Value: 10m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantPricingConfiguration?)null);

        var command = new BulkUpdatePricingCommand(
            ProductIds: [Guid.NewGuid()],
            UpdateType: PricingUpdateType.PercentageIncrease,
            Value: 10m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("PricingConfig.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_SkipsNotFoundProducts_ReturnsUpdatedCount()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var config = CreateDefaultConfig(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var existingProduct = CreateProductWithPricing(_tenantId);
        var missingProductId = Guid.NewGuid();

        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(missingProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new BulkUpdatePricingCommand(
            ProductIds: [existingProduct.Id, missingProductId],
            UpdateType: PricingUpdateType.PercentageIncrease,
            Value: 5m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        _productPricingRepositoryMock.Verify(
            x => x.UpdateProductPricingAsync(
                existingProduct.Id,
                It.IsAny<List<ProductDiscount>>(),
                It.IsAny<List<ProductMarginPrice>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UsesTransaction()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var config = CreateDefaultConfig(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var product = CreateProductWithPricing(_tenantId);
        _productPricingRepositoryMock
            .Setup(x => x.GetProductPricingAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var command = new BulkUpdatePricingCommand(
            ProductIds: [product.Id],
            UpdateType: PricingUpdateType.PercentageIncrease,
            Value: 10m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
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
