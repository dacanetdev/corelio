using Corelio.Domain.Enums;

namespace Corelio.Application.Sales.Common;

/// <summary>
/// Full sale data transfer object.
/// </summary>
public record SaleDto(
    Guid Id,
    string Folio,
    SaleType Type,
    SaleStatus Status,
    Guid? CustomerId,
    string? CustomerName,
    Guid WarehouseId,
    string WarehouseName,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal Total,
    string? Notes,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    List<SaleItemDto> Items,
    List<PaymentDto> Payments);

/// <summary>
/// Sale item data transfer object.
/// </summary>
public record SaleItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSku,
    decimal UnitPrice,
    decimal Quantity,
    decimal DiscountPercentage,
    decimal TaxPercentage,
    decimal LineTotal);

/// <summary>
/// Payment data transfer object.
/// </summary>
public record PaymentDto(
    Guid Id,
    PaymentMethod Method,
    decimal Amount,
    string? Reference,
    PaymentStatus Status);
