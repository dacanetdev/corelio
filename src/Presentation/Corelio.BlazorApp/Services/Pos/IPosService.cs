using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Pos;

namespace Corelio.BlazorApp.Services.Pos;

/// <summary>
/// Service interface for POS operations.
/// </summary>
public interface IPosService
{
    Task<Result<IEnumerable<PosProductModel>>> SearchProductsAsync(
        string term,
        int limit = 20,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateSaleAsync(
        CreateSaleRequestModel request,
        CancellationToken cancellationToken = default);

    Task<Result<SaleModel>> GetSaleAsync(
        Guid saleId,
        CancellationToken cancellationToken = default);

    Task<Result<SaleModel>> CompleteSaleAsync(
        Guid saleId,
        List<PaymentRequestModel> payments,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> CancelSaleAsync(
        Guid saleId,
        CancellationToken cancellationToken = default);
}
