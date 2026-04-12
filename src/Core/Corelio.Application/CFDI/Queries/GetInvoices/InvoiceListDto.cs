using Corelio.Domain.Enums;

namespace Corelio.Application.CFDI.Queries.GetInvoices;

/// <summary>
/// Summary invoice DTO for list views.
/// </summary>
public record InvoiceListDto(
    Guid Id,
    string Folio,
    string Serie,
    string? Uuid,
    string ReceiverRfc,
    string ReceiverName,
    decimal Total,
    CfdiStatus Status,
    DateTime CreatedAt,
    DateTime? StampDate);
