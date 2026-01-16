using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;
using MapsterMapper;

namespace Corelio.Application.Products.Queries.GetProducts;

/// <summary>
/// Handler for the GetProductsQuery that retrieves a paged list of products.
/// </summary>
public class GetProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductListDto>>>
{
    public async Task<Result<PagedResult<ProductListDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        // Get paged products from repository
        var (products, totalCount) = await productRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.CategoryId,
            request.IsActive,
            cancellationToken);

        // Map to DTOs
        var productDtos = mapper.Map<List<ProductListDto>>(products);

        var pagedResult = PagedResult<ProductListDto>.Create(
            productDtos,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result<PagedResult<ProductListDto>>.Success(pagedResult);
    }
}
