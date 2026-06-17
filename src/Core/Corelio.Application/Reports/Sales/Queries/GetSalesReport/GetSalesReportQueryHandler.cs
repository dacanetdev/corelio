using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Sales.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Sales.Queries.GetSalesReport;

/// <summary>
/// Handler for GetSalesReportQuery — delegates aggregation to ISalesReportQueryService.
/// </summary>
public class GetSalesReportQueryHandler(ISalesReportQueryService salesReportQueryService)
    : IRequestHandler<GetSalesReportQuery, Result<SalesReportDto>>
{
    public async Task<Result<SalesReportDto>> Handle(
        GetSalesReportQuery request,
        CancellationToken cancellationToken)
    {
        var report = await salesReportQueryService.GetSalesReportAsync(
            request.DateFrom,
            request.DateTo,
            request.WarehouseId,
            request.PaymentMethod,
            cancellationToken);

        return Result<SalesReportDto>.Success(report);
    }
}
