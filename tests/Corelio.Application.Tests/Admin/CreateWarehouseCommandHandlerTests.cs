using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Commands.CreateWarehouse;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Admin;

[Trait("Category", "Unit")]
public class CreateWarehouseCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly CreateWarehouseCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CreateWarehouseCommandHandlerTests()
    {
        _inventoryRepoMock = new Mock<IInventoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new CreateWarehouseCommandHandler(
            _inventoryRepoMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsWarehouseId()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var command = new CreateWarehouseCommand("Almacén Norte", WarehouseType.Secondary, false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _inventoryRepoMock.Verify(x => x.AddWarehouseAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(
            new CreateWarehouseCommand("Almacén", WarehouseType.Main, false), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _inventoryRepoMock.Verify(x => x.AddWarehouseAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIsDefaultTrue_UnsetsExistingDefault()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var command = new CreateWarehouseCommand("Nuevo Principal", WarehouseType.Main, true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _inventoryRepoMock.Verify(x => x.UnsetDefaultWarehouseAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIsDefaultFalse_DoesNotUnsetExistingDefault()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var command = new CreateWarehouseCommand("Almacén Secundario", WarehouseType.Secondary, false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _inventoryRepoMock.Verify(x => x.UnsetDefaultWarehouseAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
