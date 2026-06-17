using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;
using Corelio.BlazorApp.Services.Http;
using Corelio.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP client implementation for the sales report API.
/// </summary>
public class SalesReportHttpService(
    AuthenticatedHttpClient httpClient,
    ILogger<SalesReportHttpService> logger) : ISalesReportHttpService
{
    private const string BaseUrl = "/api/v1/reports/sales";

    public async Task<Result<SalesReportModel>> GetSalesReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? warehouseId = null,
        PaymentMethod? paymentMethod = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"dateFrom={dateFrom:yyyy-MM-dd}",
                $"dateTo={dateTo:yyyy-MM-dd}"
            };

            if (warehouseId.HasValue)
            {
                queryParams.Add($"warehouseId={warehouseId.Value}");
            }

            if (paymentMethod.HasValue)
            {
                queryParams.Add($"paymentMethod={paymentMethod.Value}");
            }

            var url = $"{BaseUrl}?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SalesReportModel>(
                    JsonOptions.Default, cancellationToken);

                return result is not null
                    ? Result<SalesReportModel>.Success(result)
                    : Result<SalesReportModel>.Failure("No se pudo deserializar el reporte.");
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<SalesReportModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener el reporte de ventas");
            return Result<SalesReportModel>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportSalesReportAsync(
        string format,
        DateTime dateFrom,
        DateTime dateTo,
        Guid? warehouseId = null,
        PaymentMethod? paymentMethod = null,
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

            if (warehouseId.HasValue)
            {
                queryParams.Add($"warehouseId={warehouseId.Value}");
            }

            if (paymentMethod.HasValue)
            {
                queryParams.Add($"paymentMethod={paymentMethod.Value}");
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
            logger.LogError(ex, "Error al exportar el reporte de ventas");
            return Result<byte[]>.Failure($"Error: {ex.Message}");
        }
    }
}
