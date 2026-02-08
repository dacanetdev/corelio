using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Queries.GetProductPricing;

/// <summary>
/// Handler for retrieving a single product's pricing information.
/// </summary>
public class GetProductPricingQueryHandler(
    IProductPricingRepository productPricingRepository,
    ITenantPricingConfigurationRepository configRepository,
    ITenantService tenantService) : IRequestHandler<GetProductPricingQuery, Result<ProductPricingDto>>
{
    public async Task<Result<ProductPricingDto>> Handle(
        GetProductPricingQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<ProductPricingDto>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var product = await productPricingRepository.GetProductPricingAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result<ProductPricingDto>.Failure(
                new Error("Product.NotFound", $"Product with ID '{request.ProductId}' not found.", ErrorType.NotFound));
        }

        var config = await configRepository.GetWithTierDefinitionsAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            return Result<ProductPricingDto>.Failure(
                new Error("PricingConfig.NotFound", "Pricing configuration not found for this tenant.", ErrorType.NotFound));
        }

        return Result<ProductPricingDto>.Success(PricingMapper.ToProductPricingDto(product, config));
    }
}
