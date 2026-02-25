using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Commands.CancelSale;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Sales.Commands;

[Trait("Category", "Unit")]
public class CancelSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CancelSaleCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CancelSaleCommandHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CancelSaleCommandHandler(
            _saleRepositoryMock.Object,
            _inventoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithDraftSale_SetsCancelledStatus()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        sale.Status.Should().Be(SaleStatus.Cancelled);
        _saleRepositoryMock.Verify(x => x.Update(sale), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _inventoryRepositoryMock.Verify(x => x.UpdateInventoryItem(It.IsAny<InventoryItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithCompletedSale_CancelsAndRestoresInventory()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var inventoryItemId = Guid.NewGuid();

        var sale = CreateSaleWithItems(saleId, SaleStatus.Completed, warehouseId, productId, quantity: 2m);
        var inventoryItem = new InventoryItem
        {
            Id = inventoryItemId,
            TenantId = _tenantId,
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = 5m,
            ReservedQuantity = 0,
            MinimumLevel = 0
        };

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _inventoryRepositoryMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Status.Should().Be(SaleStatus.Cancelled);
        inventoryItem.Quantity.Should().Be(7m); // 5 + 2 = 7
        _inventoryRepositoryMock.Verify(x => x.UpdateInventoryItem(inventoryItem), Times.Once);
        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Quantity == 2m &&
            t.PreviousQuantity == 5m &&
            t.NewQuantity == 7m &&
            t.Type == InventoryTransactionType.Return)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCompletedSaleNoInventory_CancelsWithoutInventoryError()
    {
        // Arrange (item was sold but no inventory record — graceful skip)
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();

        var sale = CreateSaleWithItems(saleId, SaleStatus.Completed, warehouseId, productId, quantity: 1m);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _inventoryRepositoryMock.Setup(x => x.GetByProductAndWarehouseAsync(productId, warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Status.Should().Be(SaleStatus.Cancelled);
        _inventoryRepositoryMock.Verify(x => x.UpdateInventoryItem(It.IsAny<InventoryItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithReason_AppendsCancellationNotes()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);
        sale.Notes = "Original notes";

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId, Reason: "Customer changed mind");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Notes.Should().Contain("Customer changed mind");
        sale.Notes.Should().Contain("Cancelled:");
    }

    [Fact]
    public async Task Handle_WithReasonAndNoExistingNotes_SetsOnlyCancellationNotes()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);
        sale.Notes = null;

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId, Reason: "Test reason");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Notes.Should().Be("Cancelled: Test reason");
    }

    [Fact]
    public async Task Handle_WithNoReason_DoesNotModifyNotes()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);
        sale.Notes = "Original notes";

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Notes.Should().Be("Original notes");
    }

    [Fact]
    public async Task Handle_WhenSaleNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _saleRepositoryMock.Verify(x => x.Update(It.IsAny<Sale>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaleAlreadyCancelled_ReturnsConflictError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Cancelled);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.AlreadyCancelled");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    private Sale CreateSale(Guid id, SaleStatus status) =>
        new()
        {
            Id = id,
            TenantId = _tenantId,
            Folio = "V-00001",
            Type = SaleType.Pos,
            Status = status,
            WarehouseId = Guid.NewGuid(),
            SubTotal = 100m,
            Total = 100m,
            Items = []
        };

    private Sale CreateSaleWithItems(Guid id, SaleStatus status, Guid warehouseId, Guid productId, decimal quantity) =>
        new()
        {
            Id = id,
            TenantId = _tenantId,
            Folio = "V-00001",
            Type = SaleType.Pos,
            Status = status,
            WarehouseId = warehouseId,
            SubTotal = 200m,
            Total = 200m,
            Items =
            [
                new SaleItem
                {
                    TenantId = _tenantId,
                    ProductId = productId,
                    ProductName = "Test Product",
                    ProductSku = "PROD-001",
                    UnitPrice = 100m,
                    Quantity = quantity,
                    DiscountPercentage = 0,
                    TaxPercentage = 0,
                    LineTotal = 100m * quantity
                }
            ]
        };
}
