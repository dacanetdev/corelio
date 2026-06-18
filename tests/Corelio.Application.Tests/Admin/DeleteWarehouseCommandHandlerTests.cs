using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Commands.DeleteWarehouse;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Admin;

[Trait("Category", "Unit")]
public class DeleteWarehouseCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteWarehouseCommandHandler _handler;

    public DeleteWarehouseCommandHandlerTests()
    {
        _inventoryRepoMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteWarehouseCommandHandler(_inventoryRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidNonDefaultWarehouse_Deletes()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse
        {
            Id = warehouseId,
            TenantId = Guid.NewGuid(),
            Name = "Almacén Secundario",
            Type = WarehouseType.Secondary,
            IsDefault = false
        };

        _inventoryRepoMock.Setup(x => x.GetWarehouseByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
        _inventoryRepoMock.Setup(x => x.WarehouseHasInventoryAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(new DeleteWarehouseCommand(warehouseId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _inventoryRepoMock.Verify(x => x.DeleteWarehouse(warehouse), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenWarehouseNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        _inventoryRepoMock.Setup(x => x.GetWarehouseByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        // Act
        var result = await _handler.Handle(new DeleteWarehouseCommand(warehouseId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Warehouse.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenWarehouseIsDefault_ReturnsConflictError()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse
        {
            Id = warehouseId,
            TenantId = Guid.NewGuid(),
            Name = "Almacén Principal",
            Type = WarehouseType.Main,
            IsDefault = true
        };

        _inventoryRepoMock.Setup(x => x.GetWarehouseByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        // Act
        var result = await _handler.Handle(new DeleteWarehouseCommand(warehouseId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Warehouse.IsDefault");
        result.Error.Type.Should().Be(ErrorType.Conflict);
        _inventoryRepoMock.Verify(x => x.DeleteWarehouse(It.IsAny<Warehouse>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenWarehouseHasInventory_ReturnsConflictError()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse
        {
            Id = warehouseId,
            TenantId = Guid.NewGuid(),
            Name = "Almacén Con Stock",
            Type = WarehouseType.Secondary,
            IsDefault = false
        };

        _inventoryRepoMock.Setup(x => x.GetWarehouseByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
        _inventoryRepoMock.Setup(x => x.WarehouseHasInventoryAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(new DeleteWarehouseCommand(warehouseId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Warehouse.HasInventory");
        result.Error.Type.Should().Be(ErrorType.Conflict);
        _inventoryRepoMock.Verify(x => x.DeleteWarehouse(It.IsAny<Warehouse>()), Times.Never);
    }
}
