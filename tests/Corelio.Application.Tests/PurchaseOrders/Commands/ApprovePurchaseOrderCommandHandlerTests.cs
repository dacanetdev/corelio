using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.PurchaseOrders.Commands;

[Trait("Category", "Unit")]
public class ApprovePurchaseOrderCommandHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _repoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ITenantService> _tenantMock;
    private readonly ApprovePurchaseOrderCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public ApprovePurchaseOrderCommandHandlerTests()
    {
        _repoMock = new Mock<IPurchaseOrderRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _tenantMock = new Mock<ITenantService>();
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        _handler = new ApprovePurchaseOrderCommandHandler(
            _repoMock.Object,
            _uowMock.Object,
            _tenantMock.Object);
    }

    [Fact]
    public async Task Handle_WithSubmittedOrder_TransitionsToApproved()
    {
        // Arrange
        var po = CreateOrderWithStatus(PurchaseOrderStatus.Submitted);
        _repoMock.Setup(x => x.GetByIdAsync(po.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);

        // Act
        var result = await _handler.Handle(new ApprovePurchaseOrderCommand(po.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        po.Status.Should().Be(PurchaseOrderStatus.Approved);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder?)null);

        // Act
        var result = await _handler.Handle(new ApprovePurchaseOrderCommand(id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrderIsDraftNotSubmitted_ReturnsConflictError()
    {
        // Arrange
        var po = CreateOrderWithStatus(PurchaseOrderStatus.Draft);
        _repoMock.Setup(x => x.GetByIdAsync(po.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);

        // Act
        var result = await _handler.Handle(new ApprovePurchaseOrderCommand(po.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("PurchaseOrder.InvalidTransition");
        po.Status.Should().Be(PurchaseOrderStatus.Draft); // unchanged
    }

    private PurchaseOrder CreateOrderWithStatus(PurchaseOrderStatus status) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = _tenantId,
        OrderNumber = "PO-2026-0001",
        SupplierId = Guid.NewGuid(),
        Status = status,
        Items = []
    };
}
