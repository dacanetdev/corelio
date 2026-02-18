using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a payment received for a sale (supports mixed payments).
/// </summary>
public class Payment : TenantAuditableEntity
{
    /// <summary>
    /// Sale this payment belongs to.
    /// </summary>
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    /// <summary>
    /// Payment method used.
    /// </summary>
    public PaymentMethod Method { get; set; }

    /// <summary>
    /// Amount paid with this method.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Reference code (card authorization number, bank transfer reference, etc.).
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// Payment status.
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;
}
