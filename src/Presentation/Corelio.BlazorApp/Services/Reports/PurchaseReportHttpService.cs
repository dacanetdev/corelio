using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;
using Corelio.BlazorApp.Services.Http;
using Microsoft.Extensions.Logging;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP client implementation for the purchase summary report API.
/// </summary>
public class PurchaseReportHttpService(
    AuthenticatedHttpClient httpClient,
    ILogger<PurchaseReportHttpService> logger) : IPurchaseReportHttpService
{
    private const string BaseUrl = "/api/v1/reports/purchases";

    public async Task<Result<PurchaseSummaryReportModel>> GetPurchaseSummaryReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? supplierId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"dateFrom={dateFrom:yyyy-MM-dd}",
                $"dateTo={dateTo:yyyy-MM-dd}"
            };

            if (supplierId.HasValue)
            {
                queryParams.Add($"supplierId={supplierId.Value}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PurchaseSummaryReportModel>(
                    JsonOptions.Default, cancellationToken);

                return result is not null
                    ? Result<PurchaseSummaryReportModel>.Success(result)
                    : Result<PurchaseSummaryReportModel>.Failure("No se pudo deserializar el reporte.");
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PurchaseSummaryReportModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener el reporte de compras");
            return Result<PurchaseSummaryReportModel>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportPurchaseReportAsync(
        string format,
        DateTime dateFrom,
        DateTime dateTo,
        Guid? supplierId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"format={format}",
                $"dateFrom={dateFrom:yyyy-MM-dd}",
                $"dateTo={dateTo:yyyy-MM-dd}"
            };

            if (supplierId.HasValue)
            {
                queryParams.Add($"supplierId={supplierId.Value}");
            }

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
            logger.LogError(ex, "Error al exportar el reporte de compras");
            return Result<byte[]>.Failure($"Error: {ex.Message}");
        }
    }
}
