using Corelio.Application.Sales.Queries.GetSales;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Sales.Queries;

[Trait("Category", "Unit")]
public class GetSalesQueryHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly GetSalesQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetSalesQueryHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _handler = new GetSalesQueryHandler(_saleRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithDefaultParameters_ReturnsPagedResult()
    {
        // Arrange
        var sales = CreateSaleList(5);

        _saleRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((sales, 5));

        var query = new GetSalesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(5);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(20);
        result.Value.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_PassesSearchTermToRepository()
    {
        // Arrange
        var sales = CreateSaleList(2);

        _saleRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, "V-001", null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((sales, 2));

        var query = new GetSalesQuery(SearchTerm: "V-001");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        _saleRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, null, "V-001", null, null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_PassesStatusToRepository()
    {
        // Arrange
        var sales = CreateSaleList(3);

        _saleRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, SaleStatus.Completed, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((sales, 3));

        var query = new GetSalesQuery(Status: SaleStatus.Completed);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _saleRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, SaleStatus.Completed, null, null, null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDateRange_PassesDateRangeToRepository()
    {
        // Arrange
        var sales = CreateSaleList(4);
        var dateFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTo = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc);

        _saleRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, null, dateFrom, dateTo, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((sales, 4));

        var query = new GetSalesQuery(DateFrom: dateFrom, DateTo: dateTo);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _saleRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, null, null, dateFrom, dateTo, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoSales_ReturnsEmptyPagedResult()
    {
        // Arrange
        _saleRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Sale>(), 0));

        var query = new GetSalesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
        result.Value.TotalPages.Should().Be(0);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithPagination_CalculatesCorrectMetadata()
    {
        // Arrange
        var sales = CreateSaleList(10);

        _saleRepositoryMock.Setup(x => x.GetPagedAsync(
                2, 10, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((sales, 35));

        var query = new GetSalesQuery(PageNumber: 2, PageSize: 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(2);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(35);
        result.Value.TotalPages.Should().Be(4); // ceil(35/10) = 4
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MapsCustomerNameFromNavigationProperty()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Folio = "V-00001",
            Type = SaleType.Pos,
            Status = SaleStatus.Completed,
            WarehouseId = warehouseId,
            SubTotal = 100m,
            Total = 116m,
            Customer = new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                FirstName = "Juan",
                LastName = "García",
                CustomerType = CustomerType.Individual
            },
            Items = []
        };

        _saleRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(([sale], 1));

        var query = new GetSalesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].CustomerName.Should().Be("Juan García");
        result.Value.Items[0].Folio.Should().Be("V-00001");
        result.Value.Items[0].Status.Should().Be(SaleStatus.Completed);
    }

    private List<Sale> CreateSaleList(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Sale
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Folio = $"V-{i:D5}",
                Type = SaleType.Pos,
                Status = SaleStatus.Completed,
                WarehouseId = Guid.NewGuid(),
                SubTotal = 100m * i,
                Total = 116m * i,
                Items = []
            })
            .ToList();
    }
}
