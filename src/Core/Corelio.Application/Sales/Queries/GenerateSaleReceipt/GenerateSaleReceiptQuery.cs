using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Queries.GenerateSaleReceipt;

/// <summary>
/// Query to generate a PDF receipt for a completed sale.
/// Returns the raw PDF bytes along with the sale folio for use in the filename.
/// </summary>
public record GenerateSaleReceiptQuery(Guid SaleId) : IRequest<Result<SaleReceiptResult>>;

/// <summary>
/// Result of the GenerateSaleReceiptQuery, containing the PDF bytes and sale folio.
/// </summary>
public record SaleReceiptResult(byte[] PdfBytes, string Folio);
