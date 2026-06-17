using Corelio.Application.Reports.Sales.Common;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Generates exportable files (PDF and CSV) from a sales report DTO.
/// </summary>
public interface ISalesReportExportService
{
    /// <summary>
    /// Generates a PDF document summarizing the sales report.
    /// </summary>
    Task<byte[]> GeneratePdfAsync(SalesReportDto report, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a CSV file with multiple sections: KPIs, payment breakdown, top products, hourly distribution.
    /// </summary>
    Task<byte[]> GenerateCsvAsync(SalesReportDto report, CancellationToken cancellationToken = default);
}
