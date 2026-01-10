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
    ITenantProvider tenantProvider,
    TenantInterceptor tenantInterceptor,
    AuditInterceptor auditInterceptor) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(tenantInterceptor, auditInterceptor);
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply multi-tenancy query filters
        ApplyTenantQueryFilters(modelBuilder);
    }

    /// <summary>
    /// Applies global query filters for multi-tenancy.
    /// Filters are applied when tenant context exists, bypassed for system operations.
    /// </summary>
    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
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
    }
}
