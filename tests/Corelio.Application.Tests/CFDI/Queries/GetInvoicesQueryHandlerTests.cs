using Corelio.Application.CFDI.Queries.GetInvoices;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.CFDI.Queries;

[Trait("Category", "Unit")]
public class GetInvoicesQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly GetInvoicesQueryHandler _handler;

    public GetInvoicesQueryHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new GetInvoicesQueryHandler(_invoiceRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithInvoices_ReturnsPagedResultWithCorrectDtos()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            CreateInvoice("F-00001", "XAXX010101000", "Público en General", 1160m, CfdiStatus.Draft),
            CreateInvoice("F-00002", "ABC123456789", "Empresa ABC", 2320m, CfdiStatus.Stamped)
        };

        _invoiceRepositoryMock.Setup(x => x.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((invoices, 2));

        var query = new GetInvoicesQuery(PageNumber: 1, PageSize: 20);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value!.TotalCount.Should().Be(2);
        result.Value!.PageNumber.Should().Be(1);
        result.Value!.PageSize.Should().Be(20);

        var first = result.Value!.Items[0];
        first.Folio.Should().Be("F-00001");
        first.ReceiverRfc.Should().Be("XAXX010101000");
        first.Total.Should().Be(1160m);
        first.Status.Should().Be(CfdiStatus.Draft);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_PassesFilterToRepository()
    {
        // Arrange
        _invoiceRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, CfdiStatus.Stamped, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Invoice>(), 0));

        var query = new GetInvoicesQuery(PageNumber: 1, PageSize: 20, Status: CfdiStatus.Stamped);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _invoiceRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, CfdiStatus.Stamped, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_PassesSearchTermToRepository()
    {
        // Arrange
        _invoiceRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, "ABC", It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Invoice>(), 0));

        var query = new GetInvoicesQuery(PageNumber: 1, PageSize: 20, SearchTerm: "ABC");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _invoiceRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, null, "ABC", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyRepository_ReturnsEmptyPagedResult()
    {
        // Arrange
        _invoiceRepositoryMock.Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CfdiStatus?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Invoice>(), 0));

        var query = new GetInvoicesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value!.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MapsStampDateCorrectly()
    {
        // Arrange
        var stampDate = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        var invoice = CreateInvoice("F-00003", "RFC123", "Empresa X", 500m, CfdiStatus.Stamped);
        invoice.StampDate = stampDate;
        invoice.Uuid = "ABC-UUID-123";

        _invoiceRepositoryMock.Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CfdiStatus?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(([invoice], 1));

        var query = new GetInvoicesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Value!.Items[0];
        dto.StampDate.Should().Be(stampDate);
        dto.Uuid.Should().Be("ABC-UUID-123");
    }

    private static Invoice CreateInvoice(string folio, string receiverRfc, string receiverName, decimal total, CfdiStatus status) =>
        new()
        {
            Id = Guid.NewGuid(),
            Folio = folio,
            Serie = "A",
            ReceiverRfc = receiverRfc,
            ReceiverName = receiverName,
            Total = total,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
}
