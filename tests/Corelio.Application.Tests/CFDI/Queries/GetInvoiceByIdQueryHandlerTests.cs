using Corelio.Application.CFDI.Queries.GetInvoiceById;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.CFDI.Queries;

[Trait("Category", "Unit")]
public class GetInvoiceByIdQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly GetInvoiceByIdQueryHandler _handler;

    public GetInvoiceByIdQueryHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new GetInvoiceByIdQueryHandler(_invoiceRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenInvoiceExists_ReturnsDtoWithAllFields()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var saleId = Guid.NewGuid();
        var createdAt = new DateTime(2025, 3, 15, 9, 0, 0, DateTimeKind.Utc);

        var invoice = new Invoice
        {
            Id = invoiceId,
            SaleId = saleId,
            Folio = "F-00001",
            Serie = "A",
            Uuid = "6128396F-CFFB-4A1D-A5C5-6B9B0E1234AB",
            Status = CfdiStatus.Stamped,
            InvoiceType = CfdiType.Ingreso,
            IssuerRfc = "ABC123456789",
            IssuerName = "Ferretería ABC",
            IssuerTaxRegime = "601",
            ReceiverRfc = "XAXX010101000",
            ReceiverName = "Público General",
            ReceiverTaxRegime = "616",
            ReceiverPostalCode = "06600",
            ReceiverCfdiUse = "G01",
            Subtotal = 1000m,
            Discount = 0m,
            Total = 1160m,
            PaymentForm = "01",
            PaymentMethod = "PUE",
            StampDate = createdAt.AddHours(1),
            SatCertificateNumber = "12345678901234567890",
            QrCodeData = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx",
            CancellationReason = null,
            CancellationDate = null,
            CreatedAt = createdAt,
            Items = [
                new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    ItemNumber = 1,
                    ProductKey = "25171500",
                    UnitKey = "H87",
                    Description = "Perno 1/4\"",
                    Quantity = 10m,
                    UnitValue = 100m,
                    Amount = 1000m,
                    Discount = 0m,
                    TaxRate = 0.16m,
                    TaxAmount = 160m
                }
            ]
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var query = new GetInvoiceByIdQuery(invoiceId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var dto = result.Value!;
        dto.Id.Should().Be(invoiceId);
        dto.SaleId.Should().Be(saleId);
        dto.Folio.Should().Be("F-00001");
        dto.Serie.Should().Be("A");
        dto.Uuid.Should().Be("6128396F-CFFB-4A1D-A5C5-6B9B0E1234AB");
        dto.Status.Should().Be(CfdiStatus.Stamped);
        dto.InvoiceType.Should().Be(CfdiType.Ingreso);
        dto.IssuerRfc.Should().Be("ABC123456789");
        dto.ReceiverRfc.Should().Be("XAXX010101000");
        dto.ReceiverCfdiUse.Should().Be("G01");
        dto.Subtotal.Should().Be(1000m);
        dto.Total.Should().Be(1160m);
        dto.PaymentForm.Should().Be("01");
        dto.PaymentMethod.Should().Be("PUE");
        dto.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var query = new GetInvoiceByIdQuery(invoiceId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WithMultipleItems_ReturnsItemsOrderedByItemNumber()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateMinimalInvoice(invoiceId);

        // Add items out of order
        invoice.Items = [
            new InvoiceItem { Id = Guid.NewGuid(), ItemNumber = 3, Description = "Producto C", Quantity = 1, UnitValue = 10m, Amount = 10m },
            new InvoiceItem { Id = Guid.NewGuid(), ItemNumber = 1, Description = "Producto A", Quantity = 1, UnitValue = 10m, Amount = 10m },
            new InvoiceItem { Id = Guid.NewGuid(), ItemNumber = 2, Description = "Producto B", Quantity = 1, UnitValue = 10m, Amount = 10m }
        ];

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var query = new GetInvoiceByIdQuery(invoiceId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var items = result.Value!.Items;
        items.Should().HaveCount(3);
        items[0].ItemNumber.Should().Be(1);
        items[0].Description.Should().Be("Producto A");
        items[1].ItemNumber.Should().Be(2);
        items[2].ItemNumber.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WithNoItems_ReturnsDtoWithEmptyItemsList()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateMinimalInvoice(invoiceId);
        invoice.Items = [];

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var query = new GetInvoiceByIdQuery(invoiceId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithCancelledInvoice_ReturnsCancellationFields()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var cancellationDate = DateTime.UtcNow.AddHours(-1);
        var invoice = CreateMinimalInvoice(invoiceId);
        invoice.Status = CfdiStatus.Cancelled;
        invoice.CancellationReason = "01";
        invoice.CancellationDate = cancellationDate;

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var query = new GetInvoiceByIdQuery(invoiceId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.CancellationReason.Should().Be("01");
        result.Value!.CancellationDate.Should().Be(cancellationDate);
        result.Value!.Status.Should().Be(CfdiStatus.Cancelled);
    }

    private static Invoice CreateMinimalInvoice(Guid id) =>
        new()
        {
            Id = id,
            Folio = "F-00001",
            Serie = "A",
            Status = CfdiStatus.Draft,
            InvoiceType = CfdiType.Ingreso,
            IssuerRfc = "ABC123",
            IssuerName = "Empresa",
            IssuerTaxRegime = "601",
            ReceiverRfc = "XAXX010101000",
            ReceiverName = "Receptor",
            ReceiverTaxRegime = "616",
            ReceiverPostalCode = "06600",
            ReceiverCfdiUse = "G01",
            PaymentForm = "01",
            PaymentMethod = "PUE",
            CreatedAt = DateTime.UtcNow
        };
}
