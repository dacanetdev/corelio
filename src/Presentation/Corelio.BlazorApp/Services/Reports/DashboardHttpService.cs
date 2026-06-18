using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;
using Corelio.BlazorApp.Services.Http;
using Microsoft.Extensions.Logging;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP client implementation for the dashboard KPI summary API.
/// </summary>
public class DashboardHttpService(
    AuthenticatedHttpClient httpClient,
    ILogger<DashboardHttpService> logger) : IDashboardHttpService
{
    private const string BaseUrl = "/api/v1/reports/dashboard";

    public async Task<Result<DashboardSummaryModel>> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync(BaseUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DashboardSummaryModel>(
                    JsonOptions.Default, cancellationToken);

                return result is not null
                    ? Result<DashboardSummaryModel>.Success(result)
                    : Result<DashboardSummaryModel>.Failure("No se pudo deserializar el resumen del tablero.");
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<DashboardSummaryModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener el resumen del tablero");
            return Result<DashboardSummaryModel>.Failure($"Error: {ex.Message}");
        }
    }
}
