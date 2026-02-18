using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Commands.CreateSale;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Sales.Commands;

[Trait("Category", "Unit")]
public class CreateSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly CreateSaleCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CreateSaleCommandHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new CreateSaleCommandHandler(
            _saleRepositoryMock.Object,
            _inventoryRepositoryMock.Object,
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidItems_ReturnsSuccessWithSaleId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var product = CreateProduct(productId, ivaEnabled: true, taxRate: 0.16m);
        var warehouse = CreateWarehouse(warehouseId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _inventoryRepositoryMock.Setup(x => x.GetDefaultWarehouseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _saleRepositoryMock.Setup(x => x.GetNextFolioNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(productId, Quantity: 2, UnitPrice: 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _saleRepositoryMock.Verify(x => x.Add(It.Is<Sale>(s =>
            s.Folio == "V-00001" &&
            s.Status == SaleStatus.Draft &&
            s.TenantId == _tenantId &&
            s.Items.Count == 1)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithIvaEnabledProduct_CalculatesTaxCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateProduct(productId, ivaEnabled: true, taxRate: 0.16m);
        var warehouse = CreateWarehouse(Guid.NewGuid());

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _inventoryRepositoryMock.Setup(x => x.GetDefaultWarehouseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _saleRepositoryMock.Setup(x => x.GetNextFolioNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // UnitPrice=1000, Qty=1, Discount=0 → SubTotal=1000, Tax=160, Total=1160
        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(productId, Quantity: 1, UnitPrice: 1000m)]);

        Sale? capturedSale = null;
        _saleRepositoryMock.Setup(x => x.Add(It.IsAny<Sale>()))
            .Callback<Sale>(s => capturedSale = s);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedSale.Should().NotBeNull();
        capturedSale!.SubTotal.Should().Be(1000m);
        capturedSale.TaxAmount.Should().Be(160m);
        capturedSale.Total.Should().Be(1160m);
        capturedSale.Items.First().TaxPercentage.Should().Be(16m);
        capturedSale.Items.First().LineTotal.Should().Be(1160m);
    }

    [Fact]
    public async Task Handle_WithDiscountPercentage_DeductsDiscountFromLineTotal()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateProduct(productId, ivaEnabled: false, taxRate: 0m);
        var warehouse = CreateWarehouse(Guid.NewGuid());

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _inventoryRepositoryMock.Setup(x => x.GetDefaultWarehouseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _saleRepositoryMock.Setup(x => x.GetNextFolioNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // UnitPrice=200, Qty=2, Discount=10% → LineBase=400, LineDiscount=40, LineNet=360, Tax=0, Total=360
        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(productId, Quantity: 2, UnitPrice: 200m, DiscountPercentage: 10m)]);

        Sale? capturedSale = null;
        _saleRepositoryMock.Setup(x => x.Add(It.IsAny<Sale>()))
            .Callback<Sale>(s => capturedSale = s);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedSale.Should().NotBeNull();
        capturedSale!.Folio.Should().Be("V-00005");
        capturedSale.SubTotal.Should().Be(360m);
        capturedSale.DiscountAmount.Should().Be(40m);
        capturedSale.TaxAmount.Should().Be(0m);
        capturedSale.Total.Should().Be(360m);
        capturedSale.Items.First().LineTotal.Should().Be(360m);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(Guid.NewGuid(), Quantity: 1, UnitPrice: 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _saleRepositoryMock.Verify(x => x.Add(It.IsAny<Sale>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDefaultWarehouseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _inventoryRepositoryMock.Setup(x => x.GetDefaultWarehouseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(Guid.NewGuid(), Quantity: 1, UnitPrice: 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Warehouse.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var unknownProductId = Guid.NewGuid();
        var warehouse = CreateWarehouse(Guid.NewGuid());

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _inventoryRepositoryMock.Setup(x => x.GetDefaultWarehouseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
        _productRepositoryMock.Setup(x => x.GetByIdAsync(unknownProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(unknownProductId, Quantity: 1, UnitPrice: 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Product.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WithExplicitWarehouseId_UsesThatWarehouse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var explicitWarehouseId = Guid.NewGuid();
        var product = CreateProduct(productId, ivaEnabled: false, taxRate: 0m);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _saleRepositoryMock.Setup(x => x.GetNextFolioNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Sale? capturedSale = null;
        _saleRepositoryMock.Setup(x => x.Add(It.IsAny<Sale>()))
            .Callback<Sale>(s => capturedSale = s);

        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(productId, Quantity: 1, UnitPrice: 50m)],
            WarehouseId: explicitWarehouseId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedSale!.WarehouseId.Should().Be(explicitWarehouseId);
        // GetDefaultWarehouse should NOT be called when warehouse is explicit
        _inventoryRepositoryMock.Verify(x => x.GetDefaultWarehouseAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithIvaDisabledProduct_SetsTaxToZero()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateProduct(productId, ivaEnabled: false, taxRate: 0m);
        var warehouse = CreateWarehouse(Guid.NewGuid());

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _inventoryRepositoryMock.Setup(x => x.GetDefaultWarehouseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _saleRepositoryMock.Setup(x => x.GetNextFolioNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Sale? capturedSale = null;
        _saleRepositoryMock.Setup(x => x.Add(It.IsAny<Sale>()))
            .Callback<Sale>(s => capturedSale = s);

        var command = new CreateSaleCommand(
            Items: [new CartItemRequest(productId, Quantity: 3, UnitPrice: 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedSale!.TaxAmount.Should().Be(0m);
        capturedSale.Total.Should().Be(300m);
        capturedSale.Items.First().TaxPercentage.Should().Be(0m);
    }

    private static Product CreateProduct(Guid id, bool ivaEnabled, decimal taxRate) =>
        new()
        {
            Id = id,
            Sku = "PROD-001",
            Name = "Test Product",
            SalePrice = 100m,
            CostPrice = 50m,
            UnitOfMeasure = UnitOfMeasure.PCS,
            IvaEnabled = ivaEnabled,
            TaxRate = taxRate,
            IsActive = true
        };

    private static Warehouse CreateWarehouse(Guid id) =>
        new()
        {
            Id = id,
            Name = "Almacén Principal",
            Type = WarehouseType.Main,
            IsDefault = true
        };
}
