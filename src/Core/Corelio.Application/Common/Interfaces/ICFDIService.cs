namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Orchestrates the full CFDI stamping workflow:
/// LoadCert → GenerateXML → Sign → PAC.Stamp → Persist
/// </summary>
public interface ICFDIService
{
    /// <summary>
    /// Stamps a Draft invoice: generates signed XML, sends to PAC,
    /// persists UUID + stamp seals, transitions status to Stamped.
    /// Returns the SAT UUID on success.
    /// </summary>
    Task<string> StampAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a Stamped invoice via PAC. Validates 72-hour cancellation window.
    /// </summary>
    Task CancelAsync(Guid invoiceId, string reason, CancellationToken cancellationToken = default);
}
