using Corelio.Application.Common.Models;
using Corelio.Application.GoodsReceipts.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.GoodsReceipts.Queries.GetGoodsReceipts;

public record GetGoodsReceiptsQuery(
    int Page,
    int Size,
    Guid? PurchaseOrderId = null,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null) : IRequest<Result<PagedResult<GoodsReceiptListDto>>>;
