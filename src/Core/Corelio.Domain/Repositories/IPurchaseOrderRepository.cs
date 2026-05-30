using Corelio.Domain.Entities;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for PurchaseOrder aggregate operations.
/// </summary>
public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns the next sequence number for a given tenant+year (used to generate OrderNumber).</summary>
    Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default);

    Task<(List<PurchaseOrder> Items, int TotalCount)> GetPagedAsync(
        int page,
        int size,
        PurchaseOrderStatus? status,
        Guid? supplierId,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        CancellationToken cancellationToken = default);

    void Add(PurchaseOrder purchaseOrder);
    void Update(PurchaseOrder purchaseOrder);
    void Delete(PurchaseOrder purchaseOrder);
}
