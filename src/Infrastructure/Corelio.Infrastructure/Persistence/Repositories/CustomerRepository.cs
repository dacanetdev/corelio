using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Customer aggregate.
/// </summary>
public class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLowerInvariant();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(lower) ||
                c.LastName.ToLower().Contains(lower) ||
                (c.BusinessName != null && c.BusinessName.ToLower().Contains(lower)) ||
                (c.Rfc != null && c.Rfc.ToLower().Contains(lower)) ||
                (c.Email != null && c.Email.ToLower().Contains(lower)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.CustomerType == Domain.Enums.CustomerType.Business ? c.BusinessName : c.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<Customer>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        var lower = term.ToLowerInvariant();

        return await context.Customers
            .Where(c =>
                c.FirstName.ToLower().Contains(lower) ||
                c.LastName.ToLower().Contains(lower) ||
                (c.BusinessName != null && c.BusinessName.ToLower().Contains(lower)) ||
                (c.Rfc != null && c.Rfc.ToLower().Contains(lower)))
            .OrderBy(c => c.FirstName)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> RfcExistsAsync(string rfc, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Customers.Where(c => c.Rfc == rfc);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public void Add(Customer customer)
    {
        context.Customers.Add(customer);
    }

    public void Update(Customer customer)
    {
        context.Customers.Update(customer);
    }

    public void Delete(Customer customer)
    {
        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        context.Customers.Update(customer);
    }
}
