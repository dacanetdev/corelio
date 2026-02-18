using Corelio.Domain.Enums;

namespace Corelio.Application.Sales.Common;

/// <summary>
/// Request model for a payment when completing a sale.
/// </summary>
public record PaymentRequest(
    PaymentMethod Method,
    decimal Amount,
    string? Reference = null);
