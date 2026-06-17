using Corelio.Application.Reports.Inventory.Common;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Generates PDF and CSV exports for the inventory valuation report.
/// </summary>
public interface IInventoryReportExportService
{
    /// <summary>
    /// Generates a PDF document summarizing the inventory valuation report.
    /// </summary>
    Task<byte[]> GeneratePdfAsync(InventoryValuationReportDto report, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a CSV file with sections: summary KPIs, category breakdown, product detail, low-stock alerts.
    /// </summary>
    Task<byte[]> GenerateCsvAsync(InventoryValuationReportDto report, CancellationToken cancellationToken = default);
}
