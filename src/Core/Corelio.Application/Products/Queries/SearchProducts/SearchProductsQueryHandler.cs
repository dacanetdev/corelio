using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;
using MapsterMapper;

namespace Corelio.Application.Products.Queries.SearchProducts;

/// <summary>
/// Handler for the SearchProductsQuery that searches products by barcode, SKU, or name.
/// </summary>
public class SearchProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<SearchProductsQuery, Result<List<ProductListDto>>>
{
    public async Task<Result<List<ProductListDto>>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await productRepository.SearchAsync(
            request.Query,
            request.Limit,
            cancellationToken);

        var productDtos = mapper.Map<List<ProductListDto>>(products);

        return Result<List<ProductListDto>>.Success(productDtos);
    }
}
