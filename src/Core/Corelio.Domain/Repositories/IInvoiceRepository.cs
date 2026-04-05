using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for the Invoice aggregate.
/// </summary>
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByUuidAsync(string uuid, CancellationToken cancellationToken = default);

    Task<(List<Invoice> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CfdiStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    void Add(Invoice invoice);
    void Update(Invoice invoice);
}
