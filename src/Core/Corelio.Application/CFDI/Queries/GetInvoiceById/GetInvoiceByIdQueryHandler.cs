using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Queries.GetInvoiceById;

/// <summary>
/// Handler for GetInvoiceByIdQuery.
/// </summary>
public class GetInvoiceByIdQueryHandler(
    IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
{
    public async Task<Result<InvoiceDto>> Handle(
        GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
        {
            return Result<InvoiceDto>.Failure(
                new Error("Invoice.NotFound", $"Invoice '{request.Id}' not found.", ErrorType.NotFound));
        }

        var items = invoice.Items
            .OrderBy(i => i.ItemNumber)
            .Select(i => new InvoiceItemDto(
                i.Id,
                i.ItemNumber,
                i.ProductKey,
                i.UnitKey,
                i.Description,
                i.Quantity,
                i.UnitValue,
                i.Amount,
                i.Discount,
                i.TaxRate,
                i.TaxAmount))
            .ToList();

        var dto = new InvoiceDto(
            invoice.Id,
            invoice.SaleId,
            invoice.Folio,
            invoice.Serie,
            invoice.Uuid,
            invoice.Status,
            invoice.InvoiceType,
            invoice.IssuerRfc,
            invoice.IssuerName,
            invoice.IssuerTaxRegime,
            invoice.ReceiverRfc,
            invoice.ReceiverName,
            invoice.ReceiverTaxRegime,
            invoice.ReceiverPostalCode,
            invoice.ReceiverCfdiUse,
            invoice.Subtotal,
            invoice.Discount,
            invoice.Total,
            invoice.PaymentForm,
            invoice.PaymentMethod,
            invoice.StampDate,
            invoice.SatCertificateNumber,
            invoice.QrCodeData,
            invoice.CancellationReason,
            invoice.CancellationDate,
            invoice.CreatedAt,
            items);

        return Result<InvoiceDto>.Success(dto);
    }
}
