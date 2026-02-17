using Corelio.Domain.Entities;
using Corelio.Integration.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Corelio.Integration.Tests.Pricing;

/// <summary>
/// Integration tests for bulk pricing update workflows.
/// Verifies bulk operations work correctly with real PostgreSQL transactions.
/// </summary>
[Trait("Category", "Integration")]
[Collection("PostgreSQL")]
public class BulkUpdateWorkflowTests(PostgreSqlTestContainerFixture fixture) : IClassFixture<PostgreSqlTestContainerFixture>
{
    private readonly PostgreSqlTestContainerFixture _fixture = fixture;

    [Fact]
    public async Task BulkPercentageIncrease_Updates10Products_AllUpdated()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create 10 products with margin prices
        var products = Enumerable.Range(1, 10).Select(i =>
        {
            var productId = Guid.NewGuid();
            return new
            {
                Product = new Product
                {
                    Id = productId,
                    TenantId = tenantId,
                    Name = $"Product {i}",
                    Sku = $"SKU-{i:D3}",
                    ListPrice = 100m * i,
                    NetCost = 80m * i
                },
                MarginPrice = new ProductMarginPrice
                {
                    ProductId = productId,
                    TenantId = tenantId,
                    TierNumber = 1,
                    MarginPercentage = 30m,
                    SalePrice = 100m * i,
                    PriceWithIva = 116m * i
                }
            };
        }).ToList();

        dbContext.Products.AddRange(products.Select(p => p.Product));
        await dbContext.SaveChangesAsync();

        dbContext.ProductMarginPrices.AddRange(products.Select(p => p.MarginPrice));
        await dbContext.SaveChangesAsync();

        // Act - Apply 10% increase to all margin prices for tier 1
        var originalPrices = products.Select(p => p.MarginPrice.SalePrice).ToList();

        await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 1)
            .ExecuteUpdateAsync(m => m
                .SetProperty(x => x.SalePrice, x => x.SalePrice * 1.1m)
                .SetProperty(x => x.PriceWithIva, x => x.PriceWithIva!.Value * 1.1m));

        // Assert - Reload and verify all 10 products updated
        var updatedPrices = await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 1)
            .OrderBy(m => m.ProductId)
            .Select(m => m.SalePrice)
            .ToListAsync();

        updatedPrices.Should().HaveCount(10);

        for (int i = 0; i < 10; i++)
        {
            var expected = Math.Round(originalPrices[i]!.Value * 1.1m, 2);
            updatedPrices[i].Should().Be(expected);
        }
    }

    [Fact]
    public async Task BulkSetNewMargin_UpdatesSpecificTier_Success()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create 5 products with 3 margin tiers each
        var products = Enumerable.Range(1, 5).Select(i =>
        {
            var productId = Guid.NewGuid();
            return new
            {
                Product = new Product
                {
                    Id = productId,
                    TenantId = tenantId,
                    Name = $"Product {i}",
                    Sku = $"SKU-{i:D3}",
                    ListPrice = 1000m,
                    NetCost = 800m
                },
                MarginTier1 = new ProductMarginPrice
                {
                    ProductId = productId,
                    TenantId = tenantId,
                    TierNumber = 1,
                    MarginPercentage = 30m,
                    SalePrice = 1142.86m,
                    PriceWithIva = 1325.72m
                },
                MarginTier2 = new ProductMarginPrice
                {
                    ProductId = productId,
                    TenantId = tenantId,
                    TierNumber = 2,
                    MarginPercentage = 25m,
                    SalePrice = 1066.67m,
                    PriceWithIva = 1237.34m
                },
                MarginTier3 = new ProductMarginPrice
                {
                    ProductId = productId,
                    TenantId = tenantId,
                    TierNumber = 3,
                    MarginPercentage = 20m,
                    SalePrice = 1000m,
                    PriceWithIva = 1160m
                }
            };
        }).ToList();

        dbContext.Products.AddRange(products.Select(p => p.Product));
        await dbContext.SaveChangesAsync();

        dbContext.ProductMarginPrices.AddRange(products.SelectMany(p => new[] { p.MarginTier1, p.MarginTier2, p.MarginTier3 }));
        await dbContext.SaveChangesAsync();

        // Act - Set tier 2 margin to 35% for all 5 products
        var netCost = 800m;
        var newMargin = 35m;
        var newSalePrice = Math.Round(netCost / (1 - newMargin / 100m), 2);
        var newPriceWithIva = Math.Round(newSalePrice * 1.16m, 2);

        await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 2)
            .ExecuteUpdateAsync(m => m
                .SetProperty(x => x.MarginPercentage, newMargin)
                .SetProperty(x => x.SalePrice, newSalePrice)
                .SetProperty(x => x.PriceWithIva, newPriceWithIva));

        // Assert - Verify only tier 2 updated, tiers 1 and 3 unchanged
        var tier1Prices = await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 1)
            .Select(m => m.MarginPercentage)
            .ToListAsync();

        var tier2Prices = await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 2)
            .Select(m => new { m.MarginPercentage, m.SalePrice, m.PriceWithIva })
            .ToListAsync();

        var tier3Prices = await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 3)
            .Select(m => m.MarginPercentage)
            .ToListAsync();

        // Tier 1 unchanged
        tier1Prices.Should().HaveCount(5);
        tier1Prices.Should().OnlyContain(m => m == 30m);

        // Tier 2 updated
        tier2Prices.Should().HaveCount(5);
        tier2Prices.Should().OnlyContain(m => m.MarginPercentage == 35m);
        tier2Prices.Should().OnlyContain(m => m.SalePrice == newSalePrice);
        tier2Prices.Should().OnlyContain(m => m.PriceWithIva == newPriceWithIva);

        // Tier 3 unchanged
        tier3Prices.Should().HaveCount(5);
        tier3Prices.Should().OnlyContain(m => m == 20m);
    }

    [Fact]
    public async Task BulkUpdate_TransactionRollback_OnError()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create 5 products with margin prices
        var products = Enumerable.Range(1, 5).Select(i =>
        {
            var productId = Guid.NewGuid();
            return new
            {
                Product = new Product
                {
                    Id = productId,
                    TenantId = tenantId,
                    Name = $"Product {i}",
                    Sku = $"SKU-{i:D3}",
                    ListPrice = 100m,
                    NetCost = 80m
                },
                MarginPrice = new ProductMarginPrice
                {
                    ProductId = productId,
                    TenantId = tenantId,
                    TierNumber = 1,
                    MarginPercentage = 30m,
                    SalePrice = 100m,
                    PriceWithIva = 116m
                }
            };
        }).ToList();

        dbContext.Products.AddRange(products.Select(p => p.Product));
        await dbContext.SaveChangesAsync();

        dbContext.ProductMarginPrices.AddRange(products.Select(p => p.MarginPrice));
        await dbContext.SaveChangesAsync();

        var originalPrices = await dbContext.ProductMarginPrices
            .Select(m => new { m.ProductId, m.SalePrice })
            .ToListAsync();

        // Act - Attempt bulk update with invalid data (negative margin percentage)
        // This should fail due to business logic or database constraints
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            // Simulate a failed update (e.g., invalid negative value)
            await dbContext.ProductMarginPrices
                .ExecuteUpdateAsync(m => m.SetProperty(x => x.MarginPercentage, -10m));

            // Simulate another operation that might fail
            throw new InvalidOperationException("Simulated error during bulk update");
        }
        catch
        {
            // Rollback on error
            await transaction.RollbackAsync();
        }

        // Assert - Verify all products unchanged after rollback
        var unchangedPrices = await dbContext.ProductMarginPrices
            .Select(m => new { m.ProductId, m.SalePrice, m.MarginPercentage })
            .ToListAsync();

        unchangedPrices.Should().HaveCount(5);

        foreach (var price in unchangedPrices)
        {
            var original = originalPrices.First(p => p.ProductId == price.ProductId);
            price.SalePrice.Should().Be(original.SalePrice);
            price.MarginPercentage.Should().Be(30m); // Original value
        }
    }

    [Fact]
    public async Task BulkUpdate_Pagination_HandlesManyProducts()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create 100 products
        var products = Enumerable.Range(1, 100).Select(i =>
        {
            var productId = Guid.NewGuid();
            return new
            {
                Product = new Product
                {
                    Id = productId,
                    TenantId = tenantId,
                    Name = $"Product {i}",
                    Sku = $"SKU-{i:D4}",
                    ListPrice = 100m,
                    NetCost = 80m
                },
                MarginPrice = new ProductMarginPrice
                {
                    ProductId = productId,
                    TenantId = tenantId,
                    TierNumber = 1,
                    MarginPercentage = 30m,
                    SalePrice = 100m,
                    PriceWithIva = 116m
                }
            };
        }).ToList();

        dbContext.Products.AddRange(products.Select(p => p.Product));
        await dbContext.SaveChangesAsync();

        dbContext.ProductMarginPrices.AddRange(products.Select(p => p.MarginPrice));
        await dbContext.SaveChangesAsync();

        // Act - Apply bulk update to all 100 products (measure performance)
        var stopwatch = Stopwatch.StartNew();

        await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 1)
            .ExecuteUpdateAsync(m => m.SetProperty(x => x.SalePrice, x => x.SalePrice * 1.05m));

        stopwatch.Stop();

        // Assert - All 100 products updated
        var updatedCount = await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 1)
            .CountAsync();

        updatedCount.Should().Be(100);

        // Assert - Performance requirement: <10 seconds (US-6.6 success criteria)
        stopwatch.Elapsed.TotalSeconds.Should().BeLessThan(10);

        // Verify at least one product has updated price
        var sampleProduct = await dbContext.ProductMarginPrices
            .Where(m => m.TierNumber == 1)
            .FirstAsync();

        sampleProduct.SalePrice.Should().Be(105m); // 100 × 1.05
    }

    [Fact]
    public async Task BulkUpdateFixedAmount_WithNegativeResult_ClampsToZero()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create product with margin price = 50
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "Low Price Product",
            Sku = "LOW-001",
            ListPrice = 100m,
            NetCost = 40m
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var marginPrice = new ProductMarginPrice
        {
            ProductId = productId,
            TenantId = tenantId,
            TierNumber = 1,
            MarginPercentage = 25m,
            SalePrice = 50m,
            PriceWithIva = 58m
        };

        dbContext.ProductMarginPrices.Add(marginPrice);
        await dbContext.SaveChangesAsync();

        // Act - Apply fixed amount decrease of 100 (would result in -50)
        // Clamp to 0 to prevent negative prices
        await dbContext.ProductMarginPrices
            .Where(m => m.ProductId == productId && m.TierNumber == 1)
            .ExecuteUpdateAsync(m => m.SetProperty(
                x => x.SalePrice,
                x => x.SalePrice - 100m > 0 ? x.SalePrice - 100m : 0m));

        // Assert - Margin price set to 0 (not negative)
        dbContext.Entry(marginPrice).State = EntityState.Detached;

        var updatedMargin = await dbContext.ProductMarginPrices
            .FirstOrDefaultAsync(m => m.ProductId == productId && m.TierNumber == 1);

        updatedMargin.Should().NotBeNull();
        updatedMargin!.SalePrice.Should().Be(0m); // Clamped to zero
    }
}
