using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using Corelio.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.PurchaseOrders.Commands;

[Trait("Category", "Unit")]
public class UpdatePurchaseOrderCommandHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _repoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ITenantService> _tenantMock;
    private readonly UpdatePurchaseOrderCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public UpdatePurchaseOrderCommandHandlerTests()
    {
        _repoMock = new Mock<IPurchaseOrderRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _tenantMock = new Mock<ITenantService>();

        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        _handler = new UpdatePurchaseOrderCommandHandler(
            _repoMock.Object,
            _uowMock.Object,
            _tenantMock.Object);
    }

    [Fact]
    public async Task Handle_WithDraftOrder_UpdatesAndReturnsSuccess()
    {
        // Arrange
        var po = CreateDraftOrder();
        _repoMock.Setup(x => x.GetByIdAsync(po.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);

        var newSupplierId = Guid.NewGuid();
        var command = new UpdatePurchaseOrderCommand(
            po.Id,
            newSupplierId,
            ExpectedDate: null,
            Notes: "Updated notes",
            Items: [new PurchaseOrderItemRequest(Guid.NewGuid(), "Updated Product", 2m, 150m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        po.SupplierId.Should().Be(newSupplierId);
        po.Notes.Should().Be("Updated notes");
        _repoMock.Verify(x => x.Update(po), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder?)null);

        var command = new UpdatePurchaseOrderCommand(
            id, Guid.NewGuid(), null, null,
            [new PurchaseOrderItemRequest(Guid.NewGuid(), "P", 1m, 1m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrderNotInDraft_ReturnsConflictError()
    {
        // Arrange
        var po = CreateDraftOrder();
        po.Status = PurchaseOrderStatus.Submitted;
        _repoMock.Setup(x => x.GetByIdAsync(po.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);

        var command = new UpdatePurchaseOrderCommand(
            po.Id, Guid.NewGuid(), null, null,
            [new PurchaseOrderItemRequest(Guid.NewGuid(), "P", 1m, 1m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("PurchaseOrder.InvalidTransition");
        _repoMock.Verify(x => x.Update(It.IsAny<PurchaseOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RecalculatesTotalsOnUpdate()
    {
        // Arrange
        var po = CreateDraftOrder();
        _repoMock.Setup(x => x.GetByIdAsync(po.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);

        var command = new UpdatePurchaseOrderCommand(
            po.Id,
            Guid.NewGuid(),
            null,
            null,
            Items: [new PurchaseOrderItemRequest(Guid.NewGuid(), "New Product", 10m, 100m)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        po.Subtotal.Should().Be(1000m);
        po.IvaAmount.Should().Be(160m);
        po.Total.Should().Be(1160m);
    }

    private PurchaseOrder CreateDraftOrder() => new()
    {
        Id = Guid.NewGuid(),
        TenantId = _tenantId,
        OrderNumber = "PO-2026-0001",
        SupplierId = Guid.NewGuid(),
        Status = PurchaseOrderStatus.Draft,
        Subtotal = 500m,
        IvaAmount = 80m,
        Total = 580m,
        Items = []
    };
}
