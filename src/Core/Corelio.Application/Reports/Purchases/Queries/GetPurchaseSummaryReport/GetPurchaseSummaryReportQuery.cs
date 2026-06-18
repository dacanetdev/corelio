using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Purchases.Common;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Purchases.Queries.GetPurchaseSummaryReport;

/// <summary>
/// Query to retrieve aggregated purchase summary report data for a date range.
/// </summary>
public record GetPurchaseSummaryReportQuery(
    DateTime DateFrom,
    DateTime DateTo,
    Guid? SupplierId = null,
    PurchaseOrderStatus? Status = null) : IRequest<Result<PurchaseSummaryReportDto>>;
