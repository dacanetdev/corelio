using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Sale aggregate.
/// </summary>
public class SaleRepository(ApplicationDbContext context) : ISaleRepository
{
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Sales
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.Payments)
            .Include(s => s.Customer)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<(List<Sale> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        SaleStatus? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Sales
            .Include(s => s.Customer)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(s => s.CreatedAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(s => s.CreatedAt <= dateTo.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<int> GetNextFolioNumberAsync(CancellationToken cancellationToken = default)
    {
        var count = await context.Sales.CountAsync(cancellationToken);
        return count + 1;
    }

    public void Add(Sale sale)
    {
        context.Sales.Add(sale);
    }

    public void Update(Sale sale)
    {
        context.Sales.Update(sale);
    }
}
