using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Queries.GetSales;

/// <summary>
/// Handler for GetSalesQuery.
/// </summary>
public class GetSalesQueryHandler(
    ISaleRepository saleRepository) : IRequestHandler<GetSalesQuery, Result<PagedResult<SaleListDto>>>
{
    public async Task<Result<PagedResult<SaleListDto>>> Handle(
        GetSalesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await saleRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Status,
            request.DateFrom,
            request.DateTo,
            cancellationToken);

        var dtos = items.Select(s => new SaleListDto(
            s.Id,
            s.Folio,
            s.Type,
            s.Status,
            s.Customer?.FullName,
            s.Total,
            s.Items.Count,
            s.CreatedAt,
            s.CompletedAt)).ToList();

        var pagedResult = PagedResult<SaleListDto>.Create(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result<PagedResult<SaleListDto>>.Success(pagedResult);
    }
}
