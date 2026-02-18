using Corelio.Domain.Entities;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for Sale aggregate.
/// </summary>
public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(List<Sale> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        SaleStatus? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next folio number for a tenant (for auto-generation of "V-00001").
    /// </summary>
    Task<int> GetNextFolioNumberAsync(CancellationToken cancellationToken = default);

    void Add(Sale sale);
    void Update(Sale sale);
}
