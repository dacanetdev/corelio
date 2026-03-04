namespace Corelio.Application.Inventory.Common;

/// <summary>
/// Data transfer object for an inventory transaction.
/// </summary>
public record InventoryTransactionDto(
    Guid Id,
    string Type,
    decimal Quantity,
    decimal PreviousQuantity,
    decimal NewQuantity,
    string? Notes,
    DateTime CreatedAt,
    string? CreatedBy);
