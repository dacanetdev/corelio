using Corelio.Application.Reports.Purchases.Common;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Generates exportable files (PDF and CSV) from a purchase summary report DTO.
/// </summary>
public interface IPurchaseReportExportService
{
    /// <summary>
    /// Generates a PDF document summarizing the purchase report.
    /// </summary>
    Task<byte[]> GeneratePdfAsync(PurchaseSummaryReportDto report, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a CSV file with multiple sections: summary, supplier spending, status breakdown, product receipts.
    /// </summary>
    Task<byte[]> GenerateCsvAsync(PurchaseSummaryReportDto report, CancellationToken cancellationToken = default);
}
