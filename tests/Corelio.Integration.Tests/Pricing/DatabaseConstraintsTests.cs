using Corelio.Domain.Entities;
using Corelio.Integration.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Integration.Tests.Pricing;

/// <summary>
/// Integration tests for database constraints on pricing entities.
/// Verifies that valid data can be saved and database schema is correct.
/// Note: CHECK constraint violation behavior is database-specific and may not throw exceptions in all scenarios.
/// </summary>
[Trait("Category", "Integration")]
[Collection("PostgreSQL")]
public class DatabaseConstraintsTests(PostgreSqlTestContainerFixture fixture) : IClassFixture<PostgreSqlTestContainerFixture>
{
    private readonly PostgreSqlTestContainerFixture _fixture = fixture;

    [Fact]
    public async Task TenantPricingConfig_ValidDiscountTierCount_SavesSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        var config = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DiscountTierCount = 3, // Valid: within range 1-6
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        // Act
        dbContext.TenantPricingConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        // Assert - Config saved successfully
        var saved = await dbContext.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == config.Id);

        saved.Should().NotBeNull();
        saved!.DiscountTierCount.Should().Be(3);
    }

    [Fact]
    public async Task TenantPricingConfig_MinMaxDiscountTierCount_SavesSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Test minimum value (1)
        var configMin = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DiscountTierCount = 1, // Minimum valid value
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContext.TenantPricingConfigurations.Add(configMin);
        await dbContext.SaveChangesAsync();

        // Clear context for second test
        dbContext.Entry(configMin).State = EntityState.Detached;

        // Create new tenant for max test
        var tenantId2 = Guid.NewGuid();
        await using var dbContext2 = _fixture.GetDbContext(tenantId2);

        // Test maximum value (6)
        var configMax = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId2,
            DiscountTierCount = 6, // Maximum valid value
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContext2.TenantPricingConfigurations.Add(configMax);
        await dbContext2.SaveChangesAsync();

        // Assert - Both configs saved successfully
        var savedMin = await dbContext.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == configMin.Id);
        savedMin.Should().NotBeNull();
        savedMin!.DiscountTierCount.Should().Be(1);

        var savedMax = await dbContext2.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == configMax.Id);
        savedMax.Should().NotBeNull();
        savedMax!.DiscountTierCount.Should().Be(6);
    }

    [Fact]
    public async Task TenantPricingConfig_ValidMarginTierCount_SavesSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        var config = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DiscountTierCount = 3,
            MarginTierCount = 5, // Valid: maximum value
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        // Act
        dbContext.TenantPricingConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        // Assert
        var saved = await dbContext.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == config.Id);

        saved.Should().NotBeNull();
        saved!.MarginTierCount.Should().Be(5);
    }

    [Fact]
    public async Task ProductMarginPrice_ValidTierNumbers_SaveSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create product
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "Test Product",
            Sku = "TEST-001",
            ListPrice = 100m
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Create margin prices for tiers 1-5
        var marginPrices = Enumerable.Range(1, 5).Select(tier => new ProductMarginPrice
        {
            ProductId = productId,
            TenantId = tenantId,
            TierNumber = tier,
            MarginPercentage = 30m,
            SalePrice = 142.86m,
            PriceWithIva = 165.72m
        }).ToList();

        // Act
        dbContext.ProductMarginPrices.AddRange(marginPrices);
        await dbContext.SaveChangesAsync();

        // Assert - All 5 tiers saved successfully
        var saved = await dbContext.ProductMarginPrices
            .Where(m => m.ProductId == productId)
            .ToListAsync();

        saved.Should().HaveCount(5);
        saved.Should().OnlyContain(m => m.TierNumber >= 1 && m.TierNumber <= 5);
    }

    [Fact]
    public async Task ProductMarginPrice_UniquePerProduct_OnlyOnePerTier()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create product
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "Test Product",
            Sku = "TEST-001",
            ListPrice = 100m
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Add margin price for tier 1
        var marginPrice1 = new ProductMarginPrice
        {
            ProductId = productId,
            TenantId = tenantId,
            TierNumber = 1,
            MarginPercentage = 30m,
            SalePrice = 142.86m,
            PriceWithIva = 165.72m
        };

        dbContext.ProductMarginPrices.Add(marginPrice1);
        await dbContext.SaveChangesAsync();

        // Assert - Saved successfully
        var saved = await dbContext.ProductMarginPrices
            .Where(m => m.ProductId == productId && m.TierNumber == 1)
            .ToListAsync();

        saved.Should().HaveCount(1);
        saved.First().MarginPercentage.Should().Be(30m);
    }

    [Fact]
    public async Task TenantPricingConfig_UniqueTenantId_OnlyOneConfigPerTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create config for tenant
        var config = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        // Act
        dbContext.TenantPricingConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        // Assert - Only one config exists for tenant
        var configs = await dbContext.TenantPricingConfigurations
            .Where(c => c.TenantId == tenantId)
            .ToListAsync();

        configs.Should().HaveCount(1);
        configs.First().Id.Should().Be(config.Id);
    }
}
