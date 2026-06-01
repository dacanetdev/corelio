using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.PurchaseOrders;
using Corelio.BlazorApp.Services.Http;
using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Services.PurchaseOrders;

/// <summary>
/// Implementation of purchase order HTTP service.
/// </summary>
public class PurchaseOrderHttpService(
    AuthenticatedHttpClient httpClient,
    ILogger<PurchaseOrderHttpService> logger)
    : IPurchaseOrderHttpService
{
    private const string BaseUrl = "/api/v1/purchase-orders";
    private const string GoodsReceiptsUrl = "/api/v1/goods-receipts";

    public async Task<Result<PagedResult<PurchaseOrderListModel>>> GetPurchaseOrdersAsync(
        int pageNumber = 1,
        int pageSize = 20,
        PurchaseOrderStatus? status = null,
        Guid? supplierId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };

            if (status.HasValue)
            {
                queryParams.Add($"status={(int)status.Value}");
            }

            if (supplierId.HasValue)
            {
                queryParams.Add($"supplierId={supplierId.Value}");
            }

            if (startDate.HasValue)
            {
                queryParams.Add($"startDate={startDate.Value:O}");
            }

            if (endDate.HasValue)
            {
                queryParams.Add($"endDate={endDate.Value:O}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<PurchaseOrderListModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<PurchaseOrderListModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<PurchaseOrderListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading purchase orders");
            return Result<PagedResult<PurchaseOrderListModel>>.Failure($"Error loading purchase orders: {ex.Message}");
        }
    }

    public async Task<Result<PurchaseOrderModel>> GetPurchaseOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<PurchaseOrderModel>(
                    JsonOptions.Default, cancellationToken);
                if (model is not null)
                {
                    return Result<PurchaseOrderModel>.Success(model);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PurchaseOrderModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading purchase order {Id}", id);
            return Result<PurchaseOrderModel>.Failure($"Error loading purchase order: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> CreatePurchaseOrderAsync(
        PurchaseOrderFormModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new CreatePurchaseOrderRequest(
                model.SupplierId,
                model.ExpectedDate.HasValue ? new DateTimeOffset(model.ExpectedDate.Value) : null,
                model.Notes,
                model.Items.Select(i => new PurchaseOrderItemLineRequest(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList());

            var response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreatePurchaseOrderResponse>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<Guid>.Success(result.PurchaseOrderId);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating purchase order");
            return Result<Guid>.Failure($"Error creating purchase order: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdatePurchaseOrderAsync(
        Guid id,
        PurchaseOrderFormModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UpdatePurchaseOrderRequest(
                model.SupplierId,
                model.ExpectedDate.HasValue ? new DateTimeOffset(model.ExpectedDate.Value) : null,
                model.Notes,
                model.Items.Select(i => new PurchaseOrderItemLineRequest(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList());

            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating purchase order {Id}", id);
            return Result<bool>.Failure($"Error updating purchase order: {ex.Message}");
        }
    }

    public async Task<Result<bool>> SubmitAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{id}/submit", new { }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error submitting purchase order {Id}", id);
            return Result<bool>.Failure($"Error submitting purchase order: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ApproveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{id}/approve", new { }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error approving purchase order {Id}", id);
            return Result<bool>.Failure($"Error approving purchase order: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{id}/cancel", new { }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling purchase order {Id}", id);
            return Result<bool>.Failure($"Error cancelling purchase order: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> ReceiveGoodsAsync(
        ReceiveGoodsFormModel model,
        Guid purchaseOrderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ReceiveGoodsRequest(
                purchaseOrderId,
                model.WarehouseId,
                DateOnly.FromDateTime(model.ReceivedDate),
                model.Notes,
                model.Items
                    .Where(i => i.QuantityToReceive > 0)
                    .Select(i => new ReceiveGoodsItemBody(i.PurchaseOrderItemId, i.QuantityToReceive))
                    .ToList());

            var response = await httpClient.PostAsJsonAsync(GoodsReceiptsUrl, request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ReceiveGoodsResponse>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<Guid>.Success(result.GoodsReceiptId);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error receiving goods for purchase order {PurchaseOrderId}", purchaseOrderId);
            return Result<Guid>.Failure($"Error receiving goods: {ex.Message}");
        }
    }

    private sealed record PurchaseOrderItemLineRequest(Guid ProductId, string ProductName, decimal Quantity, decimal UnitPrice);
    private sealed record CreatePurchaseOrderRequest(Guid SupplierId, DateTimeOffset? ExpectedDate, string? Notes, List<PurchaseOrderItemLineRequest> Items);
    private sealed record UpdatePurchaseOrderRequest(Guid SupplierId, DateTimeOffset? ExpectedDate, string? Notes, List<PurchaseOrderItemLineRequest> Items);
    private sealed record ReceiveGoodsItemBody(Guid PurchaseOrderItemId, decimal QuantityReceived);
    private sealed record ReceiveGoodsRequest(Guid PurchaseOrderId, Guid WarehouseId, DateOnly ReceivedDate, string? Notes, List<ReceiveGoodsItemBody> Items);
    private sealed record CreatePurchaseOrderResponse(Guid PurchaseOrderId);
    private sealed record ReceiveGoodsResponse(Guid GoodsReceiptId);
}
