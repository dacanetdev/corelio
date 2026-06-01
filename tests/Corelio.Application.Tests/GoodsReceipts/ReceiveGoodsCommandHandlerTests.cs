using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.GoodsReceipts.Commands.ReceiveGoods;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.GoodsReceipts;

[Trait("Category", "Unit")]
public class ReceiveGoodsCommandHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _poRepoMock;
    private readonly Mock<IGoodsReceiptRepository> _receiptRepoMock;
    private readonly Mock<IInventoryRepository> _inventoryRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ITenantService> _tenantMock;
    private readonly ReceiveGoodsCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();

    public ReceiveGoodsCommandHandlerTests()
    {
        _poRepoMock = new Mock<IPurchaseOrderRepository>();
        _receiptRepoMock = new Mock<IGoodsReceiptRepository>();
        _inventoryRepoMock = new Mock<IInventoryRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _tenantMock = new Mock<ITenantService>();
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _inventoryRepoMock
            .Setup(x => x.GetAllWarehousesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Warehouse { Id = _warehouseId, Name = "Almacén", TenantId = _tenantId }]);

        _handler = new ReceiveGoodsCommandHandler(
            _poRepoMock.Object,
            _receiptRepoMock.Object,
            _inventoryRepoMock.Object,
            _uowMock.Object,
            _tenantMock.Object);
    }

    [Fact]
    public async Task Handle_WithApprovedPO_FullReceipt_TransitionsToReceived()
    {
        // Arrange
        var poItem = CreatePoItem(orderedQty: 10m);
        var po = CreatePurchaseOrder([poItem], PurchaseOrderStatus.Approved);
        SetupPoRepo(po);
        SetupNoExistingInventory(poItem.ProductId);

        var command = CreateCommand(po.Id, [(poItem.Id, 10m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        po.Status.Should().Be(PurchaseOrderStatus.Received);
        poItem.ReceivedQuantity.Should().Be(10m);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithApprovedPO_PartialReceipt_TransitionsToPartiallyReceived()
    {
        // Arrange
        var poItem = CreatePoItem(orderedQty: 10m);
        var po = CreatePurchaseOrder([poItem], PurchaseOrderStatus.Approved);
        SetupPoRepo(po);
        SetupNoExistingInventory(poItem.ProductId);

        var command = CreateCommand(po.Id, [(poItem.Id, 4m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        po.Status.Should().Be(PurchaseOrderStatus.PartiallyReceived);
        poItem.ReceivedQuantity.Should().Be(4m);
    }

    [Fact]
    public async Task Handle_WithPartiallyReceivedPO_SecondReceiptCompletes_TransitionsToReceived()
    {
        // Arrange — first receipt already set 4 of 10
        var poItem = CreatePoItem(orderedQty: 10m, receivedQty: 4m);
        var po = CreatePurchaseOrder([poItem], PurchaseOrderStatus.PartiallyReceived);
        SetupPoRepo(po);
        SetupNoExistingInventory(poItem.ProductId);

        var command = CreateCommand(po.Id, [(poItem.Id, 6m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        po.Status.Should().Be(PurchaseOrderStatus.Received);
        poItem.ReceivedQuantity.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_WhenQuantityExceedsRemaining_ReturnsOverReceiptError()
    {
        // Arrange — 3 already received, 7 remaining; requesting 8
        var poItem = CreatePoItem(orderedQty: 10m, receivedQty: 3m);
        var po = CreatePurchaseOrder([poItem], PurchaseOrderStatus.PartiallyReceived);
        SetupPoRepo(po);

        var command = CreateCommand(po.Id, [(poItem.Id, 8m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("GoodsReceipt.OverReceipt");
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPOIsInDraftStatus_ReturnsConflictError()
    {
        // Arrange
        var po = CreatePurchaseOrder([], PurchaseOrderStatus.Draft);
        SetupPoRepo(po);

        var command = CreateCommand(po.Id, []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("PurchaseOrder.InvalidTransition");
    }

    [Fact]
    public async Task Handle_WhenPOIsCancelled_ReturnsConflictError()
    {
        // Arrange
        var po = CreatePurchaseOrder([], PurchaseOrderStatus.Cancelled);
        SetupPoRepo(po);

        var command = CreateCommand(po.Id, []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("PurchaseOrder.InvalidTransition");
    }

    [Fact]
    public async Task Handle_WhenPONotFound_ReturnsNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();
        _poRepoMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder?)null);

        var command = CreateCommand(id, []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("PurchaseOrder.NotFound");
    }

    [Fact]
    public async Task Handle_WhenInventoryItemDoesNotExist_CreatesNewInventoryItem()
    {
        // Arrange
        var poItem = CreatePoItem(orderedQty: 5m);
        var po = CreatePurchaseOrder([poItem], PurchaseOrderStatus.Approved);
        SetupPoRepo(po);
        SetupNoExistingInventory(poItem.ProductId);

        var command = CreateCommand(po.Id, [(poItem.Id, 5m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _inventoryRepoMock.Verify(
            x => x.AddInventoryItem(It.Is<InventoryItem>(i =>
                i.ProductId == poItem.ProductId &&
                i.WarehouseId == _warehouseId &&
                i.Quantity == 5m)),
            Times.Once);
        _inventoryRepoMock.Verify(x => x.UpdateInventoryItem(It.IsAny<InventoryItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInventoryItemExists_IncrementsQuantity()
    {
        // Arrange
        var poItem = CreatePoItem(orderedQty: 5m);
        var po = CreatePurchaseOrder([poItem], PurchaseOrderStatus.Approved);
        SetupPoRepo(po);

        var existingItem = new InventoryItem
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            ProductId = poItem.ProductId,
            WarehouseId = _warehouseId,
            Quantity = 20m
        };
        _inventoryRepoMock
            .Setup(x => x.GetByProductAndWarehouseAsync(poItem.ProductId, _warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        var command = CreateCommand(po.Id, [(poItem.Id, 5m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingItem.Quantity.Should().Be(25m);
        _inventoryRepoMock.Verify(x => x.UpdateInventoryItem(existingItem), Times.Once);
        _inventoryRepoMock.Verify(x => x.AddInventoryItem(It.IsAny<InventoryItem>()), Times.Never);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static PurchaseOrderItem CreatePoItem(decimal orderedQty, decimal receivedQty = 0m) => new()
    {
        Id = Guid.NewGuid(),
        ProductId = Guid.NewGuid(),
        ProductName = "Producto de Prueba",
        Quantity = orderedQty,
        UnitPrice = 10m,
        Subtotal = orderedQty * 10m,
        ReceivedQuantity = receivedQty
    };

    private PurchaseOrder CreatePurchaseOrder(
        ICollection<PurchaseOrderItem> items,
        PurchaseOrderStatus status) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = _tenantId,
        OrderNumber = "PO-2026-0001",
        SupplierId = Guid.NewGuid(),
        Status = status,
        Items = items
    };

    private void SetupPoRepo(PurchaseOrder po)
    {
        _poRepoMock.Setup(x => x.GetByIdAsync(po.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);
    }

    private void SetupNoExistingInventory(Guid productId)
    {
        _inventoryRepoMock
            .Setup(x => x.GetByProductAndWarehouseAsync(productId, _warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);
    }

    private ReceiveGoodsCommand CreateCommand(
        Guid poId,
        IEnumerable<(Guid itemId, decimal qty)> items) => new(
        PurchaseOrderId: poId,
        WarehouseId: _warehouseId,
        ReceivedDate: DateOnly.FromDateTime(DateTime.UtcNow),
        Notes: null,
        Items: items.Select(i => new ReceiveGoodsItemRequest(i.itemId, i.qty)).ToList());
}
