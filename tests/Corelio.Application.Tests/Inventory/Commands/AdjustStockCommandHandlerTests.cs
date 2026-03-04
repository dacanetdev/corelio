using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Commands.AdjustStock;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Inventory.Commands;

[Trait("Category", "Unit")]
public class AdjustStockCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AdjustStockCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public AdjustStockCommandHandlerTests()
    {
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AdjustStockCommandHandler(
            _inventoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_Increase_HappyPath_QuantityIncreasesAndAdjustmentPositiveTransactionCreated()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = CreateInventoryItem(itemId, quantity: 10m, reservedQuantity: 0m);

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 5m,
            IsIncrease: true,
            ReasonCode: "Other",
            Notes: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        item.Quantity.Should().Be(15m);

        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Type == InventoryTransactionType.AdjustmentPositive &&
            t.Quantity == 5m &&
            t.PreviousQuantity == 10m &&
            t.NewQuantity == 15m)), Times.Once);

        _inventoryRepositoryMock.Verify(x => x.UpdateInventoryItem(item), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Decrease_HappyPath_QuantityDecreasesAndAdjustmentNegativeTransactionCreated()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = CreateInventoryItem(itemId, quantity: 10m, reservedQuantity: 0m);

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 3m,
            IsIncrease: false,
            ReasonCode: "CountCorrection",
            Notes: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(7m);

        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Type == InventoryTransactionType.AdjustmentNegative &&
            t.Quantity == -3m &&
            t.PreviousQuantity == 10m &&
            t.NewQuantity == 7m)), Times.Once);
    }

    [Fact]
    public async Task Handle_Decrease_BelowAvailableQuantity_ReturnsFailure()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        // AvailableQuantity = 5 - 2 = 3
        var item = CreateInventoryItem(itemId, quantity: 5m, reservedQuantity: 2m);

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 4m,
            IsIncrease: false,
            ReasonCode: "Other",
            Notes: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Inventory.InsufficientStock");
        result.Error.Type.Should().Be(ErrorType.Validation);

        _inventoryRepositoryMock.Verify(x => x.UpdateInventoryItem(It.IsAny<InventoryItem>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Decrease_ExactlyAvailableQuantity_Succeeds()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        // AvailableQuantity = 5 - 2 = 3
        var item = CreateInventoryItem(itemId, quantity: 5m, reservedQuantity: 2m);

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 3m,
            IsIncrease: false,
            ReasonCode: "Other",
            Notes: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(2m);
    }

    [Fact]
    public async Task Handle_InventoryItemNotFound_ReturnsNotFoundFailure()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 5m,
            IsIncrease: true,
            ReasonCode: "Other",
            Notes: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Inventory.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ReasonCode_Damaged_MapsToDamagedTransactionType()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = CreateInventoryItem(itemId, quantity: 10m, reservedQuantity: 0m);

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 1m,
            IsIncrease: false,
            ReasonCode: "Damaged",
            Notes: null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Type == InventoryTransactionType.Damaged)), Times.Once);
    }

    [Fact]
    public async Task Handle_ReasonCode_Found_MapsToFoundTransactionType()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = CreateInventoryItem(itemId, quantity: 10m, reservedQuantity: 0m);

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 2m,
            IsIncrease: true,
            ReasonCode: "Found",
            Notes: null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Type == InventoryTransactionType.Found)), Times.Once);
    }

    [Fact]
    public async Task Handle_ReasonCode_Stolen_MapsToLostTransactionType()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = CreateInventoryItem(itemId, quantity: 10m, reservedQuantity: 0m);

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 1m,
            IsIncrease: false,
            ReasonCode: "Stolen",
            Notes: null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Type == InventoryTransactionType.Lost)), Times.Once);
    }

    [Fact]
    public async Task Handle_NotesProvided_UsesNotesInTransaction()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = CreateInventoryItem(itemId, quantity: 10m, reservedQuantity: 0m);
        const string expectedNotes = "Manual count correction";

        _inventoryRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var command = new AdjustStockCommand(
            InventoryItemId: itemId,
            Quantity: 5m,
            IsIncrease: true,
            ReasonCode: "Other",
            Notes: expectedNotes);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _inventoryRepositoryMock.Verify(x => x.AddTransaction(It.Is<InventoryTransaction>(t =>
            t.Notes == expectedNotes)), Times.Once);
    }

    private InventoryItem CreateInventoryItem(Guid id, decimal quantity, decimal reservedQuantity) =>
        new()
        {
            Id = id,
            TenantId = _tenantId,
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = quantity,
            ReservedQuantity = reservedQuantity,
            MinimumLevel = 0
        };
}
