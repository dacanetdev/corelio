using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.PurchaseOrders.Queries;

[Trait("Category", "Unit")]
public class GetPurchaseOrderByIdQueryHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _repoMock;
    private readonly GetPurchaseOrderByIdQueryHandler _handler;

    public GetPurchaseOrderByIdQueryHandlerTests()
    {
        _repoMock = new Mock<IPurchaseOrderRepository>();
        _handler = new GetPurchaseOrderByIdQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsMappedDto()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var po = new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            OrderNumber = "PO-2026-0001",
            SupplierId = supplierId,
            Supplier = new Supplier { Id = supplierId, Name = "Proveedor SA" },
            Status = PurchaseOrderStatus.Approved,
            ExpectedDate = DateTimeOffset.UtcNow.AddDays(7),
            Notes = "Urgent",
            Subtotal = 1000m,
            IvaAmount = 160m,
            Total = 1160m,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ProductName = "Tornillo 1/4",
                    Quantity = 100m,
                    UnitPrice = 10m,
                    Subtotal = 1000m,
                    ReceivedQuantity = 0m
                }
            ]
        };

        _repoMock.Setup(x => x.GetByIdAsync(po.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(po);

        // Act
        var result = await _handler.Handle(new GetPurchaseOrderByIdQuery(po.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.OrderNumber.Should().Be("PO-2026-0001");
        result.Value.SupplierName.Should().Be("Proveedor SA");
        result.Value.Status.Should().Be(PurchaseOrderStatus.Approved);
        result.Value.Total.Should().Be(1160m);
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].ProductName.Should().Be("Tornillo 1/4");
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder?)null);

        // Act
        var result = await _handler.Handle(new GetPurchaseOrderByIdQuery(id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("PurchaseOrder.NotFound");
    }
}
