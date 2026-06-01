namespace Corelio.BlazorApp.Models.Suppliers;

/// <summary>
/// Supplier list item model.
/// </summary>
public class SupplierListModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Full supplier detail model.
/// </summary>
public class SupplierModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public int PaymentTermsDays { get; set; }
    public string? TaxRegime { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Form model for creating/editing a supplier.
/// </summary>
public class SupplierFormModel
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
}
