using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Pos;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Sales;

/// <summary>
/// Implementation of sale service using AuthenticatedHttpClient.
/// </summary>
public class SaleService(AuthenticatedHttpClient httpClient, ILogger<SaleService> logger) : ISaleService
{
    private const string BaseUrl = "/api/v1/sales";

    public async Task<Result<PagedResult<SaleListModel>>> GetSalesAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                queryParams.Add($"status={Uri.EscapeDataString(status)}");
            }

            if (dateFrom.HasValue)
            {
                queryParams.Add($"dateFrom={dateFrom.Value:O}");
            }

            if (dateTo.HasValue)
            {
                queryParams.Add($"dateTo={dateTo.Value:O}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<SaleListModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<SaleListModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<SaleListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading sales");
            return Result<PagedResult<SaleListModel>>.Failure($"Error loading sales: {ex.Message}");
        }
    }

    public async Task<Result<SaleModel>> GetSaleByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<SaleModel>(
                    JsonOptions.Default, cancellationToken);
                if (model is not null)
                {
                    return Result<SaleModel>.Success(model);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<SaleModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading sale {Id}", id);
            return Result<SaleModel>.Failure($"Error loading sale: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CancelSaleAsync(
        Guid id,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{BaseUrl}/{id}";
            if (!string.IsNullOrWhiteSpace(reason))
            {
                url += $"?reason={Uri.EscapeDataString(reason)}";
            }

            var response = await httpClient.DeleteAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling sale {Id}", id);
            return Result<bool>.Failure($"Error cancelling sale: {ex.Message}");
        }
    }
}
