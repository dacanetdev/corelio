using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Inventory;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Inventory;

/// <summary>
/// Implementation of inventory service using AuthenticatedHttpClient.
/// </summary>
public class InventoryService(AuthenticatedHttpClient httpClient, ILogger<InventoryService> logger)
    : IInventoryService
{
    private const string BaseUrl = "/api/v1/inventory";

    public async Task<Result<PagedResult<InventoryItemModel>>> GetInventoryItemsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        Guid? warehouseId = null,
        bool lowStockOnly = false,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}",
                $"lowStockOnly={lowStockOnly}"
            };

            if (warehouseId.HasValue)
            {
                queryParams.Add($"warehouseId={warehouseId.Value}");
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<InventoryItemModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<InventoryItemModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<InventoryItemModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading inventory items");
            return Result<PagedResult<InventoryItemModel>>.Failure($"Error loading inventory: {ex.Message}");
        }
    }

    public async Task<Result<List<WarehouseModel>>> GetWarehousesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/warehouses", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var warehouses = await response.Content.ReadFromJsonAsync<List<WarehouseModel>>(
                    JsonOptions.Default, cancellationToken);
                if (warehouses is not null)
                {
                    return Result<List<WarehouseModel>>.Success(warehouses);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<List<WarehouseModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading warehouses");
            return Result<List<WarehouseModel>>.Failure($"Error loading warehouses: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<InventoryTransactionModel>>> GetTransactionsAsync(
        Guid inventoryItemId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{BaseUrl}/{inventoryItemId}/transactions?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<InventoryTransactionModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<InventoryTransactionModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<InventoryTransactionModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading transactions for item {Id}", inventoryItemId);
            return Result<PagedResult<InventoryTransactionModel>>.Failure($"Error loading transactions: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AdjustStockAsync(
        AdjustStockRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/adjustments", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adjusting stock for item {Id}", request.InventoryItemId);
            return Result<bool>.Failure($"Error adjusting stock: {ex.Message}");
        }
    }
}
