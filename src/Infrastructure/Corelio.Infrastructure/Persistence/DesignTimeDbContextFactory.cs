using Corelio.Domain.Common.Interfaces;
using Corelio.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Corelio.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances for EF Core tooling (migrations, etc.).
/// This factory is only used during development for EF Core CLI commands.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use a design-time connection string
        // In production, this is configured through Aspire
        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=corelio_dev;Username=postgres;Password=postgres",
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

        // Create design-time implementations of required services
        var tenantProvider = new DesignTimeTenantProvider();
        var currentUserProvider = new DesignTimeCurrentUserProvider();
        var tenantInterceptor = new TenantInterceptor(tenantProvider);
        var auditInterceptor = new AuditInterceptor(currentUserProvider);

        return new ApplicationDbContext(optionsBuilder.Options, tenantProvider, tenantInterceptor, auditInterceptor);
    }
}

/// <summary>
/// Design-time implementation of ITenantProvider that provides no tenant context.
/// Used only for EF Core migrations and tooling.
/// </summary>
sealed file class DesignTimeTenantProvider : ITenantProvider
{
    public Guid TenantId => Guid.Empty;
    public bool HasTenantContext => false;
}

/// <summary>
/// Design-time implementation of ICurrentUserProvider that provides no user context.
/// Used only for EF Core migrations and tooling.
/// </summary>
sealed file class DesignTimeCurrentUserProvider : ICurrentUserProvider
{
    public Guid? UserId => null;
}
