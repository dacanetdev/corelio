using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.ConvertQuoteToSale;

/// <summary>
/// Handler for ConvertQuoteToSaleCommand.
/// Validates the quote is open and not expired, then marks it as Cancelled with a conversion note.
/// The UI navigates to the POS with the quote ID to pre-load the cart items.
/// </summary>
public class ConvertQuoteToSaleCommandHandler(
    ISaleRepository saleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ConvertQuoteToSaleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ConvertQuoteToSaleCommand request, CancellationToken cancellationToken)
    {
        var quote = await saleRepository.GetByIdAsync(request.QuoteId, cancellationToken);
        if (quote is null)
        {
            return Result<bool>.Failure(
                new Error("Quote.NotFound", $"Quote '{request.QuoteId}' not found.", ErrorType.NotFound));
        }

        if (quote.Type != SaleType.Quote)
        {
            return Result<bool>.Failure(
                new Error("Quote.InvalidType", $"Sale '{quote.Folio}' is not a quote.", ErrorType.Validation));
        }

        if (quote.Status == SaleStatus.Cancelled)
        {
            return Result<bool>.Failure(
                new Error("Quote.AlreadyCancelled", $"Quote '{quote.Folio}' is already cancelled.", ErrorType.Conflict));
        }

        if (quote.ExpiresAt.HasValue && quote.ExpiresAt.Value < DateTime.UtcNow)
        {
            return Result<bool>.Failure(
                new Error("Quote.Expired", $"Quote '{quote.Folio}' has expired and cannot be converted.", ErrorType.Validation));
        }

        quote.Status = SaleStatus.Cancelled;
        quote.Notes = string.IsNullOrWhiteSpace(quote.Notes)
            ? "Convertida a venta"
            : $"{quote.Notes} | Convertida a venta";

        saleRepository.Update(quote);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
