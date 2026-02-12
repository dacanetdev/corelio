using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Pricing;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Pricing;

/// <summary>
/// Implementation of pricing service using AuthenticatedHttpClient to call backend API.
/// </summary>
public partial class PricingService(AuthenticatedHttpClient httpClient, ILogger<PricingService> logger) : IPricingService
{
    private const string BaseUrl = "/api/v1/pricing";

    /// <inheritdoc />
    public async Task<Result<TenantPricingConfigModel>> GetTenantConfigAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            LogGettingTenantConfig(logger, $"{BaseUrl}/tenant-config");
            var response = await httpClient.GetAsync($"{BaseUrl}/tenant-config", cancellationToken);
            LogGetTenantConfigResponse(logger, response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var config = await response.Content.ReadFromJsonAsync<TenantPricingConfigModel>(
                    cancellationToken: cancellationToken);
                if (config is not null)
                {
                    return Result<TenantPricingConfigModel>.Success(config);
                }
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            LogGetTenantConfigFailed(logger, response.StatusCode, errorMessage);
            return Result<TenantPricingConfigModel>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            LogTenantConfigException(logger, ex);
            return Result<TenantPricingConfigModel>.Failure($"Error loading pricing configuration: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<TenantPricingConfigModel>> UpdateTenantConfigAsync(
        TenantPricingConfigModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new
            {
                model.DiscountTierCount,
                model.MarginTierCount,
                model.DefaultIvaEnabled,
                model.IvaPercentage,
                DiscountTiers = model.DiscountTiers.Select(t => new
                {
                    t.TierNumber,
                    t.TierName,
                    t.IsActive
                }),
                MarginTiers = model.MarginTiers.Select(t => new
                {
                    t.TierNumber,
                    t.TierName,
                    t.IsActive
                })
            };

            LogUpdatingTenantConfig(logger, model.DiscountTierCount, model.MarginTierCount, model.IvaPercentage);
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/tenant-config", request, cancellationToken);
            LogPutTenantConfigResponse(logger, response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var config = await response.Content.ReadFromJsonAsync<TenantPricingConfigModel>(
                    cancellationToken: cancellationToken);
                if (config is not null)
                {
                    return Result<TenantPricingConfigModel>.Success(config);
                }

                LogPutTenantConfigNullBody(logger);
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            LogPutTenantConfigFailed(logger, response.StatusCode, errorMessage);
            return Result<TenantPricingConfigModel>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            LogUpdateTenantConfigException(logger, ex);
            return Result<TenantPricingConfigModel>.Failure($"Error updating pricing configuration: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<ProductPricingModel>>> GetProductsPricingAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        Guid? categoryId = null,
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

            if (categoryId.HasValue)
            {
                queryParams.Add($"categoryId={categoryId.Value}");
            }

            var queryString = string.Join("&", queryParams);
            var response = await httpClient.GetAsync($"{BaseUrl}/products?{queryString}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductPricingModel>>(
                    cancellationToken: cancellationToken);
                if (result is not null)
                {
                    return Result<PagedResult<ProductPricingModel>>.Success(result);
                }
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PagedResult<ProductPricingModel>>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ProductPricingModel>>.Failure($"Error loading products pricing: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<ProductPricingModel>> GetProductPricingAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/products/{productId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductPricingModel>(
                    cancellationToken: cancellationToken);
                if (product is not null)
                {
                    return Result<ProductPricingModel>.Success(product);
                }
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            return Result<ProductPricingModel>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<ProductPricingModel>.Failure($"Error loading product pricing: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<ProductPricingModel>> UpdateProductPricingAsync(
        Guid productId,
        decimal? listPrice,
        bool ivaEnabled,
        List<ProductDiscountModel> discounts,
        List<ProductMarginPriceModel> marginPrices,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new
            {
                ListPrice = listPrice,
                IvaEnabled = ivaEnabled,
                Discounts = discounts.Select(d => new
                {
                    d.TierNumber,
                    DiscountPercentage = d.DiscountPercentage
                }),
                MarginPrices = marginPrices.Select(m => new
                {
                    m.TierNumber,
                    m.MarginPercentage,
                    m.SalePrice
                })
            };

            var response = await httpClient.PutAsJsonAsync(
                $"{BaseUrl}/products/{productId}", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductPricingModel>(
                    cancellationToken: cancellationToken);
                if (product is not null)
                {
                    return Result<ProductPricingModel>.Success(product);
                }
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            return Result<ProductPricingModel>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<ProductPricingModel>.Failure($"Error updating product pricing: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<PricingCalculationResultModel>> CalculatePricesAsync(
        decimal listPrice,
        List<decimal> discounts,
        bool ivaEnabled,
        decimal ivaPercentage = 16.00m,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new
            {
                ListPrice = listPrice,
                Discounts = discounts,
                IvaEnabled = ivaEnabled,
                IvaPercentage = ivaPercentage
            };

            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/calculate", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PricingCalculationResultModel>(
                    cancellationToken: cancellationToken);
                if (result is not null)
                {
                    return Result<PricingCalculationResultModel>.Success(result);
                }
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            return Result<PricingCalculationResultModel>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<PricingCalculationResultModel>.Failure($"Error calculating prices: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<int>> BulkUpdatePricingAsync(
        BulkUpdateRequestModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new
            {
                model.ProductIds,
                model.UpdateType,
                model.Value,
                model.TierNumber
            };

            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/bulk-update", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var updatedCount = await response.Content.ReadFromJsonAsync<int>(
                    cancellationToken: cancellationToken);
                return Result<int>.Success(updatedCount);
            }

            var errorMessage = await response.GetErrorMessageAsync(cancellationToken);
            return Result<int>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error applying bulk pricing update: {ex.Message}");
        }
    }

    // High-performance logging via LoggerMessage source generator

    [LoggerMessage(Level = LogLevel.Information, Message = "Getting tenant pricing config from {Url}")]
    private static partial void LogGettingTenantConfig(ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Information, Message = "GET tenant-config response: {StatusCode}")]
    private static partial void LogGetTenantConfigResponse(ILogger logger, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(Level = LogLevel.Warning, Message = "GET tenant-config failed: {StatusCode} - {Error}")]
    private static partial void LogGetTenantConfigFailed(ILogger logger, System.Net.HttpStatusCode statusCode, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Exception loading pricing configuration")]
    private static partial void LogTenantConfigException(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Updating tenant pricing config: {DiscountTiers} discount tiers, {MarginTiers} margin tiers, IVA={IvaPercentage}%")]
    private static partial void LogUpdatingTenantConfig(ILogger logger, int discountTiers, int marginTiers, decimal ivaPercentage);

    [LoggerMessage(Level = LogLevel.Information, Message = "PUT tenant-config response: {StatusCode}")]
    private static partial void LogPutTenantConfigResponse(ILogger logger, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(Level = LogLevel.Warning, Message = "PUT tenant-config returned success but response body was null")]
    private static partial void LogPutTenantConfigNullBody(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "PUT tenant-config failed: {StatusCode} - {Error}")]
    private static partial void LogPutTenantConfigFailed(ILogger logger, System.Net.HttpStatusCode statusCode, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Exception updating pricing configuration")]
    private static partial void LogUpdateTenantConfigException(ILogger logger, Exception ex);
}
