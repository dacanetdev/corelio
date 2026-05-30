using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.SubmitPurchaseOrder;

public record SubmitPurchaseOrderCommand(Guid Id) : IRequest<Result<bool>>;
