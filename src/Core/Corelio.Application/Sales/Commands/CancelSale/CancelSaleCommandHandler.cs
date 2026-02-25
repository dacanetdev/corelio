using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.CancelSale;

/// <summary>
/// Handler for CancelSaleCommand. Cancels a sale and restores inventory for completed sales.
/// </summary>
public class CancelSaleCommandHandler(
    ISaleRepository saleRepository,
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelSaleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            return Result<bool>.Failure(
                new Error("Sale.NotFound", $"Sale '{request.SaleId}' not found.", ErrorType.NotFound));
        }

        if (sale.Status == SaleStatus.Cancelled)
        {
            return Result<bool>.Failure(
                new Error("Sale.AlreadyCancelled", $"Sale '{sale.Folio}' is already cancelled.", ErrorType.Conflict));
        }

        // For completed sales, restore inventory for each item
        if (sale.Status == SaleStatus.Completed)
        {
            foreach (var item in sale.Items)
            {
                var inventoryItem = await inventoryRepository.GetByProductAndWarehouseAsync(
                    item.ProductId, sale.WarehouseId, cancellationToken);

                if (inventoryItem is not null)
                {
                    var previousQty = inventoryItem.Quantity;
                    inventoryItem.Quantity += item.Quantity;

                    var transaction = new InventoryTransaction
                    {
                        TenantId = sale.TenantId,
                        InventoryItemId = inventoryItem.Id,
                        Type = InventoryTransactionType.Return,
                        Quantity = item.Quantity,
                        PreviousQuantity = previousQty,
                        NewQuantity = inventoryItem.Quantity,
                        ReferenceId = sale.Id,
                        Notes = $"Sale cancelled: {sale.Folio}"
                    };

                    inventoryRepository.UpdateInventoryItem(inventoryItem);
                    inventoryRepository.AddTransaction(transaction);
                }
            }
        }

        sale.Status = SaleStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            sale.Notes = string.IsNullOrWhiteSpace(sale.Notes)
                ? $"Cancelled: {request.Reason}"
                : $"{sale.Notes} | Cancelled: {request.Reason}";
        }

        saleRepository.Update(sale);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
