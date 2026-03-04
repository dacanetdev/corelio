using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Commands.AdjustStock;

/// <summary>
/// Command to manually adjust stock for an inventory item.
/// </summary>
public record AdjustStockCommand(
    Guid InventoryItemId,
    decimal Quantity,           // always positive
    bool IsIncrease,
    string ReasonCode,          // Damaged|Lost|Stolen|Found|CountCorrection|Other
    string? Notes) : IRequest<Result<bool>>;
