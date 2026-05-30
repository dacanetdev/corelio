using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Common;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrders;

public record GetPurchaseOrdersQuery(
    int Page,
    int Size,
    PurchaseOrderStatus? Status,
    Guid? SupplierId,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate) : IRequest<Result<PagedResult<PurchaseOrderListDto>>>;
