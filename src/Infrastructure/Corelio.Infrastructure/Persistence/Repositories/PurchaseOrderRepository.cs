using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for PurchaseOrder aggregate.
/// </summary>
public class PurchaseOrderRepository(ApplicationDbContext context) : IPurchaseOrderRepository
{
    public async Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default)
    {
        var count = await context.PurchaseOrders
            .IgnoreQueryFilters()
            .CountAsync(po => po.CreatedAt.Year == year, cancellationToken);

        return count + 1;
    }

    public async Task<(List<PurchaseOrder> Items, int TotalCount)> GetPagedAsync(
        int page,
        int size,
        PurchaseOrderStatus? status,
        Guid? supplierId,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        CancellationToken cancellationToken = default)
    {
        var query = context.PurchaseOrders
            .Include(po => po.Supplier)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(po => po.Status == status.Value);
        }

        if (supplierId.HasValue)
        {
            query = query.Where(po => po.SupplierId == supplierId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(po => po.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(po => po.CreatedAt <= endDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(po => po.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(PurchaseOrder purchaseOrder)
    {
        context.PurchaseOrders.Add(purchaseOrder);
    }

    public void Update(PurchaseOrder purchaseOrder)
    {
        context.PurchaseOrders.Update(purchaseOrder);
    }

    public void Delete(PurchaseOrder purchaseOrder)
    {
        purchaseOrder.IsDeleted = true;
        purchaseOrder.DeletedAt = DateTime.UtcNow;
        context.PurchaseOrders.Update(purchaseOrder);
    }
}
