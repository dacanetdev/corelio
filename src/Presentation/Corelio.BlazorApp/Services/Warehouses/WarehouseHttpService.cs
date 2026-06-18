using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Warehouses;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Warehouses;

public class WarehouseHttpService(AuthenticatedHttpClient httpClient, ILogger<WarehouseHttpService> logger)
    : IWarehouseHttpService
{
    private const string ListUrl = "/api/v1/inventory/warehouses";
    private const string CrudUrl = "/api/v1/warehouses";

    public async Task<Result<List<WarehouseListModel>>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync(ListUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<List<WarehouseListModel>>(
                    JsonOptions.Default, cancellationToken);
                return Result<List<WarehouseListModel>>.Success(items ?? []);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<List<WarehouseListModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading warehouses");
            return Result<List<WarehouseListModel>>.Failure($"Error loading warehouses: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> CreateWarehouseAsync(WarehouseFormModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(CrudUrl, new
            {
                model.Name,
                model.Type,
                model.IsDefault
            }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var id = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions.Default, cancellationToken);
                return Result<Guid>.Success(id);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating warehouse");
            return Result<Guid>.Failure($"Error creating warehouse: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateWarehouseAsync(Guid id, WarehouseFormModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"{CrudUrl}/{id}", new
            {
                model.Name,
                model.Type,
                model.IsDefault
            }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating warehouse {Id}", id);
            return Result<bool>.Failure($"Error updating warehouse: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteWarehouseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"{CrudUrl}/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting warehouse {Id}", id);
            return Result<bool>.Failure($"Error deleting warehouse: {ex.Message}");
        }
    }
}
