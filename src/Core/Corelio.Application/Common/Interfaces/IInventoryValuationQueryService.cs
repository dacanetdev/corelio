using Corelio.Application.Reports.Inventory.Common;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Aggregates inventory data for the valuation report.
/// Implemented by Infrastructure using EF Core.
/// </summary>
public interface IInventoryValuationQueryService
{
    /// <summary>
    /// Returns aggregated inventory valuation including per-category breakdown,
    /// full product detail, and low-stock alerts for the current tenant.
    /// </summary>
    Task<InventoryValuationReportDto> GetInventoryValuationReportAsync(
        Guid? warehouseId = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);
}
