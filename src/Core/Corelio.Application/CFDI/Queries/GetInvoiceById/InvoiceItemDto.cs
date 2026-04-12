namespace Corelio.Application.CFDI.Queries.GetInvoiceById;

/// <summary>
/// Invoice line item DTO.
/// </summary>
public record InvoiceItemDto(
    Guid Id,
    int ItemNumber,
    string ProductKey,
    string UnitKey,
    string Description,
    decimal Quantity,
    decimal UnitValue,
    decimal Amount,
    decimal Discount,
    decimal TaxRate,
    decimal TaxAmount);
