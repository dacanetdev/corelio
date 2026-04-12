using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Queries.GetInvoices;

/// <summary>
/// Handler for GetInvoicesQuery.
/// </summary>
public class GetInvoicesQueryHandler(
    IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoicesQuery, Result<PagedResult<InvoiceListDto>>>
{
    public async Task<Result<PagedResult<InvoiceListDto>>> Handle(
        GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await invoiceRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Status,
            request.SearchTerm,
            cancellationToken);

        var dtos = items.Select(i => new InvoiceListDto(
            i.Id,
            i.Folio,
            i.Serie,
            i.Uuid,
            i.ReceiverRfc,
            i.ReceiverName,
            i.Total,
            i.Status,
            i.CreatedAt,
            i.StampDate)).ToList();

        var pagedResult = PagedResult<InvoiceListDto>.Create(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result<PagedResult<InvoiceListDto>>.Success(pagedResult);
    }
}
