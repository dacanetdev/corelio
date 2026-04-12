namespace Corelio.BlazorApp.Models.Cfdi;

/// <summary>
/// Summary invoice model for list views.
/// </summary>
public class InvoiceListModel
{
    public Guid Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public string? Uuid { get; set; }
    public string ReceiverRfc { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? StampDate { get; set; }
}

/// <summary>
/// Full invoice detail model.
/// </summary>
public class InvoiceModel
{
    public Guid Id { get; set; }
    public Guid? SaleId { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public string? Uuid { get; set; }
    public string Status { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    // Issuer
    public string IssuerRfc { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
    public string IssuerTaxRegime { get; set; } = string.Empty;
    // Receiver
    public string ReceiverRfc { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverTaxRegime { get; set; } = string.Empty;
    public string ReceiverPostalCode { get; set; } = string.Empty;
    public string ReceiverCfdiUse { get; set; } = string.Empty;
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string PaymentForm { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    // Stamp
    public DateTime? StampDate { get; set; }
    public string? SatCertificateNumber { get; set; }
    public string? QrCodeData { get; set; }
    // Cancellation
    public string? CancellationReason { get; set; }
    public DateTime? CancellationDate { get; set; }
    // Dates
    public DateTime CreatedAt { get; set; }
    // Items
    public List<InvoiceItemModel> Items { get; set; } = [];
}

/// <summary>
/// Invoice line item model.
/// </summary>
public class InvoiceItemModel
{
    public Guid Id { get; set; }
    public int ItemNumber { get; set; }
    public string ProductKey { get; set; } = string.Empty;
    public string UnitKey { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitValue { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
}

/// <summary>
/// CFDI issuer settings model.
/// </summary>
public class CfdiSettingsModel
{
    public string? IssuerRfc { get; set; }
    public string? IssuerName { get; set; }
    public string? IssuerTaxRegime { get; set; }
    public string? IssuerPostalCode { get; set; }
    public string CfdiSeries { get; set; } = "A";
    public int CfdiNextFolio { get; set; }
    public DateTime? CfdiCertificateExpiresAt { get; set; }
    public bool HasCertificate { get; set; }
}

/// <summary>
/// Request to generate a new CFDI invoice from a sale.
/// </summary>
public class GenerateInvoiceRequest
{
    public Guid SaleId { get; set; }
    public string ReceiverCfdiUse { get; set; } = "G03";
}

/// <summary>
/// Request to update CFDI issuer settings.
/// </summary>
public class UpdateCfdiSettingsRequest
{
    public string IssuerRfc { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
    public string IssuerTaxRegime { get; set; } = string.Empty;
    public string IssuerPostalCode { get; set; } = string.Empty;
    public string CfdiSeries { get; set; } = "A";
}
