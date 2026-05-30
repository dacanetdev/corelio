using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.CreatePurchaseOrder;

/// <summary>A single line item in the create purchase order request.</summary>
public record PurchaseOrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice);

public record CreatePurchaseOrderCommand(
    Guid SupplierId,
    DateTimeOffset? ExpectedDate,
    string? Notes,
    List<PurchaseOrderItemRequest> Items) : IRequest<Result<Guid>>;
