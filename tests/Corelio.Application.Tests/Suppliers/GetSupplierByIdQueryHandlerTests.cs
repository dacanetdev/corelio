using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Queries.GetSupplierById;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Suppliers;

[Trait("Category", "Unit")]
public class GetSupplierByIdQueryHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
    private readonly GetSupplierByIdQueryHandler _handler;

    public GetSupplierByIdQueryHandlerTests()
    {
        _supplierRepositoryMock = new Mock<ISupplierRepository>();
        _handler = new GetSupplierByIdQueryHandler(_supplierRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_ReturnsSupplierDto()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var updatedAt = DateTime.UtcNow.AddDays(-2);
        var supplier = new Supplier
        {
            Id = supplierId,
            TenantId = Guid.NewGuid(),
            Name = "Acero del Norte",
            Rfc = "ANO010101ABC",
            ContactName = "Carlos López",
            Email = "carlos@aceronorte.mx",
            Phone = "8009991234",
            Street = "Calle Industrial 500",
            City = "Monterrey",
            State = "Nuevo León",
            ZipCode = "64000",
            PaymentTermsDays = 45,
            TaxRegime = "612",
            Notes = "Pago puntual",
            IsActive = true,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(supplierId);
        result.Value.Name.Should().Be("Acero del Norte");
        result.Value.Rfc.Should().Be("ANO010101ABC");
        result.Value.ContactName.Should().Be("Carlos López");
        result.Value.Email.Should().Be("carlos@aceronorte.mx");
        result.Value.Phone.Should().Be("8009991234");
        result.Value.Street.Should().Be("Calle Industrial 500");
        result.Value.City.Should().Be("Monterrey");
        result.Value.State.Should().Be("Nuevo León");
        result.Value.ZipCode.Should().Be("64000");
        result.Value.PaymentTermsDays.Should().Be(45);
        result.Value.TaxRegime.Should().Be("612");
        result.Value.Notes.Should().Be("Pago puntual");
        result.Value.IsActive.Should().BeTrue();
        result.Value.CreatedAt.Should().Be(createdAt);
        result.Value.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async Task Handle_WhenSupplierNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var supplierId = Guid.NewGuid();

        _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);

        var query = new GetSupplierByIdQuery(supplierId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Supplier.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
