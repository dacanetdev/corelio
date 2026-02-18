using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.CancelSale;

/// <summary>
/// Command to cancel a Draft or Pending sale.
/// </summary>
public record CancelSaleCommand(Guid SaleId, string? Reason = null) : IRequest<Result<bool>>;
