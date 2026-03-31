using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.ConvertQuoteToSale;

/// <summary>
/// Marks a quote as cancelled (converted to sale) so it no longer appears in the open quote list.
/// The actual sale is then created through the normal POS flow with cart pre-loaded from the quote.
/// </summary>
public record ConvertQuoteToSaleCommand(Guid QuoteId) : IRequest<Result<bool>>;
