using System.Net.Http.Json;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Pricing;

namespace Corelio.BlazorApp.Services.Pricing;

/// <summary>
/// Implementation of pricing service using HttpClient to call backend API.
/// </summary>
public class PricingService(HttpClient httpClient) : IPricingService
{
    private const string BaseUrl = "/api/v1/pricing";

    /// <inheritdoc />
    public async Task<Result<TenantPricingConfigModel>> GetTenantConfigAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/tenant-config", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var config = await response.Content.ReadFromJsonAsync<TenantPricingConfigModel>(
                    cancellationToken: cancellationToken);
                if (config is not null)
                {
                    return Result<TenantPricingConfigModel>.Success(config);
                }
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<TenantPricingConfigModel>.Failure(errorMessage ?? "Failed to load pricing configuration");
        }
        catch (Exception ex)
        {
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

            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/tenant-config", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var config = await response.Content.ReadFromJsonAsync<TenantPricingConfigModel>(
                    cancellationToken: cancellationToken);
                if (config is not null)
                {
                    return Result<TenantPricingConfigModel>.Success(config);
                }
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<TenantPricingConfigModel>.Failure(errorMessage ?? "Failed to update pricing configuration");
        }
        catch (Exception ex)
        {
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

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<PagedResult<ProductPricingModel>>.Failure(errorMessage ?? "Failed to load products pricing");
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

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<ProductPricingModel>.Failure(errorMessage ?? "Product pricing not found");
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

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<ProductPricingModel>.Failure(errorMessage ?? "Failed to update product pricing");
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

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<PricingCalculationResultModel>.Failure(errorMessage ?? "Failed to calculate prices");
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

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<int>.Failure(errorMessage ?? "Failed to apply bulk pricing update");
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error applying bulk pricing update: {ex.Message}");
        }
    }
}
