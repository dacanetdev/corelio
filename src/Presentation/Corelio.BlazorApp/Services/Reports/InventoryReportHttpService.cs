using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;
using Corelio.BlazorApp.Services.Http;
using Microsoft.Extensions.Logging;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP client implementation for the inventory valuation report API.
/// </summary>
public class InventoryReportHttpService(
    AuthenticatedHttpClient httpClient,
    ILogger<InventoryReportHttpService> logger) : IInventoryReportHttpService
{
    private const string BaseUrl = "/api/v1/reports/inventory";

    public async Task<Result<InventoryValuationReportModel>> GetInventoryValuationReportAsync(
        Guid? warehouseId = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>();
            if (warehouseId.HasValue) { queryParams.Add($"warehouseId={warehouseId.Value}"); }
            if (categoryId.HasValue) { queryParams.Add($"categoryId={categoryId.Value}"); }

            var url = queryParams.Count > 0 ? $"{BaseUrl}?{string.Join("&", queryParams)}" : BaseUrl;
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<InventoryValuationReportModel>(
                    JsonOptions.Default, cancellationToken);
                return result is not null
                    ? Result<InventoryValuationReportModel>.Success(result)
                    : Result<InventoryValuationReportModel>.Failure("No se pudo deserializar el reporte.");
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<InventoryValuationReportModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener el reporte de inventario");
            return Result<InventoryValuationReportModel>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportInventoryReportAsync(
        string format,
        Guid? warehouseId = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string> { $"format={format}" };
            if (warehouseId.HasValue) { queryParams.Add($"warehouseId={warehouseId.Value}"); }
            if (categoryId.HasValue) { queryParams.Add($"categoryId={categoryId.Value}"); }

            var url = $"{BaseUrl}/export?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                return Result<byte[]>.Success(bytes);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<byte[]>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al exportar el reporte de inventario");
            return Result<byte[]>.Failure($"Error: {ex.Message}");
        }
    }
}
