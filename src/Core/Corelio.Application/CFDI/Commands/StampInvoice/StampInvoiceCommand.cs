using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.StampInvoice;

/// <summary>
/// Stamps a Draft invoice via PAC. Returns the SAT UUID on success.
/// </summary>
public record StampInvoiceCommand(Guid InvoiceId) : IRequest<Result<string>>;
