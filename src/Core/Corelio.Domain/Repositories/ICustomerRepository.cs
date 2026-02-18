using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for Customer aggregate.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Gets a customer by its ID.
    /// </summary>
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of customers with optional search filter.
    /// </summary>
    Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name or RFC (for POS quick search).
    /// </summary>
    Task<List<Customer>> SearchAsync(string term, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an RFC already exists for another customer.
    /// </summary>
    Task<bool> RfcExistsAsync(string rfc, Guid? excludeId = null, CancellationToken cancellationToken = default);

    void Add(Customer customer);
    void Update(Customer customer);
    void Delete(Customer customer);
}
