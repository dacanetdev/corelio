using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.StampInvoice;

/// <summary>
/// Delegates to ICFDIService for the full stamp workflow (LoadCert → XML → Sign → PAC → Persist).
/// </summary>
public class StampInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    ICFDIService cfdiService) : IRequestHandler<StampInvoiceCommand, Result<string>>
{
    public async Task<Result<string>> Handle(StampInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            return Result<string>.Failure(
                new Error("Invoice.NotFound", $"Invoice '{request.InvoiceId}' not found.", ErrorType.NotFound));
        }

        if (invoice.Status != CfdiStatus.Draft)
        {
            return Result<string>.Failure(
                new Error("Invoice.NotDraft",
                    $"Only Draft invoices can be stamped. Current status: {invoice.Status}.",
                    ErrorType.Validation));
        }

        try
        {
            var uuid = await cfdiService.StampAsync(request.InvoiceId, cancellationToken);
            return Result<string>.Success(uuid);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure(
                new Error("Invoice.StampFailed", ex.Message, ErrorType.Failure));
        }
    }
}
