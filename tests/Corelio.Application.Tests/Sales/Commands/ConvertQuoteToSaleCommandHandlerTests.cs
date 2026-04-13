using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Commands.ConvertQuoteToSale;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Sales.Commands;

[Trait("Category", "Unit")]
public class ConvertQuoteToSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ConvertQuoteToSaleCommandHandler _handler;

    public ConvertQuoteToSaleCommandHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new ConvertQuoteToSaleCommandHandler(
            _saleRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidOpenQuote_ReturnsSuccessAndMarksCancelled()
    {
        // Arrange
        var quoteId = Guid.NewGuid();
        var quote = CreateQuote(quoteId, SaleStatus.Draft);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(quoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var command = new ConvertQuoteToSaleCommand(quoteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        quote.Status.Should().Be(SaleStatus.Cancelled);
        quote.Notes.Should().Contain("Convertida a venta");
        _saleRepositoryMock.Verify(x => x.Update(quote), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingNotes_AppendsConversionNote()
    {
        // Arrange
        var quoteId = Guid.NewGuid();
        var quote = CreateQuote(quoteId, SaleStatus.Draft);
        quote.Notes = "Cliente especial";

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(quoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var command = new ConvertQuoteToSaleCommand(quoteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        quote.Notes.Should().Be("Cliente especial | Convertida a venta");
    }

    [Fact]
    public async Task Handle_WhenQuoteNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var quoteId = Guid.NewGuid();

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(quoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new ConvertQuoteToSaleCommand(quoteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Quote.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaleIsNotAQuoteType_ReturnsValidationError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            Folio = "V-00001",
            Type = SaleType.Pos, // POS sale, not a Quote
            Status = SaleStatus.Completed
        };

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new ConvertQuoteToSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Quote.InvalidType");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenQuoteAlreadyCancelled_ReturnsConflictError()
    {
        // Arrange
        var quoteId = Guid.NewGuid();
        var quote = CreateQuote(quoteId, SaleStatus.Cancelled);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(quoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var command = new ConvertQuoteToSaleCommand(quoteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Quote.AlreadyCancelled");
        result.Error.Type.Should().Be(ErrorType.Conflict);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenQuoteIsExpired_ReturnsValidationError()
    {
        // Arrange
        var quoteId = Guid.NewGuid();
        var quote = CreateQuote(quoteId, SaleStatus.Draft);
        quote.ExpiresAt = DateTime.UtcNow.AddDays(-1); // Expired yesterday

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(quoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var command = new ConvertQuoteToSaleCommand(quoteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Quote.Expired");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenQuoteExpiresInFuture_Succeeds()
    {
        // Arrange
        var quoteId = Guid.NewGuid();
        var quote = CreateQuote(quoteId, SaleStatus.Draft);
        quote.ExpiresAt = DateTime.UtcNow.AddDays(7); // Expires in a week

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(quoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var command = new ConvertQuoteToSaleCommand(quoteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        quote.Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_WhenQuoteHasNoExpiryDate_Succeeds()
    {
        // Arrange
        var quoteId = Guid.NewGuid();
        var quote = CreateQuote(quoteId, SaleStatus.Draft);
        quote.ExpiresAt = null; // No expiry

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(quoteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        var command = new ConvertQuoteToSaleCommand(quoteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        quote.Status.Should().Be(SaleStatus.Cancelled);
    }

    private static Sale CreateQuote(Guid id, SaleStatus status) =>
        new()
        {
            Id = id,
            Folio = "C-00001",
            Type = SaleType.Quote,
            Status = status
        };
}
