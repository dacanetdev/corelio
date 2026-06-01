using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.GoodsReceipts.Commands.ReceiveGoods;

public record ReceiveGoodsCommand(
    Guid PurchaseOrderId,
    Guid WarehouseId,
    DateOnly ReceivedDate,
    string? Notes,
    IReadOnlyList<ReceiveGoodsItemRequest> Items) : IRequest<Result<Guid>>;

public record ReceiveGoodsItemRequest(
    Guid PurchaseOrderItemId,
    decimal QuantityReceived);
