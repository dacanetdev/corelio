using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.CancelSale;

/// <summary>
/// Handler for CancelSaleCommand. Only Draft sales can be cancelled (inventory not yet deducted).
/// </summary>
public class CancelSaleCommandHandler(
    ISaleRepository saleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelSaleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            return Result<bool>.Failure(
                new Error("Sale.NotFound", $"Sale '{request.SaleId}' not found.", ErrorType.NotFound));
        }

        if (sale.Status == SaleStatus.Cancelled)
        {
            return Result<bool>.Failure(
                new Error("Sale.AlreadyCancelled", $"Sale '{sale.Folio}' is already cancelled.", ErrorType.Conflict));
        }

        if (sale.Status == SaleStatus.Completed)
        {
            return Result<bool>.Failure(
                new Error("Sale.CannotCancelCompleted",
                    $"Sale '{sale.Folio}' is completed and cannot be cancelled. Use a refund instead.",
                    ErrorType.Conflict));
        }

        sale.Status = SaleStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            sale.Notes = string.IsNullOrWhiteSpace(sale.Notes)
                ? $"Cancelled: {request.Reason}"
                : $"{sale.Notes} | Cancelled: {request.Reason}";
        }

        saleRepository.Update(sale);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
