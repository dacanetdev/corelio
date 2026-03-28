using Corelio.Domain.Entities;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Generates PDF receipts for completed sales.
/// </summary>
public interface ISaleReceiptService
{
    /// <summary>
    /// Generates a PDF receipt for the given sale.
    /// The sale must have Items and Payments navigation properties loaded.
    /// </summary>
    Task<byte[]> GenerateAsync(Sale sale, CancellationToken cancellationToken = default);
}
