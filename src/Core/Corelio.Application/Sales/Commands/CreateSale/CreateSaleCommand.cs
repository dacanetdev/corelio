using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.CreateSale;

/// <summary>
/// Command to create a new Draft sale.
/// Inventory is NOT deducted until CompleteSaleCommand is issued.
/// </summary>
public record CreateSaleCommand(
    List<CartItemRequest> Items,
    Guid? CustomerId = null,
    Guid? WarehouseId = null,
    SaleType Type = SaleType.Pos,
    string? Notes = null) : IRequest<Result<Guid>>;
