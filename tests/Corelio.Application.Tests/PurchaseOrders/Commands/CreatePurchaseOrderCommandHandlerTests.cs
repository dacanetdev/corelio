using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.PurchaseOrders.Commands;

[Trait("Category", "Unit")]
public class CreatePurchaseOrderCommandHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _repoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ITenantService> _tenantMock;
    private readonly CreatePurchaseOrderCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CreatePurchaseOrderCommandHandlerTests()
    {
        _repoMock = new Mock<IPurchaseOrderRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _tenantMock = new Mock<ITenantService>();

        _repoMock.Setup(x => x.GetNextSequenceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new CreatePurchaseOrderCommandHandler(
            _repoMock.Object,
            _uowMock.Object,
            _tenantMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessWithId()
    {
        // Arrange
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _repoMock.Verify(x => x.Add(It.IsAny<PurchaseOrder>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _repoMock.Verify(x => x.Add(It.IsAny<PurchaseOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CalculatesTotalsCorrectly()
    {
        // Arrange
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        PurchaseOrder? captured = null;
        _repoMock.Setup(x => x.Add(It.IsAny<PurchaseOrder>()))
            .Callback<PurchaseOrder>(po => captured = po);

        var command = new CreatePurchaseOrderCommand(
            SupplierId: Guid.NewGuid(),
            ExpectedDate: null,
            Notes: null,
            Items:
            [
                new PurchaseOrderItemRequest(Guid.NewGuid(), "Product A", 10m, 100m),  // subtotal 1000
                new PurchaseOrderItemRequest(Guid.NewGuid(), "Product B", 5m, 200m)    // subtotal 1000
            ]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        captured!.Subtotal.Should().Be(2000m);
        captured.IvaAmount.Should().Be(320m);   // 2000 * 0.16
        captured.Total.Should().Be(2320m);
    }

    [Fact]
    public async Task Handle_GeneratesOrderNumberInCorrectFormat()
    {
        // Arrange
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _repoMock.Setup(x => x.GetNextSequenceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        PurchaseOrder? captured = null;
        _repoMock.Setup(x => x.Add(It.IsAny<PurchaseOrder>()))
            .Callback<PurchaseOrder>(po => captured = po);

        // Act
        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var expectedYear = DateTimeOffset.UtcNow.Year;
        captured!.OrderNumber.Should().Be($"PO-{expectedYear}-0042");
    }

    [Fact]
    public async Task Handle_SetsDraftStatusOnNewOrder()
    {
        // Arrange
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        PurchaseOrder? captured = null;
        _repoMock.Setup(x => x.Add(It.IsAny<PurchaseOrder>()))
            .Callback<PurchaseOrder>(po => captured = po);

        // Act
        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        captured!.Status.Should().Be(PurchaseOrderStatus.Draft);
        captured.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task Handle_SetsCorrectSubtotalOnEachItem()
    {
        // Arrange
        _tenantMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        PurchaseOrder? captured = null;
        _repoMock.Setup(x => x.Add(It.IsAny<PurchaseOrder>()))
            .Callback<PurchaseOrder>(po => captured = po);

        var productId = Guid.NewGuid();
        var command = new CreatePurchaseOrderCommand(
            SupplierId: Guid.NewGuid(),
            ExpectedDate: null,
            Notes: null,
            Items: [new PurchaseOrderItemRequest(productId, "Widget", 3m, 50m)]);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        captured!.Items.Should().HaveCount(1);
        captured.Items.First().Subtotal.Should().Be(150m);
    }

    private static CreatePurchaseOrderCommand CreateValidCommand()
    {
        return new CreatePurchaseOrderCommand(
            SupplierId: Guid.NewGuid(),
            ExpectedDate: null,
            Notes: null,
            Items: [new PurchaseOrderItemRequest(Guid.NewGuid(), "Test Product", 1m, 100m)]);
    }
}
