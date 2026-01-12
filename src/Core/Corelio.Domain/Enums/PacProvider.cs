namespace Corelio.Domain.Enums;

/// <summary>
/// PAC (Proveedor Autorizado de Certificación) providers for CFDI stamping.
/// </summary>
public enum PacProvider
{
    /// <summary>
    /// Finkel PAC provider.
    /// </summary>
    Finkel = 0,

    /// <summary>
    /// Divertia PAC provider.
    /// </summary>
    Divertia = 1,

    /// <summary>
    /// SW PAC provider.
    /// </summary>
    Sw = 2,

    /// <summary>
    /// Edicom PAC provider.
    /// </summary>
    Edicom = 3
}
