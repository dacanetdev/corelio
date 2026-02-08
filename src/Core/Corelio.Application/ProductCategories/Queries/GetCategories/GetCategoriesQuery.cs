using Corelio.Application.Common.Models;
using Corelio.Application.ProductCategories.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Queries.GetCategories;

/// <summary>
/// Query to get all product categories.
/// </summary>
/// <param name="IncludeInactive">Whether to include inactive categories.</param>
public record GetCategoriesQuery(bool IncludeInactive = false) : IRequest<Result<List<ProductCategoryDto>>>;
