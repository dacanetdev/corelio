using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.CancelPurchaseOrder;

public record CancelPurchaseOrderCommand(Guid Id) : IRequest<Result<bool>>;
