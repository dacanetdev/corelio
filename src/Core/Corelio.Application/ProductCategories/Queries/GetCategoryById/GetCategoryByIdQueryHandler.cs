using Corelio.Application.Common.Models;
using Corelio.Application.ProductCategories.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;
using Mapster;

namespace Corelio.Application.ProductCategories.Queries.GetCategoryById;

/// <summary>
/// Handler for the GetCategoryByIdQuery that returns a single product category.
/// </summary>
public class GetCategoryByIdQueryHandler(
    IProductCategoryRepository categoryRepository) : IRequestHandler<GetCategoryByIdQuery, Result<ProductCategoryDto>>
{
    public async Task<Result<ProductCategoryDto>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
        {
            return Result<ProductCategoryDto>.Failure(
                new Error("Category.NotFound", $"Category with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        var categoryDto = category.Adapt<ProductCategoryDto>();

        return Result<ProductCategoryDto>.Success(categoryDto);
    }
}
