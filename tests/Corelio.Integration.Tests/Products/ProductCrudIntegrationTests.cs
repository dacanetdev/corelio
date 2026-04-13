using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Infrastructure.Persistence.Repositories;
using Corelio.Integration.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Integration.Tests.Products;

/// <summary>
/// Integration tests for Product CRUD operations and multi-tenancy isolation (TD-3.1.B).
/// Verifies that tenant query filters correctly isolate product data between tenants.
/// </summary>
[Trait("Category", "Integration")]
[Collection("PostgreSQL")]
public class ProductCrudIntegrationTests(PostgreSqlTestContainerFixture fixture) : IClassFixture<PostgreSqlTestContainerFixture>
{
    private readonly PostgreSqlTestContainerFixture _fixture = fixture;

    // ──────────────────────────────────────────────────────────────────────────
    // Create
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddProduct_CanBeRetrievedByIdWithinSameTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);
        var repository = new ProductRepository(dbContext);

        var product = CreateProduct(tenantId, "BOLT-001", "Perno 1/4\"");
        repository.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        var retrieved = await repository.GetByIdAsync(product.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(product.Id);
        retrieved.Name.Should().Be("Perno 1/4\"");
        retrieved.Sku.Should().Be("BOLT-001");
        retrieved.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task AddProduct_CannotBeRetrievedByDifferentTenant()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        var repositoryA = new ProductRepository(dbContextA);
        var repositoryB = new ProductRepository(dbContextB);

        var product = CreateProduct(tenantA, "NUT-001", "Tuerca 1/4\"");
        repositoryA.Add(product);
        await dbContextA.SaveChangesAsync();

        // Act – Tenant B tries to get Tenant A's product
        var retrieved = await repositoryB.GetByIdAsync(product.Id);

        // Assert – Tenant B cannot see it
        retrieved.Should().BeNull();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Read – paged list
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPagedAsync_ReturnsOnlyCurrentTenantProducts()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        var repositoryA = new ProductRepository(dbContextA);
        var repositoryB = new ProductRepository(dbContextB);

        // Seed 4 products for Tenant A
        var productsA = Enumerable.Range(1, 4).Select(i =>
            CreateProduct(tenantA, $"PROD-A{i}", $"Producto A{i}")).ToList();

        // Seed 2 products for Tenant B
        var productsB = Enumerable.Range(1, 2).Select(i =>
            CreateProduct(tenantB, $"PROD-B{i}", $"Producto B{i}")).ToList();

        dbContextA.Products.AddRange(productsA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.AddRange(productsB);
        await dbContextB.SaveChangesAsync();

        // Act
        var (resultA, totalA) = await repositoryA.GetPagedAsync(1, 10);
        var (resultB, totalB) = await repositoryB.GetPagedAsync(1, 10);

        // Assert
        resultA.Should().OnlyContain(p => p.TenantId == tenantA);
        totalA.Should().BeGreaterThanOrEqualTo(4);

        resultB.Should().OnlyContain(p => p.TenantId == tenantB);
        totalB.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetPagedAsync_FiltersBySearchTerm()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);
        var repository = new ProductRepository(dbContext);

        var cement = CreateProduct(tenantId, "CEM-001", "Cemento Portland");
        var paint = CreateProduct(tenantId, "PAINT-001", "Pintura Blanca");
        dbContext.Products.AddRange(cement, paint);
        await dbContext.SaveChangesAsync();

        // Act
        var (results, total) = await repository.GetPagedAsync(1, 10, searchTerm: "Cemento");

        // Assert
        results.Should().OnlyContain(p => p.Name.Contains("Cemento", StringComparison.OrdinalIgnoreCase));
        total.Should().BeGreaterThanOrEqualTo(1);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Update
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateProduct_PersistsChanges()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);
        var repository = new ProductRepository(dbContext);

        var product = CreateProduct(tenantId, "SCREW-001", "Tornillo 3/4\"");
        repository.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        product.Name = "Tornillo 3/4\" Actualizado";
        product.SalePrice = 15.99m;
        repository.Update(product);
        await dbContext.SaveChangesAsync();

        // Re-load from DB
        dbContext.ChangeTracker.Clear();
        var updated = await repository.GetByIdAsync(product.Id);

        // Assert
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Tornillo 3/4\" Actualizado");
        updated.SalePrice.Should().Be(15.99m);
    }

    [Fact]
    public async Task UpdateProduct_DoesNotAffectOtherTenantProduct()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Both tenants have the same SKU
        var productA = CreateProduct(tenantA, "WIRE-001", "Cable Electrico");
        var productB = CreateProduct(tenantB, "WIRE-001", "Cable Electrico");

        dbContextA.Products.Add(productA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.Add(productB);
        await dbContextB.SaveChangesAsync();

        // Act – Update Tenant A's product
        productA.SalePrice = 99.99m;
        dbContextA.Products.Update(productA);
        await dbContextA.SaveChangesAsync();

        // Assert – Tenant B's product unchanged
        dbContextB.ChangeTracker.Clear();
        var reloadedB = await dbContextB.Products.FirstOrDefaultAsync(p => p.Id == productB.Id);
        reloadedB.Should().NotBeNull();
        reloadedB!.SalePrice.Should().Be(10m); // Original
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Delete (soft)
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteProduct_SoftDeletesAndHidesFromQueries()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);
        var repository = new ProductRepository(dbContext);

        var product = CreateProduct(tenantId, "PIPE-001", "Tubo PVC 3/4\"");
        repository.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        repository.Delete(product);
        await dbContext.SaveChangesAsync();

        // Assert – GetById returns null (soft-delete filter)
        dbContext.ChangeTracker.Clear();
        var deleted = await repository.GetByIdAsync(product.Id);
        deleted.Should().BeNull();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SKU uniqueness per tenant
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task SkuExistsAsync_ReturnsTrueForSameTenantDuplicateSku()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);
        var repository = new ProductRepository(dbContext);

        var product = CreateProduct(tenantId, "VALVE-001", "Válvula de Paso");
        repository.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        var exists = await repository.SkuExistsAsync("VALVE-001");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task SkuExistsAsync_ReturnsFalseForSameSkuInDifferentTenant()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        var repositoryA = new ProductRepository(dbContextA);
        var repositoryB = new ProductRepository(dbContextB);

        // Tenant A has VALVE-002
        var productA = CreateProduct(tenantA, "VALVE-002", "Válvula de Paso");
        repositoryA.Add(productA);
        await dbContextA.SaveChangesAsync();

        // Act – Check from Tenant B's perspective
        var existsForB = await repositoryB.SkuExistsAsync("VALVE-002");

        // Assert – Tenant B does not see Tenant A's SKU
        existsForB.Should().BeFalse();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static Product CreateProduct(Guid tenantId, string sku, string name) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Sku = sku,
            Name = name,
            SalePrice = 10m,
            CostPrice = 6m,
            UnitOfMeasure = UnitOfMeasure.PCS,
            IsActive = true
        };
}
