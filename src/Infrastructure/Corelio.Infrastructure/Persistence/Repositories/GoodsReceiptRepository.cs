using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for the GoodsReceipt aggregate.
/// </summary>
public class GoodsReceiptRepository(ApplicationDbContext context) : IGoodsReceiptRepository
{
    public async Task<GoodsReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.GoodsReceipts
            .Include(r => r.PurchaseOrder)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<(List<GoodsReceipt> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? purchaseOrderId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.GoodsReceipts
            .Include(r => r.PurchaseOrder)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .AsQueryable();

        if (purchaseOrderId.HasValue)
        {
            query = query.Where(r => r.PurchaseOrderId == purchaseOrderId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(r => r.ReceivedDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.ReceivedDate <= endDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.ReceivedDate)
            .ThenByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(GoodsReceipt goodsReceipt)
    {
        context.GoodsReceipts.Add(goodsReceipt);
    }
}
