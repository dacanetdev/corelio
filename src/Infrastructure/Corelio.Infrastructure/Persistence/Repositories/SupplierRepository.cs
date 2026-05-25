using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Supplier aggregate.
/// </summary>
public class SupplierRepository(ApplicationDbContext context) : ISupplierRepository
{
    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> RfcExistsAsync(string rfc, Guid? excludeId, CancellationToken cancellationToken = default)
    {
        var query = context.Suppliers.Where(s => s.Rfc == rfc);

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(
        int page,
        int size,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLowerInvariant();
            query = query.Where(s =>
                s.Name.ToLower().Contains(lower) ||
                (s.Rfc != null && s.Rfc.ToLower().Contains(lower)) ||
                (s.ContactName != null && s.ContactName.ToLower().Contains(lower)) ||
                (s.Email != null && s.Email.ToLower().Contains(lower)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Supplier supplier)
    {
        context.Suppliers.Add(supplier);
    }

    public void Update(Supplier supplier)
    {
        context.Suppliers.Update(supplier);
    }

    public void Delete(Supplier supplier)
    {
        supplier.IsDeleted = true;
        supplier.DeletedAt = DateTime.UtcNow;
        context.Suppliers.Update(supplier);
    }
}
