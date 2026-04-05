using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.CancelInvoice;

/// <summary>
/// Cancels a Stamped invoice via PAC.
/// Reason codes: "01" = Comprobante con errores, "02" = Comprobante no requerido,
/// "03" = No se llevó a cabo la operación, "04" = Operación nominativa relacionada en una factura global.
/// </summary>
public record CancelInvoiceCommand(Guid InvoiceId, string Reason) : IRequest<Result<bool>>;
