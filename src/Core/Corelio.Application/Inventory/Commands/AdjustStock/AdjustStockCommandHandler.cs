using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Commands.AdjustStock;

/// <summary>
/// Handler for AdjustStockCommand. Applies a manual stock adjustment and records the transaction.
/// </summary>
public class AdjustStockCommandHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AdjustStockCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var item = await inventoryRepository.GetByIdAsync(request.InventoryItemId, cancellationToken);
        if (item is null)
        {
            return Result<bool>.Failure(
                new Error("Inventory.NotFound", $"Inventory item '{request.InventoryItemId}' not found.", ErrorType.NotFound));
        }

        var previousQty = item.Quantity;

        if (request.IsIncrease)
        {
            item.Quantity += request.Quantity;
        }
        else
        {
            if (request.Quantity > item.AvailableQuantity)
            {
                return Result<bool>.Failure(
                    new Error("Inventory.InsufficientStock", "Insufficient stock for this adjustment.", ErrorType.Validation));
            }

            item.Quantity -= request.Quantity;
        }

        var txType = DetermineTransactionType(request.ReasonCode, request.IsIncrease);

        var tx = new InventoryTransaction
        {
            TenantId = item.TenantId,
            InventoryItemId = item.Id,
            Type = txType,
            Quantity = request.IsIncrease ? request.Quantity : -request.Quantity,
            PreviousQuantity = previousQty,
            NewQuantity = item.Quantity,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? request.ReasonCode : request.Notes
        };

        inventoryRepository.UpdateInventoryItem(item);
        inventoryRepository.AddTransaction(tx);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static InventoryTransactionType DetermineTransactionType(string reasonCode, bool isIncrease)
    {
        return reasonCode switch
        {
            "Found" => InventoryTransactionType.Found,
            "Damaged" => InventoryTransactionType.Damaged,
            "Lost" or "Stolen" => InventoryTransactionType.Lost,
            _ => isIncrease ? InventoryTransactionType.AdjustmentPositive : InventoryTransactionType.AdjustmentNegative
        };
    }
}
