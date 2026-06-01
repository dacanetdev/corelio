using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Suppliers;

namespace Corelio.BlazorApp.Services.Suppliers;

/// <summary>
/// Service for supplier API calls.
/// </summary>
public interface ISupplierHttpService
{
    Task<Result<PagedResult<SupplierListModel>>> GetSuppliersAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<Result<SupplierModel>> GetSupplierByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateSupplierAsync(
        SupplierFormModel model,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> UpdateSupplierAsync(
        Guid id,
        SupplierFormModel model,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteSupplierAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
