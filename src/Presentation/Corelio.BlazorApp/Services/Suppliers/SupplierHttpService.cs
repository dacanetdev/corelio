using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Suppliers;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Suppliers;

/// <summary>
/// Implementation of supplier HTTP service.
/// </summary>
public class SupplierHttpService(AuthenticatedHttpClient httpClient, ILogger<SupplierHttpService> logger)
    : ISupplierHttpService
{
    private const string BaseUrl = "/api/v1/suppliers";

    public async Task<Result<PagedResult<SupplierListModel>>> GetSuppliersAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"pageNumber={pageNumber}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
            }

            if (isActive.HasValue)
            {
                queryParams.Add($"isActive={isActive.Value.ToString().ToLowerInvariant()}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<SupplierListModel>>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<SupplierListModel>>.Success(result);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<SupplierListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading suppliers");
            return Result<PagedResult<SupplierListModel>>.Failure($"Error loading suppliers: {ex.Message}");
        }
    }

    public async Task<Result<SupplierModel>> GetSupplierByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var model = await response.Content.ReadFromJsonAsync<SupplierModel>(
                    JsonOptions.Default, cancellationToken);
                if (model is not null)
                {
                    return Result<SupplierModel>.Success(model);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<SupplierModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading supplier {Id}", id);
            return Result<SupplierModel>.Failure($"Error loading supplier: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> CreateSupplierAsync(
        SupplierFormModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(BaseUrl, model, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateSupplierResponse>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<Guid>.Success(result.SupplierId);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating supplier");
            return Result<Guid>.Failure($"Error creating supplier: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateSupplierAsync(
        Guid id,
        SupplierFormModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", model, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating supplier {Id}", id);
            return Result<bool>.Failure($"Error updating supplier: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteSupplierAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting supplier {Id}", id);
            return Result<bool>.Failure($"Error deleting supplier: {ex.Message}");
        }
    }

    private sealed record CreateSupplierResponse(Guid SupplierId);
}
