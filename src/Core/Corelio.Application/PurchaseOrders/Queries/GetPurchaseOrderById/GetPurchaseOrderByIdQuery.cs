using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrderById;

public record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<Result<PurchaseOrderDto>>;
