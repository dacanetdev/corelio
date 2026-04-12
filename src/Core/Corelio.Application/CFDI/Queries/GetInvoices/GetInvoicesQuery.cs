using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Queries.GetInvoices;

/// <summary>
/// Query to get a paged list of invoices with optional filters.
/// </summary>
public record GetInvoicesQuery(
    int PageNumber = 1,
    int PageSize = 20,
    CfdiStatus? Status = null,
    string? SearchTerm = null) : IRequest<Result<PagedResult<InvoiceListDto>>>;
