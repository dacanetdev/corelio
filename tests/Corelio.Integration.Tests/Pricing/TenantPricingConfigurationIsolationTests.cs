using Corelio.Domain.Entities;
using Corelio.Infrastructure.Persistence.Repositories;
using Corelio.Integration.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Integration.Tests.Pricing;

/// <summary>
/// Integration tests for TenantPricingConfiguration multi-tenancy isolation.
/// Verifies that tenant query filters correctly isolate pricing configuration data.
/// </summary>
[Trait("Category", "Integration")]
[Collection("PostgreSQL")]
public class TenantPricingConfigurationIsolationTests(PostgreSqlTestContainerFixture fixture) : IClassFixture<PostgreSqlTestContainerFixture>
{
    private readonly PostgreSqlTestContainerFixture _fixture = fixture;

    [Fact]
    public async Task GetByTenantId_ReturnsOnlyCurrentTenantConfig()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create config for Tenant A
        var configA = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        // Create config for Tenant B
        var configB = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            DiscountTierCount = 4,
            MarginTierCount = 5,
            DefaultIvaEnabled = false,
            IvaPercentage = 16.00m
        };

        dbContextA.TenantPricingConfigurations.Add(configA);
        await dbContextA.SaveChangesAsync();

        dbContextB.TenantPricingConfigurations.Add(configB);
        await dbContextB.SaveChangesAsync();

        // Act - Query as Tenant A using repository
        var repositoryA = new TenantPricingConfigurationRepository(dbContextA);
        var resultA = await repositoryA.GetByTenantIdAsync(tenantA);

        // Assert
        resultA.Should().NotBeNull();
        resultA!.Id.Should().Be(configA.Id);
        resultA.TenantId.Should().Be(tenantA);
        resultA.DiscountTierCount.Should().Be(3);
    }

    [Fact]
    public async Task GetByTenantId_WithDifferentTenant_ReturnsNull()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create config for Tenant A
        var configA = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContextA.TenantPricingConfigurations.Add(configA);
        await dbContextA.SaveChangesAsync();

        // Act - Query as Tenant B (should not see Tenant A's config)
        var repositoryB = new TenantPricingConfigurationRepository(dbContextB);
        var resultB = await repositoryB.GetByTenantIdAsync(tenantB);

        // Assert
        resultB.Should().BeNull();
    }

    [Fact]
    public async Task UpdateConfig_TenantBCannotQueryTenantAConfig()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create config for Tenant A
        var configA = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContextA.TenantPricingConfigurations.Add(configA);
        await dbContextA.SaveChangesAsync();

        // Act - Try to query as Tenant B (should not find it due to query filter)
        var configIdToFind = configA.Id;
        var foundByTenantB = await dbContextB.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == configIdToFind);

        // Assert - Tenant B cannot see Tenant A's config due to query filter
        foundByTenantB.Should().BeNull();

        // Assert - Tenant A can still see their own config
        var foundByTenantA = await dbContextA.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == configIdToFind);
        foundByTenantA.Should().NotBeNull();
        foundByTenantA!.DiscountTierCount.Should().Be(3);
    }

    [Fact]
    public async Task QueryFiltersApplied_AutomaticallyIsolateTenants()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Seed configs for both tenants
        var configA = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        var configB = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            DiscountTierCount = 4,
            MarginTierCount = 5,
            DefaultIvaEnabled = false,
            IvaPercentage = 16.00m
        };

        dbContextA.TenantPricingConfigurations.Add(configA);
        await dbContextA.SaveChangesAsync();

        dbContextB.TenantPricingConfigurations.Add(configB);
        await dbContextB.SaveChangesAsync();

        // Act - Query as Tenant A (query filter should apply)
        var configsA = await dbContextA.TenantPricingConfigurations.ToListAsync();

        // Act - Query as Tenant B
        var configsB = await dbContextB.TenantPricingConfigurations.ToListAsync();

        // Assert - Each tenant only sees their own config
        configsA.Should().HaveCount(1);
        configsA.Should().OnlyContain(c => c.TenantId == tenantA);

        configsB.Should().HaveCount(1);
        configsB.Should().OnlyContain(c => c.TenantId == tenantB);
    }

    [Fact]
    public async Task CreateConfig_AssignsTenantIdFromProvider()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        await using var dbContext = _fixture.GetDbContext(tenantId);

        // Create config WITHOUT manually setting TenantId (interceptor should set it)
        var config = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
            // TenantId not set - TenantInterceptor should assign it
        };

        // Act
        dbContext.TenantPricingConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        // Assert - TenantInterceptor should have assigned TenantId
        config.TenantId.Should().Be(tenantId);

        // Reload to verify database value
        dbContext.Entry(config).State = EntityState.Detached;
        var reloaded = await dbContext.TenantPricingConfigurations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == config.Id);

        reloaded.Should().NotBeNull();
        reloaded!.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task DeleteConfig_DoesNotAffectOtherTenants()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        await using var dbContextA = _fixture.GetDbContext(tenantA);
        await using var dbContextB = _fixture.GetDbContext(tenantB);

        // Create configs for both tenants
        var configA = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        var configB = new TenantPricingConfiguration
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            DiscountTierCount = 4,
            MarginTierCount = 5,
            DefaultIvaEnabled = false,
            IvaPercentage = 16.00m
        };

        dbContextA.TenantPricingConfigurations.Add(configA);
        await dbContextA.SaveChangesAsync();

        dbContextB.TenantPricingConfigurations.Add(configB);
        await dbContextB.SaveChangesAsync();

        // Act - Delete Tenant A's config
        dbContextA.TenantPricingConfigurations.Remove(configA);
        await dbContextA.SaveChangesAsync();

        // Assert - Tenant A config deleted
        var deletedConfig = await dbContextA.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == configA.Id);
        deletedConfig.Should().BeNull();

        // Assert - Tenant B config unaffected
        var tenantBConfig = await dbContextB.TenantPricingConfigurations
            .FirstOrDefaultAsync(c => c.Id == configB.Id);
        tenantBConfig.Should().NotBeNull();
        tenantBConfig!.TenantId.Should().Be(tenantB);
    }
}
