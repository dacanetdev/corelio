namespace Corelio.Domain.Enums;

/// <summary>
/// Status of a CFDI invoice.
/// </summary>
public enum CfdiStatus
{
    /// <summary>
    /// Draft, not yet stamped.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Currently being stamped by PAC.
    /// </summary>
    Stamping = 1,

    /// <summary>
    /// Successfully stamped by PAC.
    /// </summary>
    Stamped = 2,

    /// <summary>
    /// Sent to customer via email.
    /// </summary>
    Sent = 3,

    /// <summary>
    /// Cancelled with SAT.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Error during stamping process.
    /// </summary>
    Error = 5
}
