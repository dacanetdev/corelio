using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Queries.GetSuppliers;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Suppliers;

[Trait("Category", "Unit")]
public class GetSuppliersQueryHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
    private readonly GetSuppliersQueryHandler _handler;

    public GetSuppliersQueryHandlerTests()
    {
        _supplierRepositoryMock = new Mock<ISupplierRepository>();
        _handler = new GetSuppliersQueryHandler(_supplierRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithSuppliers_ReturnsPagedList()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var suppliers = new List<Supplier>
        {
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Supplier A", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Supplier B", IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        _supplierRepositoryMock.Setup(x => x.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((suppliers, 2));

        var query = new GetSuppliersQuery(1, 20, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items[0].Name.Should().Be("Supplier A");
        result.Value.Items[1].Name.Should().Be("Supplier B");
    }

    [Fact]
    public async Task Handle_WithNoSuppliers_ReturnsEmptyPagedResult()
    {
        // Arrange
        _supplierRepositoryMock.Setup(x => x.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Supplier>(), 0));

        var query = new GetSuppliersQuery(1, 20, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MapsSupplierToListDto()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-5);
        var supplier = new Supplier
        {
            Id = supplierId,
            TenantId = Guid.NewGuid(),
            Name = "Ferretería Central",
            Rfc = "FEC010101ABC",
            ContactName = "María García",
            Email = "maria@ferreteria.mx",
            Phone = "8001234567",
            IsActive = true,
            CreatedAt = createdAt
        };

        _supplierRepositoryMock.Setup(x => x.GetPagedAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(([supplier], 1));

        var query = new GetSuppliersQuery(1, 10, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!.Items[0];
        dto.Id.Should().Be(supplierId);
        dto.Name.Should().Be("Ferretería Central");
        dto.Rfc.Should().Be("FEC010101ABC");
        dto.ContactName.Should().Be("María García");
        dto.Email.Should().Be("maria@ferreteria.mx");
        dto.Phone.Should().Be("8001234567");
        dto.IsActive.Should().BeTrue();
        dto.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public async Task Handle_PassesSearchAndFilterParametersToRepository()
    {
        // Arrange
        _supplierRepositoryMock.Setup(x => x.GetPagedAsync(2, 10, "acero", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Supplier>(), 0));

        var query = new GetSuppliersQuery(2, 10, "acero", true);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _supplierRepositoryMock.Verify(
            x => x.GetPagedAsync(2, 10, "acero", true, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
