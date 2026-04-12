using Corelio.BlazorApp.Models.Cfdi;
using Corelio.BlazorApp.Models.Common;

namespace Corelio.BlazorApp.Services.Cfdi;

/// <summary>
/// Service for CFDI invoice and settings API calls.
/// </summary>
public interface ICfdiHttpService
{
    Task<Result<PagedResult<InvoiceListModel>>> GetInvoicesAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<Result<InvoiceModel>> GetInvoiceByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> GenerateInvoiceAsync(
        GenerateInvoiceRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> StampInvoiceAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> CancelInvoiceAsync(
        Guid id,
        string reason,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> DownloadXmlAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> DownloadPdfAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<CfdiSettingsModel>> GetSettingsAsync(
        CancellationToken cancellationToken = default);

    Task<Result<bool>> UpdateSettingsAsync(
        UpdateCfdiSettingsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> UploadCertificateAsync(
        Stream fileStream,
        string fileName,
        string password,
        CancellationToken cancellationToken = default);
}
