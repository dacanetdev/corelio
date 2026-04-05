using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities.CFDI;

/// <summary>
/// Represents a CFDI 4.0 electronic invoice linked to a sale.
/// </summary>
public class Invoice : TenantAuditableEntity
{
    /// <summary>
    /// Optional link to the originating sale.
    /// </summary>
    public Guid? SaleId { get; set; }
    public Sale? Sale { get; set; }

    /// <summary>
    /// Invoice folio number (e.g., "F-00001").
    /// </summary>
    public string Folio { get; set; } = string.Empty;

    /// <summary>
    /// Invoice serie (e.g., "A").
    /// </summary>
    public string Serie { get; set; } = "A";

    /// <summary>
    /// SAT UUID (Folio Fiscal) assigned after stamping. Null until stamped.
    /// </summary>
    public string? Uuid { get; set; }

    /// <summary>
    /// Current invoice status.
    /// </summary>
    public CfdiStatus Status { get; set; } = CfdiStatus.Draft;

    /// <summary>
    /// CFDI invoice type.
    /// </summary>
    public CfdiType InvoiceType { get; set; } = CfdiType.Ingreso;

    // Issuer snapshot (captured from TenantConfiguration at generation time)
    public string IssuerRfc { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
    public string IssuerTaxRegime { get; set; } = string.Empty;

    // Receiver snapshot (captured from Customer at generation time)
    public string ReceiverRfc { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverTaxRegime { get; set; } = string.Empty;
    public string ReceiverPostalCode { get; set; } = string.Empty;

    /// <summary>
    /// SAT CFDI use code (e.g., "G01" = Acquisition of goods).
    /// </summary>
    public string ReceiverCfdiUse { get; set; } = string.Empty;

    // Amounts
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }

    /// <summary>
    /// SAT payment form code (e.g., "01" = Cash, "03" = Transfer, "04" = Card).
    /// </summary>
    public string PaymentForm { get; set; } = string.Empty;

    /// <summary>
    /// SAT payment method code. "PUE" = single payment, "PPD" = installments.
    /// </summary>
    public string PaymentMethod { get; set; } = "PUE";

    // Stamp data (populated after PAC stamping)
    public DateTime? StampDate { get; set; }
    public string? SatCertificateNumber { get; set; }
    public string? PacStampSignature { get; set; }
    public string? SatStampSignature { get; set; }
    public string? QrCodeData { get; set; }
    public string? OriginalChain { get; set; }

    /// <summary>
    /// Full signed XML content stored in DB for download and audit.
    /// </summary>
    public string? XmlContent { get; set; }

    // Cancellation data
    public string? CancellationReason { get; set; }
    public DateTime? CancellationDate { get; set; }

    public ICollection<InvoiceItem> Items { get; set; } = [];
}
