using Corelio.Domain.Entities;
using Corelio.Infrastructure.Persistence.Repositories;
using Corelio.Integration.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Integration.Tests.Pricing;

/// <summary>
/// Integration tests for Product pricing multi-tenancy isolation.
/// Verifies that tenant query filters correctly isolate product pricing data.
/// </summary>
[Trait("Category", "Integration")]
[Collection("PostgreSQL")]
public class ProductPricingIsolationTests(PostgreSqlTestContainerFixture fixture) : IClassFixture<PostgreSqlTestContainerFixture>
{
    private readonly PostgreSqlTestContainerFixture _fixture = fixture;

    [Fact]
    public async Task GetProductsPricingList_ReturnsOnlyCurrentTenantProducts()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Seed 5 products for Tenant A
        var productsA = Enumerable.Range(1, 5).Select(i => new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = $"Product A{i}",
            Sku = $"SKU-A{i}",
            ListPrice = 100m * i
        }).ToList();

        // Seed 3 products for Tenant B
        var productsB = Enumerable.Range(1, 3).Select(i => new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = $"Product B{i}",
            Sku = $"SKU-B{i}",
            ListPrice = 200m * i
        }).ToList();

        dbContextA.Products.AddRange(productsA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.AddRange(productsB);
        await dbContextB.SaveChangesAsync();

        // Act - Query as Tenant A
        var repositoryA = new ProductPricingRepository(dbContextA);
        var (resultA, totalA) = await repositoryA.GetProductsPricingListAsync(1, 10, null, null);

        // Assert
        resultA.Should().HaveCount(5);
        resultA.Should().OnlyContain(p => p.TenantId == tenantA);
        totalA.Should().Be(5);
    }

    [Fact]
    public async Task GetProductPricing_CannotAccessOtherTenantsProduct()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create product for Tenant A
        var productA = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Product A",
            Sku = "SKU-A",
            ListPrice = 500m
        };

        dbContextA.Products.Add(productA);
        await dbContextA.SaveChangesAsync();

        // Act - Try to query as Tenant B
        var repositoryB = new ProductPricingRepository(dbContextB);
        var resultB = await repositoryB.GetProductPricingAsync(productA.Id);

        // Assert - Tenant B cannot see Tenant A's product
        resultB.Should().BeNull();
    }

    [Fact]
    public async Task UpdateProductPricing_OnlyAffectsCurrentTenant()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create identical SKU products for both tenants
        var productA = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Cement",
            Sku = "CEM-001",
            ListPrice = 500m,
            NetCost = 400m
        };

        var productB = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Cement",
            Sku = "CEM-001", // Same SKU
            ListPrice = 500m,
            NetCost = 400m
        };

        dbContextA.Products.Add(productA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.Add(productB);
        await dbContextB.SaveChangesAsync();

        // Act - Update Tenant A's product pricing
        productA.NetCost = 350m;
        dbContextA.Products.Update(productA);
        await dbContextA.SaveChangesAsync();

        // Assert - Tenant B's product unchanged
        dbContextB.Entry(productB).State = EntityState.Detached;
        var reloadedProductB = await dbContextB.Products
            .FirstOrDefaultAsync(p => p.Id == productB.Id);

        reloadedProductB.Should().NotBeNull();
        reloadedProductB!.NetCost.Should().Be(400m); // Original value unchanged
    }

    [Fact]
    public async Task BulkUpdatePricing_OnlyUpdatesCurrentTenantProducts()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Seed 10 products for Tenant A
        var productsA = Enumerable.Range(1, 10).Select(i => new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = $"Product A{i}",
            Sku = $"SKU-A{i}",
            ListPrice = 100m
        }).ToList();

        // Seed 5 products for Tenant B
        var productsB = Enumerable.Range(1, 5).Select(i => new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = $"Product B{i}",
            Sku = $"SKU-B{i}",
            ListPrice = 100m
        }).ToList();

        dbContextA.Products.AddRange(productsA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.AddRange(productsB);
        await dbContextB.SaveChangesAsync();

        // Act - Bulk update ListPrice for Tenant A (query filter applies)
        await dbContextA.Products
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.ListPrice, 200m));

        // Clear change tracker to avoid stale data after ExecuteUpdateAsync
        dbContextA.ChangeTracker.Clear();

        // Assert - Tenant A products updated
        var updatedProductsA = await dbContextA.Products.ToListAsync();
        updatedProductsA.Should().HaveCount(10);
        updatedProductsA.Should().OnlyContain(p => p.ListPrice == 200m);

        // Assert - Tenant B products unchanged
        var unchangedProductsB = await dbContextB.Products.ToListAsync();
        unchangedProductsB.Should().HaveCount(5);
        unchangedProductsB.Should().OnlyContain(p => p.ListPrice == 100m);
    }

    [Fact]
    public async Task ProductDiscounts_IsolatedByTenant()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create products with discounts for both tenants
        var productA = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Product A",
            Sku = "SKU-A",
            ListPrice = 1000m
        };

        var productB = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Product B",
            Sku = "SKU-B",
            ListPrice = 1000m
        };

        dbContextA.Products.Add(productA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.Add(productB);
        await dbContextB.SaveChangesAsync();

        // Add discounts
        var discountA = new ProductDiscount
        {
            ProductId = productA.Id,
            TenantId = tenantA,
            TierNumber = 1,
            DiscountPercentage = 10m
        };

        var discountB = new ProductDiscount
        {
            ProductId = productB.Id,
            TenantId = tenantB,
            TierNumber = 1,
            DiscountPercentage = 15m
        };

        dbContextA.ProductDiscounts.Add(discountA);
        await dbContextA.SaveChangesAsync();

        dbContextB.ProductDiscounts.Add(discountB);
        await dbContextB.SaveChangesAsync();

        // Act - Query discounts as Tenant A
        var discountsA = await dbContextA.ProductDiscounts.ToListAsync();

        // Act - Query discounts as Tenant B
        var discountsB = await dbContextB.ProductDiscounts.ToListAsync();

        // Assert - Each tenant only sees their own discounts
        discountsA.Should().HaveCount(1);
        discountsA.Should().OnlyContain(d => d.TenantId == tenantA);
        discountsA.First().DiscountPercentage.Should().Be(10m);

        discountsB.Should().HaveCount(1);
        discountsB.Should().OnlyContain(d => d.TenantId == tenantB);
        discountsB.First().DiscountPercentage.Should().Be(15m);
    }

    [Fact]
    public async Task ProductMarginPrices_IsolatedByTenant()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create products with margin prices for both tenants
        var productA = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Product A",
            Sku = "SKU-A",
            ListPrice = 1000m,
            NetCost = 800m
        };

        var productB = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Product B",
            Sku = "SKU-B",
            ListPrice = 1000m,
            NetCost = 800m
        };

        dbContextA.Products.Add(productA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.Add(productB);
        await dbContextB.SaveChangesAsync();

        // Add margin prices
        var marginA = new ProductMarginPrice
        {
            ProductId = productA.Id,
            TenantId = tenantA,
            TierNumber = 1,
            MarginPercentage = 30m,
            SalePrice = 1142.86m,
            PriceWithIva = 1325.72m
        };

        var marginB = new ProductMarginPrice
        {
            ProductId = productB.Id,
            TenantId = tenantB,
            TierNumber = 1,
            MarginPercentage = 40m,
            SalePrice = 1333.33m,
            PriceWithIva = 1546.66m
        };

        dbContextA.ProductMarginPrices.Add(marginA);
        await dbContextA.SaveChangesAsync();

        dbContextB.ProductMarginPrices.Add(marginB);
        await dbContextB.SaveChangesAsync();

        // Act - Query margin prices as Tenant A
        var marginsA = await dbContextA.ProductMarginPrices.ToListAsync();

        // Act - Query margin prices as Tenant B
        var marginsB = await dbContextB.ProductMarginPrices.ToListAsync();

        // Assert - Each tenant only sees their own margin prices
        marginsA.Should().HaveCount(1);
        marginsA.Should().OnlyContain(m => m.TenantId == tenantA);
        marginsA.First().MarginPercentage.Should().Be(30m);

        marginsB.Should().HaveCount(1);
        marginsB.Should().OnlyContain(m => m.TenantId == tenantB);
        marginsB.First().MarginPercentage.Should().Be(40m);
    }

    [Fact]
    public async Task SearchByNameSKU_RespectsTenantBoundary()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create same SKU for both tenants
        var productA = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Cement Portland",
            Sku = "CEM-001",
            ListPrice = 500m
        };

        var productB = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Cement Portland",
            Sku = "CEM-001", // Same SKU
            ListPrice = 550m
        };

        dbContextA.Products.Add(productA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.Add(productB);
        await dbContextB.SaveChangesAsync();

        // Act - Search by SKU as Tenant A
        var repositoryA = new ProductPricingRepository(dbContextA);
        var (resultA, _) = await repositoryA.GetProductsPricingListAsync(1, 10, "CEM-001", null);

        // Assert - Only Tenant A's product returned
        resultA.Should().HaveCount(1);
        resultA.Should().OnlyContain(p => p.TenantId == tenantA);
        resultA.First().ListPrice.Should().Be(500m);
    }

    [Fact]
    public async Task CategoryFilter_RespectsTenantBoundary()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create categories with same name for both tenants
        var categoryA = new ProductCategory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Construction Materials"
        };

        var categoryB = new ProductCategory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Construction Materials"
        };

        dbContextA.ProductCategories.Add(categoryA);
        await dbContextA.SaveChangesAsync();

        dbContextB.ProductCategories.Add(categoryB);
        await dbContextB.SaveChangesAsync();

        // Create products in categories
        var productA = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Product A",
            Sku = "SKU-A",
            ListPrice = 100m,
            CategoryId = categoryA.Id
        };

        var productB = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Product B",
            Sku = "SKU-B",
            ListPrice = 200m,
            CategoryId = categoryB.Id
        };

        dbContextA.Products.Add(productA);
        await dbContextA.SaveChangesAsync();

        dbContextB.Products.Add(productB);
        await dbContextB.SaveChangesAsync();

        // Act - Filter by category as Tenant A
        var repositoryA = new ProductPricingRepository(dbContextA);
        var (resultA, _) = await repositoryA.GetProductsPricingListAsync(1, 10, null, categoryA.Id);

        // Assert - Only Tenant A's product returned
        resultA.Should().HaveCount(1);
        resultA.Should().OnlyContain(p => p.TenantId == tenantA);
        resultA.First().ListPrice.Should().Be(100m);
    }
}
