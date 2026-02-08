using Corelio.Application.Common.Models;
using Corelio.Application.ProductCategories.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;
using Mapster;

namespace Corelio.Application.ProductCategories.Queries.GetCategories;

/// <summary>
/// Handler for the GetCategoriesQuery that returns all product categories.
/// </summary>
public class GetCategoriesQueryHandler(
    IProductCategoryRepository categoryRepository) : IRequestHandler<GetCategoriesQuery, Result<List<ProductCategoryDto>>>
{
    public async Task<Result<List<ProductCategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(
            request.IncludeInactive,
            cancellationToken);

        var categoryDtos = categories.Adapt<List<ProductCategoryDto>>();

        return Result<List<ProductCategoryDto>>.Success(categoryDtos);
    }
}
