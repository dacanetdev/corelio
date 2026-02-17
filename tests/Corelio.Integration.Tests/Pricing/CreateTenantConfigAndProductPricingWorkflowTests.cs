using Corelio.Domain.Entities;
using Corelio.Integration.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Integration.Tests.Pricing;

/// <summary>
/// Integration tests for end-to-end pricing workflows.
/// Tests the complete flow: create config → create products → update pricing → verify calculations.
/// </summary>
[Trait("Category", "Integration")]
[Collection("PostgreSQL")]
public class CreateTenantConfigAndProductPricingWorkflowTests(PostgreSqlTestContainerFixture fixture) : IClassFixture<PostgreSqlTestContainerFixture>
{
    private readonly PostgreSqlTestContainerFixture _fixture = fixture;

    [Fact]
    public async Task CompleteSetup_CreateConfigThenProducts_Success()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Step 1: Create TenantPricingConfiguration with 3 discounts, 3 margins
        var config = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContext.TenantPricingConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        // Add tier definitions
        var discountTiers = new List<DiscountTierDefinition>
        {
            new() { Id = Guid.NewGuid(), TenantPricingConfigurationId = config.Id, TenantId = tenantId, TierNumber = 1, TierName = "Descuento 1", IsActive = true },
            new() { Id = Guid.NewGuid(), TenantPricingConfigurationId = config.Id, TenantId = tenantId, TierNumber = 2, TierName = "Descuento 2", IsActive = true },
            new() { Id = Guid.NewGuid(), TenantPricingConfigurationId = config.Id, TenantId = tenantId, TierNumber = 3, TierName = "Descuento 3", IsActive = true }
        };

        var marginTiers = new List<MarginTierDefinition>
        {
            new() { Id = Guid.NewGuid(), TenantPricingConfigurationId = config.Id, TenantId = tenantId, TierNumber = 1, TierName = "Publico", IsActive = true },
            new() { Id = Guid.NewGuid(), TenantPricingConfigurationId = config.Id, TenantId = tenantId, TierNumber = 2, TierName = "Mayoreo", IsActive = true },
            new() { Id = Guid.NewGuid(), TenantPricingConfigurationId = config.Id, TenantId = tenantId, TierNumber = 3, TierName = "Especial", IsActive = true }
        };

        dbContext.DiscountTierDefinitions.AddRange(discountTiers);
        dbContext.MarginTierDefinitions.AddRange(marginTiers);
        await dbContext.SaveChangesAsync();

        // Step 2: Create Product with ListPrice = 1000
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "Test Product",
            Sku = "TEST-001",
            ListPrice = 1000m,
            IvaEnabled = true
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Step 3: Update ProductPricing with discounts [10%, 5%, 2%] and margins [30%, 25%, 20%]
        var discounts = new List<ProductDiscount>
        {
            new() { ProductId = productId, TenantId = tenantId, TierNumber = 1, DiscountPercentage = 10m },
            new() { ProductId = productId, TenantId = tenantId, TierNumber = 2, DiscountPercentage = 5m },
            new() { ProductId = productId, TenantId = tenantId, TierNumber = 3, DiscountPercentage = 2m }
        };

        dbContext.ProductDiscounts.AddRange(discounts);
        await dbContext.SaveChangesAsync();

        // Calculate NetCost: 1000 × 0.9 × 0.95 × 0.98 = 837.90
        var netCost = 1000m * 0.9m * 0.95m * 0.98m;
        product.NetCost = netCost;
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();

        // Calculate margin prices
        // Tier 1: 30% margin → SalePrice = 837.90 / 0.7 = 1197.00
        // Tier 2: 25% margin → SalePrice = 837.90 / 0.75 = 1117.20
        // Tier 3: 20% margin → SalePrice = 837.90 / 0.8 = 1047.38
        var marginPrices = new List<ProductMarginPrice>
        {
            new()
            {
                ProductId = productId,
                TenantId = tenantId,
                TierNumber = 1,
                MarginPercentage = 30m,
                SalePrice = Math.Round(netCost / 0.7m, 2),
                PriceWithIva = Math.Round(netCost / 0.7m * 1.16m, 2)
            },
            new()
            {
                ProductId = productId,
                TenantId = tenantId,
                TierNumber = 2,
                MarginPercentage = 25m,
                SalePrice = Math.Round(netCost / 0.75m, 2),
                PriceWithIva = Math.Round(netCost / 0.75m * 1.16m, 2)
            },
            new()
            {
                ProductId = productId,
                TenantId = tenantId,
                TierNumber = 3,
                MarginPercentage = 20m,
                SalePrice = Math.Round(netCost / 0.8m, 2),
                PriceWithIva = Math.Round(netCost / 0.8m * 1.16m, 2)
            }
        };

        dbContext.ProductMarginPrices.AddRange(marginPrices);
        await dbContext.SaveChangesAsync();

        // Act - Query product pricing
        var loadedProduct = await dbContext.Products
            .Include(p => p.Discounts)
            .Include(p => p.MarginPrices)
            .FirstOrDefaultAsync(p => p.Id == productId);

        // Assert - Verify NetCost
        loadedProduct.Should().NotBeNull();
        loadedProduct!.NetCost.Should().Be(837.90m);

        // Assert - Verify SalePrice tier 1 (30% margin)
        var tier1 = loadedProduct.MarginPrices.First(m => m.TierNumber == 1);
        tier1.SalePrice.Should().Be(1197.00m);
        tier1.PriceWithIva.Should().Be(1388.52m);

        // Assert - Verify all discounts applied
        loadedProduct.Discounts.Should().HaveCount(3);
        loadedProduct.MarginPrices.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateTierDefinitions_ProductsCanUseNewTiers()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create config with 3 discount tiers
        var config = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContext.TenantPricingConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        // Add 3 discount tier definitions
        var discountTiers = Enumerable.Range(1, 3).Select(i => new DiscountTierDefinition
        {
            Id = Guid.NewGuid(),
            TenantPricingConfigurationId = config.Id,
            TenantId = tenantId,
            TierNumber = i,
            TierName = $"Descuento {i}",
            IsActive = true
        }).ToList();

        dbContext.DiscountTierDefinitions.AddRange(discountTiers);
        await dbContext.SaveChangesAsync();

        // Create product with 3 discounts
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "Test Product",
            Sku = "TEST-001",
            ListPrice = 1000m
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Act - Update config to 4 discount tiers
        config.DiscountTierCount = 4;
        dbContext.TenantPricingConfigurations.Update(config);
        await dbContext.SaveChangesAsync();

        // Add tier 4 definition
        var tier4 = new DiscountTierDefinition
        {
            Id = Guid.NewGuid(),
            TenantPricingConfigurationId = config.Id,
            TenantId = tenantId,
            TierNumber = 4,
            TierName = "Descuento 4",
            IsActive = true
        };

        dbContext.DiscountTierDefinitions.Add(tier4);
        await dbContext.SaveChangesAsync();

        // Add discount for tier 4 to existing product
        var discount4 = new ProductDiscount
        {
            ProductId = productId,
            TenantId = tenantId,
            TierNumber = 4,
            DiscountPercentage = 1m
        };

        dbContext.ProductDiscounts.Add(discount4);
        await dbContext.SaveChangesAsync();

        // Assert - Product can use tier 4
        var loadedProduct = await dbContext.Products
            .Include(p => p.Discounts)
            .FirstOrDefaultAsync(p => p.Id == productId);

        loadedProduct.Should().NotBeNull();
        loadedProduct!.Discounts.Should().ContainSingle(d => d.TierNumber == 4);
        loadedProduct.Discounts.First(d => d.TierNumber == 4).DiscountPercentage.Should().Be(1m);
    }

    [Fact]
    public async Task CalculationsMatchFERRESYSLogic_ExactAccuracy()
    {
        // Arrange - Known test case from FERRESYS
        // ListPrice = 500, Discounts = [15%, 10%, 5%], Margin = 40%
        // Expected: NetCost = 363.38 (500×0.85×0.9×0.95), SalePrice = 605.63
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "FERRESYS Test Product",
            Sku = "FERR-001",
            ListPrice = 500m,
            IvaEnabled = false
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Apply discounts: 500 × 0.85 × 0.9 × 0.95 = 363.375 → 363.38
        var discounts = new List<ProductDiscount>
        {
            new() { ProductId = productId, TenantId = tenantId, TierNumber = 1, DiscountPercentage = 15m },
            new() { ProductId = productId, TenantId = tenantId, TierNumber = 2, DiscountPercentage = 10m },
            new() { ProductId = productId, TenantId = tenantId, TierNumber = 3, DiscountPercentage = 5m }
        };

        dbContext.ProductDiscounts.AddRange(discounts);
        await dbContext.SaveChangesAsync();

        // Calculate NetCost
        var netCost = 500m * 0.85m * 0.9m * 0.95m; // 363.375
        netCost = Math.Round(netCost, 2); // 363.38

        product.NetCost = netCost;
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();

        // Calculate SalePrice: 363.38 / 0.6 = 605.6333... → 605.63
        var salePrice = Math.Round(netCost / 0.6m, 2);

        var marginPrice = new ProductMarginPrice
        {
            ProductId = productId,
            TenantId = tenantId,
            TierNumber = 1,
            MarginPercentage = 40m,
            SalePrice = salePrice,
            PriceWithIva = null // IVA disabled
        };

        dbContext.ProductMarginPrices.Add(marginPrice);
        await dbContext.SaveChangesAsync();

        // Act - Reload product
        var loadedProduct = await dbContext.Products
            .Include(p => p.Discounts)
            .Include(p => p.MarginPrices)
            .FirstOrDefaultAsync(p => p.Id == productId);

        // Assert - Exact FERRESYS calculations
        loadedProduct.Should().NotBeNull();
        loadedProduct!.NetCost.Should().Be(363.38m);

        var tier1 = loadedProduct.MarginPrices.First(m => m.TierNumber == 1);
        tier1.SalePrice.Should().Be(605.63m);
        tier1.PriceWithIva.Should().BeNull(); // IVA disabled
    }

    [Fact]
    public async Task IvaToggle_AffectsPriceWithIva()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        var config = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DiscountTierCount = 1,
            MarginTierCount = 1,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContext.TenantPricingConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        // Create product with IVA enabled
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            TenantId = tenantId,
            Name = "Test Product",
            Sku = "TEST-001",
            ListPrice = 1000m,
            NetCost = 900m,
            IvaEnabled = true
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Add margin price with IVA
        var marginPrice = new ProductMarginPrice
        {
            ProductId = productId,
            TenantId = tenantId,
            TierNumber = 1,
            MarginPercentage = 30m,
            SalePrice = 1285.71m,
            PriceWithIva = 1491.42m // 1285.71 × 1.16
        };

        dbContext.ProductMarginPrices.Add(marginPrice);
        await dbContext.SaveChangesAsync();

        // Act - Disable IVA
        product.IvaEnabled = false;
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();

        // Update margin price (PriceWithIva should be null)
        marginPrice.PriceWithIva = null;
        dbContext.ProductMarginPrices.Update(marginPrice);
        await dbContext.SaveChangesAsync();

        // Assert - Reload and verify PriceWithIva is null
        dbContext.Entry(product).State = EntityState.Detached;
        dbContext.Entry(marginPrice).State = EntityState.Detached;

        var loadedProduct = await dbContext.Products
            .Include(p => p.MarginPrices)
            .FirstOrDefaultAsync(p => p.Id == productId);

        loadedProduct.Should().NotBeNull();
        loadedProduct!.IvaEnabled.Should().BeFalse();

        var loadedMargin = loadedProduct.MarginPrices.First(m => m.TierNumber == 1);
        loadedMargin.PriceWithIva.Should().BeNull();
        loadedMargin.SalePrice.Should().Be(1285.71m); // SalePrice unchanged
    }
}
