using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Commands.CompleteSale;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Sales.Commands;

[Trait("Category", "Unit")]
public class CompleteSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CompleteSaleCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CompleteSaleCommandHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CompleteSaleCommandHandler(
            _saleRepositoryMock.Object,
            _inventoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidPayment_CompletesAndDeductsInventory()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var inventoryItemId = Guid.NewGuid();

        var sale = CreateDraftSale(saleId, warehouseId, productId, quantity: 2m, total: 200m);
        var inventoryItem = CreateInventoryItem(inventoryItemId, productId, warehouseId, quantity: 10m);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _inventoryRepositoryMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        var command = new CompleteSaleCommand(
            SaleId: saleId,
            Payments: [new PaymentRequest(PaymentMethod.Cash, Amount: 200m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(SaleStatus.Completed);
        result.Value.CompletedAt.Should().NotBeNull();
        result.Value.Payments.Should().HaveCount(1);
        result.Value.Payments.First().Amount.Should().Be(200m);

        // Inventory should have been deducted
        inventoryItem.Quantity.Should().Be(8m); // 10 - 2 = 8
        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Quantity == -2m &&
            t.PreviousQuantity == 10m &&
            t.NewQuantity == 8m &&
            t.Type == InventoryTransactionType.Sale)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultiplePayments_AccumulatesAllPayments()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();

        var sale = CreateDraftSale(saleId, warehouseId, productId, quantity: 1m, total: 500m);
        var inventoryItem = CreateInventoryItem(Guid.NewGuid(), productId, warehouseId, quantity: 5m);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _inventoryRepositoryMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        var command = new CompleteSaleCommand(
            SaleId: saleId,
            Payments:
            [
                new PaymentRequest(PaymentMethod.Cash, Amount: 300m),
                new PaymentRequest(PaymentMethod.Card, Amount: 200m, Reference: "AUTH123")
            ]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Payments.Should().HaveCount(2);
        result.Value.Payments.Sum(p => p.Amount).Should().Be(500m);
    }

    [Fact]
    public async Task Handle_WhenOverpaid_Succeeds()
    {
        // Arrange (customer pays more than total — acceptable, change given at cashier)
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();

        var sale = CreateDraftSale(saleId, warehouseId, productId, quantity: 1m, total: 100m);
        var inventoryItem = CreateInventoryItem(Guid.NewGuid(), productId, warehouseId, quantity: 5m);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _inventoryRepositoryMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        var command = new CompleteSaleCommand(
            SaleId: saleId,
            Payments: [new PaymentRequest(PaymentMethod.Cash, Amount: 200m)]); // Overpay 200 for 100 total

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenSaleNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new CompleteSaleCommand(
            SaleId: saleId,
            Payments: [new PaymentRequest(PaymentMethod.Cash, Amount: 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSaleAlreadyCompleted_ReturnsConflictError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateDraftSale(saleId, Guid.NewGuid(), Guid.NewGuid(), quantity: 1m, total: 100m);
        sale.Status = SaleStatus.Completed;

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CompleteSaleCommand(
            SaleId: saleId,
            Payments: [new PaymentRequest(PaymentMethod.Cash, Amount: 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.InvalidStatus");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_WhenPaymentShort_ReturnsValidationError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateDraftSale(saleId, Guid.NewGuid(), Guid.NewGuid(), quantity: 1m, total: 500m);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CompleteSaleCommand(
            SaleId: saleId,
            Payments: [new PaymentRequest(PaymentMethod.Cash, Amount: 200m)]); // Only 200 for 500

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.PaymentShort");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WhenNoExistingInventory_CreatesInventoryRecordAndDeducts()
    {
        // Arrange (first sale of this product — no inventory record yet)
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();

        var sale = CreateDraftSale(saleId, warehouseId, productId, quantity: 3m, total: 300m);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _inventoryRepositoryMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null); // No existing inventory

        var command = new CompleteSaleCommand(
            SaleId: saleId,
            Payments: [new PaymentRequest(PaymentMethod.Cash, Amount: 300m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _inventoryRepositoryMock.Verify(x => x.AddInventoryItem(It.IsAny<InventoryItem>()), Times.Once);
        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Quantity == -3m &&
            t.PreviousQuantity == 0m &&
            t.NewQuantity == -3m)), Times.Once);
    }

    private Sale CreateDraftSale(Guid saleId, Guid warehouseId, Guid productId, decimal quantity, decimal total)
    {
        var saleItem = new SaleItem
        {
            ProductId = productId,
            ProductName = "Test Product",
            ProductSku = "PROD-001",
            UnitPrice = total / quantity,
            Quantity = quantity,
            DiscountPercentage = 0,
            TaxPercentage = 0,
            LineTotal = total,
            TenantId = _tenantId
        };

        return new Sale
        {
            Id = saleId,
            TenantId = _tenantId,
            Folio = "V-00001",
            Type = SaleType.Pos,
            Status = SaleStatus.Draft,
            WarehouseId = warehouseId,
            SubTotal = total,
            DiscountAmount = 0,
            TaxAmount = 0,
            Total = total,
            Items = [saleItem]
        };
    }

    private static InventoryItem CreateInventoryItem(Guid id, Guid productId, Guid warehouseId, decimal quantity) =>
        new()
        {
            Id = id,
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            ReservedQuantity = 0,
            MinimumLevel = 0
        };
}
