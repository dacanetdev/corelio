using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.CompleteSale;

/// <summary>
/// Command to finalize a Draft sale: confirm payments and deduct inventory.
/// </summary>
public record CompleteSaleCommand(
    Guid SaleId,
    List<PaymentRequest> Payments) : IRequest<Result<SaleDto>>;
