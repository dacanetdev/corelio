using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Pricing;

namespace Corelio.BlazorApp.Services.Pricing;

/// <summary>
/// Service for pricing management operations.
/// Makes HTTP API calls to the backend pricing endpoints.
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Gets the current tenant's pricing configuration.
    /// </summary>
    Task<Result<TenantPricingConfigModel>> GetTenantConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates the tenant's pricing configuration.
    /// </summary>
    Task<Result<TenantPricingConfigModel>> UpdateTenantConfigAsync(
        TenantPricingConfigModel model,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of products with their pricing information.
    /// </summary>
    Task<Result<PagedResult<ProductPricingModel>>> GetProductsPricingAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single product's pricing information.
    /// </summary>
    Task<Result<ProductPricingModel>> GetProductPricingAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a product's pricing (list price, discounts, margin prices).
    /// </summary>
    Task<Result<ProductPricingModel>> UpdateProductPricingAsync(
        Guid productId,
        decimal? listPrice,
        bool ivaEnabled,
        List<ProductDiscountModel> discounts,
        List<ProductMarginPriceModel> marginPrices,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Previews pricing calculations without persisting to database.
    /// </summary>
    Task<Result<PricingCalculationResultModel>> CalculatePricesAsync(
        decimal listPrice,
        List<decimal> discounts,
        bool ivaEnabled,
        decimal ivaPercentage = 16.00m,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a bulk pricing update to multiple products.
    /// </summary>
    Task<Result<int>> BulkUpdatePricingAsync(
        BulkUpdateRequestModel model,
        CancellationToken cancellationToken = default);
}
