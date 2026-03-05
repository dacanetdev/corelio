using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Queries.GetInventoryTransactions;

/// <summary>
/// Query to get a paged list of transactions for a specific inventory item.
/// </summary>
public record GetInventoryTransactionsQuery(
    Guid InventoryItemId,
    int PageNumber = 1,
    int PageSize = 50) : IRequest<Result<PagedResult<InventoryTransactionDto>>>;
