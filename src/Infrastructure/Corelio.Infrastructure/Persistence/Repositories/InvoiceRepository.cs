using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for the Invoice aggregate.
/// </summary>
public class InvoiceRepository(ApplicationDbContext context) : IInvoiceRepository
{
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Sale)
                .ThenInclude(s => s!.Customer)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Invoice?> GetBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        return await context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.SaleId == saleId, cancellationToken);
    }

    public async Task<Invoice?> GetByUuidAsync(string uuid, CancellationToken cancellationToken = default)
    {
        return await context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Uuid == uuid, cancellationToken);
    }

    public async Task<(List<Invoice> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CfdiStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Invoices
            .Include(i => i.Sale)
                .ThenInclude(s => s!.Customer)
            .AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(i =>
                EF.Functions.ILike(i.Folio, $"%{searchTerm}%") ||
                EF.Functions.ILike(i.ReceiverRfc, $"%{searchTerm}%") ||
                EF.Functions.ILike(i.ReceiverName, $"%{searchTerm}%") ||
                (i.Uuid != null && EF.Functions.ILike(i.Uuid, $"%{searchTerm}%")));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Invoice invoice)
    {
        context.Invoices.Add(invoice);
    }

    public void Update(Invoice invoice)
    {
        context.Invoices.Update(invoice);
    }
}
