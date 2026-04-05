using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.GenerateInvoice;

/// <summary>
/// Creates a Draft CFDI invoice from a completed sale.
/// No XML generation happens here — that occurs in StampInvoiceCommand.
/// </summary>
public record GenerateInvoiceCommand(
    Guid SaleId,
    string ReceiverCfdiUse = "G01") : IRequest<Result<Guid>>;
