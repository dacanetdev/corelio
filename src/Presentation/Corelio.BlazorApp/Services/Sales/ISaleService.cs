using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Pos;

namespace Corelio.BlazorApp.Services.Sales;

/// <summary>
/// Service interface for sale history and management operations.
/// </summary>
public interface ISaleService
{
    Task<Result<PagedResult<SaleListModel>>> GetSalesAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    Task<Result<SaleModel>> GetSaleByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> CancelSaleAsync(
        Guid id,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> DownloadReceiptAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
