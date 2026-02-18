using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a customer in the system for invoicing and CRM.
/// </summary>
public class Customer : TenantAuditableEntity, ISoftDeletable
{
    /// <summary>
    /// Customer type: Individual (persona física) or Business (persona moral).
    /// </summary>
    public CustomerType CustomerType { get; set; } = CustomerType.Individual;

    /// <summary>
    /// First name (for Individual customers).
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name (for Individual customers).
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Business/company name (for Business customers).
    /// </summary>
    public string? BusinessName { get; set; }

    /// <summary>
    /// Mexican RFC (Registro Federal de Contribuyentes). Optional for retail.
    /// </summary>
    public string? Rfc { get; set; }

    /// <summary>
    /// Mexican CURP (for individual customers).
    /// </summary>
    public string? Curp { get; set; }

    /// <summary>
    /// Contact email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Contact phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// SAT tax regime code (required for CFDI).
    /// </summary>
    public string? TaxRegime { get; set; }

    /// <summary>
    /// SAT CFDI use code (G01, G03, etc.). Required for invoice generation.
    /// </summary>
    public string? CfdiUse { get; set; }

    /// <summary>
    /// Preferred payment method for this customer.
    /// </summary>
    public PaymentMethod? PreferredPaymentMethod { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Computed display name based on customer type.
    /// </summary>
    public string FullName => CustomerType == CustomerType.Business
        ? BusinessName ?? string.Empty
        : $"{FirstName} {LastName}".Trim();
}
