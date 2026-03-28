using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Queries.GenerateSaleReceipt;

/// <summary>
/// Handler for GenerateSaleReceiptQuery. Fetches the sale and generates a PDF receipt.
/// </summary>
public class GenerateSaleReceiptQueryHandler(
    ISaleRepository saleRepository,
    ISaleReceiptService receiptService) : IRequestHandler<GenerateSaleReceiptQuery, Result<SaleReceiptResult>>
{
    public async Task<Result<SaleReceiptResult>> Handle(
        GenerateSaleReceiptQuery request, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            return Result<SaleReceiptResult>.Failure(
                new Error("Sale.NotFound", $"Sale '{request.SaleId}' not found.", ErrorType.NotFound));
        }

        var pdfBytes = await receiptService.GenerateAsync(sale, cancellationToken);
        return Result<SaleReceiptResult>.Success(new SaleReceiptResult(pdfBytes, sale.Folio));
    }
}
