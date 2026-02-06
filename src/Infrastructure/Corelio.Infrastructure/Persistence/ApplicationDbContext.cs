using Corelio.Domain.Common.Interfaces;
using Corelio.Domain.Entities;
using Corelio.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence;

/// <summary>
/// Application database context with multi-tenancy support.
/// </summary>
public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ITenantProvider? tenantProvider = null,
    TenantInterceptor? tenantInterceptor = null,
    AuditInterceptor? auditInterceptor = null) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Add interceptors only if provided (not during design-time/health checks)
        if (tenantInterceptor != null && auditInterceptor != null)
        {
            optionsBuilder.AddInterceptors(tenantInterceptor, auditInterceptor);
        }
    }

    // Core entities
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantConfiguration> TenantConfigurations => Set<TenantConfiguration>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Business entities
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    // Pricing entities
    public DbSet<TenantPricingConfiguration> TenantPricingConfigurations => Set<TenantPricingConfiguration>();
    public DbSet<DiscountTierDefinition> DiscountTierDefinitions => Set<DiscountTierDefinition>();
    public DbSet<MarginTierDefinition> MarginTierDefinitions => Set<MarginTierDefinition>();
    public DbSet<ProductDiscount> ProductDiscounts => Set<ProductDiscount>();
    public DbSet<ProductMarginPrice> ProductMarginPrices => Set<ProductMarginPrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Seed initial data
        Seeds.DataSeeder.Seed(modelBuilder);

        // Apply multi-tenancy query filters
        ApplyTenantQueryFilters(modelBuilder);
    }

    /// <summary>
    /// Applies global query filters for multi-tenancy.
    /// Filters are applied when tenant context exists, bypassed for system operations.
    /// </summary>
    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        // Only apply filters if tenant provider is available (runtime, not design-time)
        if (tenantProvider == null)
        {
            return;
        }

        // User - strict tenant isolation
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !tenantProvider.HasTenantContext || u.TenantId == tenantProvider.TenantId);

        // RefreshToken - strict tenant isolation
        modelBuilder.Entity<RefreshToken>()
            .HasQueryFilter(rt => !tenantProvider.HasTenantContext || rt.TenantId == tenantProvider.TenantId);

        // TenantConfiguration - strict tenant isolation
        modelBuilder.Entity<TenantConfiguration>()
            .HasQueryFilter(tc => !tenantProvider.HasTenantContext || tc.TenantId == tenantProvider.TenantId);

        // Role - shows system roles (null TenantId) OR tenant-specific roles
        modelBuilder.Entity<Role>()
            .HasQueryFilter(r => !tenantProvider.HasTenantContext || r.TenantId == null || r.TenantId == tenantProvider.TenantId);

        // AuditLog - shows system events (null TenantId) OR tenant-specific events
        modelBuilder.Entity<AuditLog>()
            .HasQueryFilter(al => !tenantProvider.HasTenantContext || al.TenantId == null || al.TenantId == tenantProvider.TenantId);

        // Product - strict tenant isolation
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !tenantProvider.HasTenantContext || p.TenantId == tenantProvider.TenantId);

        // ProductCategory - strict tenant isolation
        modelBuilder.Entity<ProductCategory>()
            .HasQueryFilter(pc => !tenantProvider.HasTenantContext || pc.TenantId == tenantProvider.TenantId);

        // TenantPricingConfiguration - strict tenant isolation
        modelBuilder.Entity<TenantPricingConfiguration>()
            .HasQueryFilter(tpc => !tenantProvider.HasTenantContext || tpc.TenantId == tenantProvider.TenantId);

        // DiscountTierDefinition - strict tenant isolation
        modelBuilder.Entity<DiscountTierDefinition>()
            .HasQueryFilter(d => !tenantProvider.HasTenantContext || d.TenantId == tenantProvider.TenantId);

        // MarginTierDefinition - strict tenant isolation
        modelBuilder.Entity<MarginTierDefinition>()
            .HasQueryFilter(m => !tenantProvider.HasTenantContext || m.TenantId == tenantProvider.TenantId);

        // ProductDiscount - strict tenant isolation
        modelBuilder.Entity<ProductDiscount>()
            .HasQueryFilter(pd => !tenantProvider.HasTenantContext || pd.TenantId == tenantProvider.TenantId);

        // ProductMarginPrice - strict tenant isolation
        modelBuilder.Entity<ProductMarginPrice>()
            .HasQueryFilter(pm => !tenantProvider.HasTenantContext || pm.TenantId == tenantProvider.TenantId);
    }
}
