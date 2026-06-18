using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Commands.UpdateWarehouse;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Admin;

[Trait("Category", "Unit")]
public class UpdateWarehouseCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateWarehouseCommandHandler _handler;

    public UpdateWarehouseCommandHandlerTests()
    {
        _inventoryRepoMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateWarehouseCommandHandler(_inventoryRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_UpdatesWarehouse()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse
        {
            Id = warehouseId,
            TenantId = Guid.NewGuid(),
            Name = "Old Name",
            Type = WarehouseType.Secondary,
            IsDefault = false
        };

        _inventoryRepoMock.Setup(x => x.GetWarehouseByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        var command = new UpdateWarehouseCommand(warehouseId, "New Name", WarehouseType.Retail, false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        warehouse.Name.Should().Be("New Name");
        warehouse.Type.Should().Be(WarehouseType.Retail);
        _inventoryRepoMock.Verify(x => x.UpdateWarehouse(warehouse), Times.Once);
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
        var result = await _handler.Handle(
            new UpdateWarehouseCommand(warehouseId, "Name", WarehouseType.Main, false), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Warehouse.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSettingAsDefault_UnsetsExistingDefault()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse
        {
            Id = warehouseId,
            TenantId = Guid.NewGuid(),
            Name = "Almacén",
            Type = WarehouseType.Secondary,
            IsDefault = false
        };

        _inventoryRepoMock.Setup(x => x.GetWarehouseByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        // Act
        await _handler.Handle(
            new UpdateWarehouseCommand(warehouseId, "Almacén", WarehouseType.Secondary, true), CancellationToken.None);

        // Assert
        _inventoryRepoMock.Verify(x => x.UnsetDefaultWarehouseAsync(It.IsAny<CancellationToken>()), Times.Once);
        warehouse.IsDefault.Should().BeTrue();
    }
}
