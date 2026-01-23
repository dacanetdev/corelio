using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Queries.GetProducts;

/// <summary>
/// Query to get a paged list of products with optional filtering.
/// </summary>
public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    Guid? CategoryId = null,
    bool? IsActive = null) : IRequest<Result<PagedResult<ProductListDto>>>;
