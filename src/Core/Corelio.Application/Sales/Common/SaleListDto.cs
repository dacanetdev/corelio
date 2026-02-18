using Corelio.Domain.Enums;

namespace Corelio.Application.Sales.Common;

/// <summary>
/// Summary sale DTO for list/management views.
/// </summary>
public record SaleListDto(
    Guid Id,
    string Folio,
    SaleType Type,
    SaleStatus Status,
    string? CustomerName,
    decimal Total,
    int ItemCount,
    DateTime CreatedAt,
    DateTime? CompletedAt);
