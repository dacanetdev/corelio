using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for Supplier entity operations.
/// </summary>
public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RfcExistsAsync(string rfc, Guid? excludeId, CancellationToken cancellationToken = default);
    Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(
        int page,
        int size,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);
    void Add(Supplier supplier);
    void Update(Supplier supplier);
    void Delete(Supplier supplier);
}
