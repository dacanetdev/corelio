using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Queries.GetSales;

/// <summary>
/// Query to get a paged list of sales with optional filters.
/// </summary>
public record GetSalesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    SaleStatus? Status = null,
    string? SearchTerm = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    SaleType? Type = null) : IRequest<Result<PagedResult<SaleListDto>>>;
