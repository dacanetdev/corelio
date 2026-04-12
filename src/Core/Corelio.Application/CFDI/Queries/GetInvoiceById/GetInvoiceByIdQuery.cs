using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Queries.GetInvoiceById;

/// <summary>
/// Query to get a single invoice by ID.
/// </summary>
public record GetInvoiceByIdQuery(Guid Id) : IRequest<Result<InvoiceDto>>;
