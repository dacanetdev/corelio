using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;
using MapsterMapper;

namespace Corelio.Application.Products.Queries.GetProductById;

/// <summary>
/// Handler for the GetProductByIdQuery that retrieves a product by its ID.
/// </summary>
public class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null || product.IsDeleted)
        {
            return Result<ProductDto>.Failure(
                new Error("Product.NotFound", $"Product with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        var productDto = mapper.Map<ProductDto>(product);

        return Result<ProductDto>.Success(productDto);
    }
}
