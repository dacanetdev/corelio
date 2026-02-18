using Corelio.Domain.Enums;

namespace Corelio.Application.Customers.Common;

/// <summary>
/// Full customer data transfer object.
/// </summary>
public record CustomerDto(
    Guid Id,
    CustomerType CustomerType,
    string FirstName,
    string LastName,
    string? BusinessName,
    string FullName,
    string? Rfc,
    string? Curp,
    string? Email,
    string? Phone,
    string? TaxRegime,
    string? CfdiUse,
    PaymentMethod? PreferredPaymentMethod,
    DateTime CreatedAt,
    DateTime UpdatedAt);
