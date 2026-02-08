using Corelio.Application.Common.Models;
using Corelio.Application.ProductCategories.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Queries.GetCategoryById;

/// <summary>
/// Query to get a product category by its ID.
/// </summary>
/// <param name="Id">The category ID.</param>
public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<ProductCategoryDto>>;
