namespace Corelio.Application.Sales.Common;

/// <summary>
/// Request model for a single line item when creating a sale.
/// </summary>
public record CartItemRequest(
    Guid ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage = 0);
