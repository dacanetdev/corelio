using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Commands.CancelSale;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Sales.Commands;

[Trait("Category", "Unit")]
public class CancelSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CancelSaleCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CancelSaleCommandHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CancelSaleCommandHandler(
            _saleRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithDraftSale_SetsCancelledStatus()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        sale.Status.Should().Be(SaleStatus.Cancelled);
        _saleRepositoryMock.Verify(x => x.Update(sale), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithReason_AppendsCancellationNotes()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);
        sale.Notes = "Original notes";

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId, Reason: "Customer changed mind");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Notes.Should().Contain("Customer changed mind");
        sale.Notes.Should().Contain("Cancelled:");
    }

    [Fact]
    public async Task Handle_WithReasonAndNoExistingNotes_SetsOnlyCancellationNotes()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);
        sale.Notes = null;

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId, Reason: "Test reason");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Notes.Should().Be("Cancelled: Test reason");
    }

    [Fact]
    public async Task Handle_WithNoReason_DoesNotModifyNotes()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Draft);
        sale.Notes = "Original notes";

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sale.Notes.Should().Be("Original notes");
    }

    [Fact]
    public async Task Handle_WhenSaleNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _saleRepositoryMock.Verify(x => x.Update(It.IsAny<Sale>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaleAlreadyCancelled_ReturnsConflictError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Cancelled);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.AlreadyCancelled");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_WhenSaleCompleted_ReturnsConflictError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateSale(saleId, SaleStatus.Completed);

        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.CannotCancelCompleted");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    private Sale CreateSale(Guid id, SaleStatus status) =>
        new()
        {
            Id = id,
            TenantId = _tenantId,
            Folio = "V-00001",
            Type = SaleType.Pos,
            Status = status,
            WarehouseId = Guid.NewGuid(),
            SubTotal = 100m,
            Total = 100m,
            Items = []
        };
}
