using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Purchases.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Purchases.Queries.GetPurchaseSummaryReport;

/// <summary>
/// Handler for GetPurchaseSummaryReportQuery — delegates aggregation to IPurchaseSummaryQueryService.
/// </summary>
public class GetPurchaseSummaryReportQueryHandler(IPurchaseSummaryQueryService purchaseSummaryQueryService)
    : IRequestHandler<GetPurchaseSummaryReportQuery, Result<PurchaseSummaryReportDto>>
{
    public async Task<Result<PurchaseSummaryReportDto>> Handle(
        GetPurchaseSummaryReportQuery request,
        CancellationToken cancellationToken)
    {
        var report = await purchaseSummaryQueryService.GetPurchaseSummaryReportAsync(
            request.DateFrom,
            request.DateTo,
            request.SupplierId,
            request.Status,
            cancellationToken);

        return Result<PurchaseSummaryReportDto>.Success(report);
    }
}
