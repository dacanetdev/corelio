namespace Corelio.Application.CFDI.Queries.GetCfdiSettings;

/// <summary>
/// CFDI issuer settings DTO (no certificate bytes).
/// </summary>
public record CfdiSettingsDto(
    string? IssuerRfc,
    string? IssuerName,
    string? IssuerTaxRegime,
    string? IssuerPostalCode,
    string CfdiSeries,
    int CfdiNextFolio,
    DateTime? CfdiCertificateExpiresAt,
    bool HasCertificate);
