using Corelio.Application.CFDI.Commands.CancelInvoice;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.CFDI.Commands;

[Trait("Category", "Unit")]
public class CancelInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<ICFDIService> _cfdiServiceMock;
    private readonly CancelInvoiceCommandHandler _handler;

    public CancelInvoiceCommandHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _cfdiServiceMock = new Mock<ICFDIService>();

        _handler = new CancelInvoiceCommandHandler(
            _invoiceRepositoryMock.Object,
            _cfdiServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidStampedInvoiceWithin72Hours_CancelsSuccessfully()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateStampedInvoice(invoiceId, stampedHoursAgo: 24);

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _cfdiServiceMock.Setup(x => x.CancelAsync(invoiceId, "01", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CancelInvoiceCommand(invoiceId, "01");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _cfdiServiceMock.Verify(x => x.CancelAsync(invoiceId, "01", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("01")]
    [InlineData("02")]
    [InlineData("03")]
    [InlineData("04")]
    public async Task Handle_WithAllValidReasonCodes_Succeeds(string reason)
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateStampedInvoice(invoiceId, stampedHoursAgo: 1);

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _cfdiServiceMock.Setup(x => x.CancelAsync(invoiceId, reason, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CancelInvoiceCommand(invoiceId, reason);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("00")]
    [InlineData("05")]
    [InlineData("")]
    [InlineData("INVALID")]
    public async Task Handle_WithInvalidReasonCode_ReturnsValidationError(string invalidReason)
    {
        // Arrange
        var command = new CancelInvoiceCommand(Guid.NewGuid(), invalidReason);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.InvalidCancellationReason");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _invoiceRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var command = new CancelInvoiceCommand(invoiceId, "01");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _cfdiServiceMock.Verify(x => x.CancelAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInvoiceNotStamped_ReturnsValidationError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var draftInvoice = new Invoice
        {
            Id = invoiceId,
            Folio = "F-00001",
            Status = CfdiStatus.Draft // Not stamped
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(draftInvoice);

        var command = new CancelInvoiceCommand(invoiceId, "01");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.NotStamped");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _cfdiServiceMock.Verify(x => x.CancelAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCancellationWindowExpired_ReturnsValidationError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateStampedInvoice(invoiceId, stampedHoursAgo: 73); // Beyond 72-hour window

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var command = new CancelInvoiceCommand(invoiceId, "01");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.CancellationWindowExpired");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _cfdiServiceMock.Verify(x => x.CancelAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AtExactly72Hours_StillWithinCancellationWindow()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateStampedInvoice(invoiceId, stampedHoursAgo: 71); // Still within window

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _cfdiServiceMock.Setup(x => x.CancelAsync(invoiceId, "02", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CancelInvoiceCommand(invoiceId, "02");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenCfdiServiceThrows_ReturnsFailureError()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = CreateStampedInvoice(invoiceId, stampedHoursAgo: 2);

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _cfdiServiceMock.Setup(x => x.CancelAsync(invoiceId, "01", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SAT service unavailable"));

        var command = new CancelInvoiceCommand(invoiceId, "01");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.CancelFailed");
        result.Error.Type.Should().Be(ErrorType.Failure);
        result.Error.Message.Should().Be("SAT service unavailable");
    }

    private static Invoice CreateStampedInvoice(Guid id, double stampedHoursAgo) =>
        new()
        {
            Id = id,
            Folio = "F-00001",
            Status = CfdiStatus.Stamped,
            Uuid = "6128396F-CFFB-4A1D-A5C5-6B9B0E1234AB",
            StampDate = DateTime.UtcNow.AddHours(-stampedHoursAgo)
        };
}
