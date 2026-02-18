using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Models.Customers;

/// <summary>
/// Blazor model for customer data.
/// </summary>
public class CustomerModel
{
    public Guid Id { get; set; }
    public CustomerType CustomerType { get; set; } = CustomerType.Individual;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string? Curp { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? TaxRegime { get; set; }
    public string? CfdiUse { get; set; }
    public PaymentMethod? PreferredPaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Blazor model for customer list items.
/// </summary>
public class CustomerListModel
{
    public Guid Id { get; set; }
    public CustomerType CustomerType { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}
