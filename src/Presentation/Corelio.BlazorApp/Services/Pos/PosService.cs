using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Pos;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Pos;

/// <summary>
/// POS service implementation using AuthenticatedHttpClient.
/// </summary>
public class PosService(AuthenticatedHttpClient httpClient, ILogger<PosService> logger) : IPosService
{
    private const string BaseUrl = "/api/v1/pos";

    public async Task<Result<IEnumerable<PosProductModel>>> SearchProductsAsync(
        string term,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{BaseUrl}/search?q={Uri.EscapeDataString(term)}&limit={limit}";
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadFromJsonAsync<IEnumerable<PosProductModel>>(
                    JsonOptions.Default, cancellationToken);
                return Result<IEnumerable<PosProductModel>>.Success(results ?? []);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<IEnumerable<PosProductModel>>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching POS products");
            return Result<IEnumerable<PosProductModel>>.Failure($"Error searching products: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> CreateSaleAsync(
        CreateSaleRequestModel request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/sales", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CreateSaleResponse>(
                    JsonOptions.Default, cancellationToken);
                if (result is not null)
                {
                    return Result<Guid>.Success(result.SaleId);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<Guid>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating sale");
            return Result<Guid>.Failure($"Error creating sale: {ex.Message}");
        }
    }

    public async Task<Result<SaleModel>> GetSaleAsync(
        Guid saleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/sales/{saleId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var sale = await response.Content.ReadFromJsonAsync<SaleModel>(
                    JsonOptions.Default, cancellationToken);
                if (sale is not null)
                {
                    return Result<SaleModel>.Success(sale);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<SaleModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading sale {SaleId}", saleId);
            return Result<SaleModel>.Failure($"Error loading sale: {ex.Message}");
        }
    }

    public async Task<Result<SaleModel>> CompleteSaleAsync(
        Guid saleId,
        List<PaymentRequestModel> payments,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new { Payments = payments };
            var response = await httpClient.PostAsJsonAsync(
                $"{BaseUrl}/sales/{saleId}/complete", body, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var sale = await response.Content.ReadFromJsonAsync<SaleModel>(
                    JsonOptions.Default, cancellationToken);
                if (sale is not null)
                {
                    return Result<SaleModel>.Success(sale);
                }
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<SaleModel>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error completing sale {SaleId}", saleId);
            return Result<SaleModel>.Failure($"Error completing sale: {ex.Message}");
        }
    }

    public async Task<Result<bool>> CancelSaleAsync(
        Guid saleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/sales/{saleId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result<bool>.Success(true);
            }

            var error = await response.GetErrorMessageAsync(cancellationToken);
            return Result<bool>.Failure(error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling sale {SaleId}", saleId);
            return Result<bool>.Failure($"Error cancelling sale: {ex.Message}");
        }
    }

    private sealed record CreateSaleResponse(Guid SaleId);
}
