using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Queries.GetProductsPricingList;

/// <summary>
/// Query to get a paged list of products with their pricing information.
/// </summary>
public record GetProductsPricingListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    Guid? CategoryId = null) : IRequest<Result<PagedResult<ProductPricingDto>>>;
