using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Commands.UpdateSupplier;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Suppliers;

[Trait("Category", "Unit")]
public class UpdateSupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly UpdateSupplierCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public UpdateSupplierCommandHandlerTests()
    {
        _supplierRepositoryMock = new Mock<ISupplierRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new UpdateSupplierCommandHandler(
            _supplierRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var existing = CreateSupplier(supplierId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _supplierRepositoryMock.Setup(x => x.RfcExistsAsync(It.IsAny<string>(), supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = CreateValidCommand(supplierId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _supplierRepositoryMock.Verify(x => x.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var supplierId = Guid.NewGuid();

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);

        var command = CreateValidCommand(supplierId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Supplier.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _supplierRepositoryMock.Verify(x => x.Update(It.IsAny<Supplier>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = CreateValidCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenRfcAlreadyExistsForAnotherSupplier_ReturnsConflictError()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var existing = CreateSupplier(supplierId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _supplierRepositoryMock.Setup(x => x.RfcExistsAsync("XAXX010101000", supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = CreateValidCommand(supplierId) with { Rfc = "XAXX010101000" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Supplier.RfcExists");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_UpdatesAllFieldsOnEntity()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var existing = CreateSupplier(supplierId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            null,
            "New Contact",
            "new@email.mx",
            "555-1234",
            "Nueva Calle 1",
            "Guadalajara",
            "Jalisco",
            "44100",
            60,
            "601",
            "Updated notes",
            false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existing.Name.Should().Be("New Name");
        existing.ContactName.Should().Be("New Contact");
        existing.Email.Should().Be("new@email.mx");
        existing.City.Should().Be("Guadalajara");
        existing.PaymentTermsDays.Should().Be(60);
        existing.IsActive.Should().BeFalse();
    }

    private Supplier CreateSupplier(Guid id) =>
        new() { Id = id, TenantId = _tenantId, Name = "Original Name", IsActive = true };

    private static UpdateSupplierCommand CreateValidCommand(Guid id) =>
        new(id, "Updated Name", null, null, null, null, null, null, null, null, 30, null, null, true);
}
