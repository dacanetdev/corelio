using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.CancelInvoice;

/// <summary>
/// Validates and cancels a Stamped invoice within the 72-hour cancellation window.
/// </summary>
public class CancelInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    ICFDIService cfdiService) : IRequestHandler<CancelInvoiceCommand, Result<bool>>
{
    private static readonly string[] ValidReasons = ["01", "02", "03", "04"];

    public async Task<Result<bool>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (!ValidReasons.Contains(request.Reason))
        {
            return Result<bool>.Failure(
                new Error("Invoice.InvalidCancellationReason",
                    "Cancellation reason must be 01, 02, 03, or 04.",
                    ErrorType.Validation));
        }

        var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            return Result<bool>.Failure(
                new Error("Invoice.NotFound", $"Invoice '{request.InvoiceId}' not found.", ErrorType.NotFound));
        }

        if (invoice.Status != CfdiStatus.Stamped)
        {
            return Result<bool>.Failure(
                new Error("Invoice.NotStamped",
                    "Only Stamped invoices can be cancelled.",
                    ErrorType.Validation));
        }

        // Enforce 72-hour cancellation window
        if (invoice.StampDate.HasValue)
        {
            var hoursSinceStamp = (DateTime.UtcNow - invoice.StampDate.Value).TotalHours;
            if (hoursSinceStamp > 72)
            {
                return Result<bool>.Failure(
                    new Error("Invoice.CancellationWindowExpired",
                        "Invoices can only be cancelled within 72 hours of stamping.",
                        ErrorType.Validation));
            }
        }

        try
        {
            await cfdiService.CancelAsync(request.InvoiceId, request.Reason, cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(
                new Error("Invoice.CancelFailed", ex.Message, ErrorType.Failure));
        }
    }
}
