using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Queries.GetSales;

/// <summary>
/// Query to get a paged list of sales.
/// </summary>
public record GetSalesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    SaleStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<Result<PagedResult<SaleListDto>>>;
