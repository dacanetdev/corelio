using Corelio.Domain.Enums;

namespace Corelio.Application.CFDI.Queries.GetInvoiceById;

/// <summary>
/// Full invoice detail DTO.
/// </summary>
public record InvoiceDto(
    Guid Id,
    Guid? SaleId,
    string Folio,
    string Serie,
    string? Uuid,
    CfdiStatus Status,
    CfdiType InvoiceType,
    // Issuer
    string IssuerRfc,
    string IssuerName,
    string IssuerTaxRegime,
    // Receiver
    string ReceiverRfc,
    string ReceiverName,
    string ReceiverTaxRegime,
    string ReceiverPostalCode,
    string ReceiverCfdiUse,
    // Amounts
    decimal Subtotal,
    decimal Discount,
    decimal Total,
    string PaymentForm,
    string PaymentMethod,
    // Stamp data
    DateTime? StampDate,
    string? SatCertificateNumber,
    string? QrCodeData,
    // Cancellation
    string? CancellationReason,
    DateTime? CancellationDate,
    // Dates
    DateTime CreatedAt,
    // Items
    List<InvoiceItemDto> Items);
