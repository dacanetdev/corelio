using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Commands.DeleteSupplier;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Suppliers;

[Trait("Category", "Unit")]
public class DeleteSupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly DeleteSupplierCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public DeleteSupplierCommandHandlerTests()
    {
        _supplierRepositoryMock = new Mock<ISupplierRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new DeleteSupplierCommandHandler(
            _supplierRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingSupplier_ReturnsSoftDeleteSuccess()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier { Id = supplierId, TenantId = _tenantId, Name = "Test Supplier" };

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        var command = new DeleteSupplierCommand(supplierId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _supplierRepositoryMock.Verify(x => x.Delete(supplier), Times.Once);
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

        var command = new DeleteSupplierCommand(supplierId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Supplier.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _supplierRepositoryMock.Verify(x => x.Delete(It.IsAny<Supplier>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = new DeleteSupplierCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _supplierRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
