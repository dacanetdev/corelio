using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Commands.CreateSupplier;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Suppliers;

[Trait("Category", "Unit")]
public class CreateSupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly CreateSupplierCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CreateSupplierCommandHandlerTests()
    {
        _supplierRepositoryMock = new Mock<ISupplierRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new CreateSupplierCommandHandler(
            _supplierRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccessWithSupplierId()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.RfcExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _supplierRepositoryMock.Verify(x => x.Add(It.IsAny<Supplier>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _supplierRepositoryMock.Verify(x => x.Add(It.IsAny<Supplier>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRfcAlreadyExists_ReturnsConflictError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.RfcExistsAsync("XAXX010101000", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = CreateValidCommand() with { Rfc = "XAXX010101000" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Supplier.RfcExists");
        result.Error.Type.Should().Be(ErrorType.Conflict);
        _supplierRepositoryMock.Verify(x => x.Add(It.IsAny<Supplier>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullRfc_SkipsRfcUniquenessCheck()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var command = CreateValidCommand() with { Rfc = null };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _supplierRepositoryMock.Verify(
            x => x.RfcExistsAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_SetsCorrectTenantIdOnSupplier()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        Supplier? captured = null;
        _supplierRepositoryMock.Setup(x => x.Add(It.IsAny<Supplier>()))
            .Callback<Supplier>(s => captured = s);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task Handle_MapsAllFieldsCorrectly()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.RfcExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Supplier? captured = null;
        _supplierRepositoryMock.Setup(x => x.Add(It.IsAny<Supplier>()))
            .Callback<Supplier>(s => captured = s);

        var command = new CreateSupplierCommand(
            "Acero del Norte SA",
            "ANO010101ABC",
            "Juan Pérez",
            "juan@aceronorte.mx",
            "8001234567",
            "Calle 5 de Mayo 100",
            "Monterrey",
            "Nuevo León",
            "64000",
            45,
            "612",
            "Proveedor de acero");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        captured!.Name.Should().Be("Acero del Norte SA");
        captured.Rfc.Should().Be("ANO010101ABC");
        captured.ContactName.Should().Be("Juan Pérez");
        captured.Email.Should().Be("juan@aceronorte.mx");
        captured.Phone.Should().Be("8001234567");
        captured.Street.Should().Be("Calle 5 de Mayo 100");
        captured.City.Should().Be("Monterrey");
        captured.State.Should().Be("Nuevo León");
        captured.ZipCode.Should().Be("64000");
        captured.PaymentTermsDays.Should().Be(45);
        captured.TaxRegime.Should().Be("612");
        captured.Notes.Should().Be("Proveedor de acero");
    }

    private static CreateSupplierCommand CreateValidCommand()
    {
        return new CreateSupplierCommand(
            Name: "Proveedor Test",
            Rfc: null,
            ContactName: null,
            Email: null,
            Phone: null,
            Street: null,
            City: null,
            State: null,
            ZipCode: null,
            PaymentTermsDays: 30,
            TaxRegime: null,
            Notes: null);
    }
}
