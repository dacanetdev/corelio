using Corelio.Domain.Entities.CFDI;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Generates a PDF representation of a stamped CFDI invoice.
/// </summary>
public interface IInvoicePdfService
{
    /// <summary>
    /// Generates an A4 PDF for the given invoice.
    /// The invoice must have Items loaded.
    /// </summary>
    Task<byte[]> GenerateAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
