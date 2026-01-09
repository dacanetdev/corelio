namespace Corelio.Domain.Enums;

/// <summary>
/// CFDI document types (TipoDeComprobante).
/// </summary>
public enum CfdiType
{
    /// <summary>
    /// Ingreso - Income invoice.
    /// </summary>
    Ingreso = 0,

    /// <summary>
    /// Egreso - Credit note/refund.
    /// </summary>
    Egreso = 1,

    /// <summary>
    /// Traslado - Transfer document.
    /// </summary>
    Traslado = 2,

    /// <summary>
    /// Pago - Payment complement.
    /// </summary>
    Pago = 3,

    /// <summary>
    /// Nomina - Payroll.
    /// </summary>
    Nomina = 4
}
