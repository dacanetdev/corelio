using Corelio.Application.Common.Models;
using Corelio.Application.GoodsReceipts.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.GoodsReceipts.Queries.GetGoodsReceiptById;

public record GetGoodsReceiptByIdQuery(Guid Id) : IRequest<Result<GoodsReceiptDto>>;
