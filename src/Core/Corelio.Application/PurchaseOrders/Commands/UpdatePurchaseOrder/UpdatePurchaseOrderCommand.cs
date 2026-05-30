using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;

public record UpdatePurchaseOrderCommand(
    Guid Id,
    Guid SupplierId,
    DateTimeOffset? ExpectedDate,
    string? Notes,
    List<PurchaseOrderItemRequest> Items) : IRequest<Result<bool>>;
