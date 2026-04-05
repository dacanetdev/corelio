using Corelio.Domain.Common;

namespace Corelio.Domain.Entities.CFDI;

/// <summary>
/// Represents a line item (Concepto) in a CFDI invoice.
/// </summary>
public class InvoiceItem : TenantAuditableEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    /// <summary>
    /// Line number within the invoice (1-based).
    /// </summary>
    public int ItemNumber { get; set; }

    /// <summary>
    /// Optional product reference for traceability.
    /// </summary>
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }

    /// <summary>
    /// SAT product/service key (ClaveProdServ). e.g., "25171500" for hardware.
    /// </summary>
    public string ProductKey { get; set; } = "25171500";

    /// <summary>
    /// SAT unit key (ClaveUnidad). e.g., "H87" for piece.
    /// </summary>
    public string UnitKey { get; set; } = "H87";

    /// <summary>
    /// Description of the item (Descripcion in CFDI XML).
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal UnitValue { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }

    /// <summary>
    /// SAT tax object code. "02" = taxed, "01" = exempt.
    /// </summary>
    public string TaxObject { get; set; } = "02";

    /// <summary>
    /// IVA rate (e.g., 0.16 for 16%).
    /// </summary>
    public decimal TaxRate { get; set; } = 0.16m;

    /// <summary>
    /// Calculated IVA amount for this line.
    /// </summary>
    public decimal TaxAmount { get; set; }
}
