using Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrders;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.PurchaseOrders.Queries;

[Trait("Category", "Unit")]
public class GetPurchaseOrdersQueryHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _repoMock;
    private readonly GetPurchaseOrdersQueryHandler _handler;

    public GetPurchaseOrdersQueryHandlerTests()
    {
        _repoMock = new Mock<IPurchaseOrderRepository>();
        _handler = new GetPurchaseOrdersQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsMappedPagedResult()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier { Id = supplierId, Name = "Acero del Norte" };
        var po = new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            OrderNumber = "PO-2026-0001",
            SupplierId = supplierId,
            Supplier = supplier,
            Status = PurchaseOrderStatus.Draft,
            Total = 1160m,
            CreatedAt = DateTime.UtcNow,
            Items = []
        };

        _repoMock.Setup(x => x.GetPagedAsync(1, 20, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(([po], 1));

        var query = new GetPurchaseOrdersQuery(1, 20, null, null, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].OrderNumber.Should().Be("PO-2026-0001");
        result.Value.Items[0].SupplierName.Should().Be("Acero del Norte");
        result.Value.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithEmptyResult_ReturnsEmptyPagedList()
    {
        // Arrange
        _repoMock.Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<PurchaseOrderStatus?>(), It.IsAny<Guid?>(),
                It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<PurchaseOrder>(), 0));

        var query = new GetPurchaseOrdersQuery(1, 20, null, null, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
}
