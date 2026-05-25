using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a supplier (vendor) from whom the tenant purchases products.
/// </summary>
public class Supplier : TenantAuditableEntity, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public string? TaxRegime { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

}
