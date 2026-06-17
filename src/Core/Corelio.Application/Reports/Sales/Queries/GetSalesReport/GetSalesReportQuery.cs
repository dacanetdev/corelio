using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Sales.Common;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Sales.Queries.GetSalesReport;

/// <summary>
/// Query to retrieve aggregated sales report data for a date range.
/// </summary>
public record GetSalesReportQuery(
    DateTime DateFrom,
    DateTime DateTo,
    Guid? WarehouseId = null,
    PaymentMethod? PaymentMethod = null) : IRequest<Result<SalesReportDto>>;
