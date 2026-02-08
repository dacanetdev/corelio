using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Queries.GetProductsPricingList;

/// <summary>
/// Handler for retrieving a paged list of products with their pricing information.
/// </summary>
public class GetProductsPricingListQueryHandler(
    IProductPricingRepository productPricingRepository,
    ITenantPricingConfigurationRepository configRepository,
    ITenantService tenantService) : IRequestHandler<GetProductsPricingListQuery, Result<PagedResult<ProductPricingDto>>>
{
    public async Task<Result<PagedResult<ProductPricingDto>>> Handle(
        GetProductsPricingListQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<PagedResult<ProductPricingDto>>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var config = await configRepository.GetWithTierDefinitionsAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            return Result<PagedResult<ProductPricingDto>>.Failure(
                new Error("PricingConfig.NotFound", "Pricing configuration not found for this tenant.", ErrorType.NotFound));
        }

        var (products, totalCount) = await productPricingRepository.GetProductsPricingListAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.CategoryId,
            cancellationToken);

        var productDtos = PricingMapper.ToProductPricingDtoList(products, config);

        var pagedResult = PagedResult<ProductPricingDto>.Create(
            productDtos,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result<PagedResult<ProductPricingDto>>.Success(pagedResult);
    }
}
