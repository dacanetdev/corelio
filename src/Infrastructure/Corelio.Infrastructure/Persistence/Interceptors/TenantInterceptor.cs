using Corelio.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Corelio.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that automatically sets TenantId on entities implementing ITenantEntity.
/// </summary>
public class TenantInterceptor(ITenantProvider tenantProvider) : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SetTenantOnEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetTenantOnEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetTenantOnEntities(DbContext? context)
    {
        if (context is null || !tenantProvider.HasTenantContext)
        {
            return;
        }

        var tenantId = tenantProvider.TenantId;

        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                // Always set TenantId on new entities
                entry.Entity.TenantId = tenantId;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Prevent changing TenantId on existing entities
                entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
            }
        }
    }
}
