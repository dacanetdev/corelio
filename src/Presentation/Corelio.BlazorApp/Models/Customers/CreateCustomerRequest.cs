using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Models.Customers;

/// <summary>
/// Request model for creating/updating a customer from Blazor.
/// </summary>
public class CustomerFormModel
{
    public CustomerType CustomerType { get; set; } = CustomerType.Individual;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? Rfc { get; set; }
    public string? Curp { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? TaxRegime { get; set; }
    public string? CfdiUse { get; set; }
    public PaymentMethod? PreferredPaymentMethod { get; set; }
}
