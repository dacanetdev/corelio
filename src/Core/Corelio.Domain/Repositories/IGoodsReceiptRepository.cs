using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for the GoodsReceipt aggregate.
/// </summary>
public interface IGoodsReceiptRepository
{
    Task<GoodsReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(List<GoodsReceipt> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? purchaseOrderId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);

    void Add(GoodsReceipt goodsReceipt);
}
