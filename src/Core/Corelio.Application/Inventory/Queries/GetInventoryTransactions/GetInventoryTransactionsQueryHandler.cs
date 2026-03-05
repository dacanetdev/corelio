using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Queries.GetInventoryTransactions;

/// <summary>
/// Handler for GetInventoryTransactionsQuery.
/// </summary>
public class GetInventoryTransactionsQueryHandler(
    IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryTransactionsQuery, Result<PagedResult<InventoryTransactionDto>>>
{
    public async Task<Result<PagedResult<InventoryTransactionDto>>> Handle(
        GetInventoryTransactionsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await inventoryRepository.GetTransactionsPagedAsync(
            request.InventoryItemId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(tx => new InventoryTransactionDto(
            tx.Id,
            tx.Type.ToString(),
            tx.Quantity,
            tx.PreviousQuantity,
            tx.NewQuantity,
            tx.Notes,
            tx.CreatedAt,
            tx.CreatedBy)).ToList();

        var pagedResult = PagedResult<InventoryTransactionDto>.Create(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result<PagedResult<InventoryTransactionDto>>.Success(pagedResult);
    }
}
