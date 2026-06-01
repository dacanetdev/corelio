using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.GoodsReceipts.Commands.ReceiveGoods;

public class ReceiveGoodsCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IGoodsReceiptRepository goodsReceiptRepository,
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<ReceiveGoodsCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ReceiveGoodsCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId, cancellationToken);
        if (purchaseOrder is null)
        {
            return Result<Guid>.Failure(new Error("PurchaseOrder.NotFound", $"Purchase order with ID '{request.PurchaseOrderId}' not found.", ErrorType.NotFound));
        }

        if (purchaseOrder.Status is not (PurchaseOrderStatus.Approved or PurchaseOrderStatus.PartiallyReceived))
        {
            return Result<Guid>.Failure(new Error(
                "PurchaseOrder.InvalidTransition",
                $"Cannot receive goods against a purchase order in '{purchaseOrder.Status}' status. Only Approved or PartiallyReceived orders can accept goods.",
                ErrorType.Conflict));
        }

        var warehouses = await inventoryRepository.GetAllWarehousesAsync(cancellationToken);
        if (!warehouses.Any(w => w.Id == request.WarehouseId))
        {
            return Result<Guid>.Failure(new Error("Warehouse.NotFound", $"Warehouse with ID '{request.WarehouseId}' not found.", ErrorType.NotFound));
        }

        var receiptItems = new List<GoodsReceiptItem>();

        foreach (var itemRequest in request.Items)
        {
            var poItem = purchaseOrder.Items.FirstOrDefault(i => i.Id == itemRequest.PurchaseOrderItemId);
            if (poItem is null)
            {
                return Result<Guid>.Failure(new Error(
                    "PurchaseOrderItem.NotFound",
                    $"Purchase order item with ID '{itemRequest.PurchaseOrderItemId}' not found in this order.",
                    ErrorType.NotFound));
            }

            var remainingQty = poItem.Quantity - poItem.ReceivedQuantity;
            if (itemRequest.QuantityReceived > remainingQty)
            {
                return Result<Guid>.Failure(new Error(
                    "GoodsReceipt.OverReceipt",
                    $"Cannot receive {itemRequest.QuantityReceived} units of '{poItem.ProductName}'. Only {remainingQty} units remain to be received.",
                    ErrorType.Conflict));
            }

            var inventoryItem = await inventoryRepository.GetByProductAndWarehouseAsync(
                poItem.ProductId, request.WarehouseId, cancellationToken);

            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    TenantId = tenantId.Value,
                    ProductId = poItem.ProductId,
                    WarehouseId = request.WarehouseId,
                    Quantity = itemRequest.QuantityReceived,
                    ReservedQuantity = 0,
                    MinimumLevel = 0
                };
                inventoryRepository.AddInventoryItem(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += itemRequest.QuantityReceived;
                inventoryRepository.UpdateInventoryItem(inventoryItem);
            }

            poItem.ReceivedQuantity += itemRequest.QuantityReceived;

            receiptItems.Add(new GoodsReceiptItem
            {
                GoodsReceiptId = Guid.Empty, // set via navigation after receipt is created
                PurchaseOrderItemId = poItem.Id,
                ProductId = poItem.ProductId,
                ProductName = poItem.ProductName,
                QuantityReceived = itemRequest.QuantityReceived
            });
        }

        var receipt = new GoodsReceipt
        {
            TenantId = tenantId.Value,
            PurchaseOrderId = purchaseOrder.Id,
            WarehouseId = request.WarehouseId,
            ReceivedDate = request.ReceivedDate,
            Notes = request.Notes,
            Items = receiptItems
        };

        var allReceived = purchaseOrder.Items.All(i => i.ReceivedQuantity >= i.Quantity);
        purchaseOrder.Status = allReceived ? PurchaseOrderStatus.Received : PurchaseOrderStatus.PartiallyReceived;

        goodsReceiptRepository.Add(receipt);
        purchaseOrderRepository.Update(purchaseOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(receipt.Id);
    }
}
