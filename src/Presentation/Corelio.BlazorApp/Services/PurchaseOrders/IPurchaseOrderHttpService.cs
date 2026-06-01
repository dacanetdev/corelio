using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.PurchaseOrders;
using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Services.PurchaseOrders;

/// <summary>
/// Service for purchase order and goods receipt API calls.
/// </summary>
public interface IPurchaseOrderHttpService
{
    Task<Result<PagedResult<PurchaseOrderListModel>>> GetPurchaseOrdersAsync(
        int pageNumber = 1,
        int pageSize = 20,
        PurchaseOrderStatus? status = null,
        Guid? supplierId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task<Result<PurchaseOrderModel>> GetPurchaseOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreatePurchaseOrderAsync(
        PurchaseOrderFormModel model,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> UpdatePurchaseOrderAsync(
        Guid id,
        PurchaseOrderFormModel model,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> SubmitAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> ApproveAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> ReceiveGoodsAsync(
        ReceiveGoodsFormModel model,
        Guid purchaseOrderId,
        CancellationToken cancellationToken = default);
}
