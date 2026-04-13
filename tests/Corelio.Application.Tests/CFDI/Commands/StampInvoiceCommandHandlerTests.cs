using Corelio.Application.CFDI.Commands.StampInvoice;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.CFDI.Commands;

[Trait("Category", "Unit")]
public class StampInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<ICFDIService> _cfdiServiceMock;
    private readonly StampInvoiceCommandHandler _handler;

    public StampInvoiceCommandHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _cfdiServiceMock = new Mock<ICFDIService>();

        _handler = new StampInvoiceCommandHandler(
            _invoiceRepositoryMock.Object,
            _cfdiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithDraftInvoice_StampsAndReturnsUuid()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var expectedUuid = "6128396F-CFFB-4A1D-A5C5-6B9B0E1234AB";
        var invoice = CreateInvoice(invoiceId, CfdiStatus.Draft);

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _cfdiServiceMock.Setup(x => x.StampAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUuid);

        var command = new StampInvoiceCommand(invoiceId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedUuid);
        _cfdiServiceMock.Verify(x => x.StampAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var command = new StampInvoiceCommand(invoiceId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _cfdiServiceMock.Verify(x => x.StampAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInvoiceAlreadyStamped_ReturnsValidationError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateInvoice(invoiceId, CfdiStatus.Stamped);

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var command = new StampInvoiceCommand(invoiceId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.NotDraft");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _cfdiServiceMock.Verify(x => x.StampAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInvoiceCancelled_ReturnsValidationError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateInvoice(invoiceId, CfdiStatus.Cancelled);

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var command = new StampInvoiceCommand(invoiceId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.NotDraft");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WhenCfdiServiceThrows_ReturnsFailureError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateInvoice(invoiceId, CfdiStatus.Draft);

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _cfdiServiceMock.Setup(x => x.StampAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("PAC connection refused"));

        var command = new StampInvoiceCommand(invoiceId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.StampFailed");
        result.Error.Type.Should().Be(ErrorType.Failure);
        result.Error.Message.Should().Be("PAC connection refused");
    }

    private static Invoice CreateInvoice(Guid id, CfdiStatus status) =>
        new()
        {
            Id = id,
            Folio = "F-00001",
            Status = status
        };
}
