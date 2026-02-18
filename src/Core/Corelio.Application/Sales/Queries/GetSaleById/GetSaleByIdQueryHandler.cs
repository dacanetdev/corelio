using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Queries.GetSaleById;

/// <summary>
/// Handler for GetSaleByIdQuery.
/// </summary>
public class GetSaleByIdQueryHandler(
    ISaleRepository saleRepository) : IRequestHandler<GetSaleByIdQuery, Result<SaleDto>>
{
    public async Task<Result<SaleDto>> Handle(
        GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale is null)
        {
            return Result<SaleDto>.Failure(
                new Error("Sale.NotFound", $"Sale '{request.Id}' not found.", ErrorType.NotFound));
        }

        var items = sale.Items.Select(i => new SaleItemDto(
            i.Id, i.ProductId, i.ProductName, i.ProductSku,
            i.UnitPrice, i.Quantity, i.DiscountPercentage, i.TaxPercentage, i.LineTotal)).ToList();

        var payments = sale.Payments.Select(p => new PaymentDto(
            p.Id, p.Method, p.Amount, p.Reference, p.Status)).ToList();

        var dto = new SaleDto(
            sale.Id,
            sale.Folio,
            sale.Type,
            sale.Status,
            sale.CustomerId,
            sale.Customer?.FullName,
            sale.WarehouseId,
            sale.Warehouse?.Name ?? string.Empty,
            sale.SubTotal,
            sale.DiscountAmount,
            sale.TaxAmount,
            sale.Total,
            sale.Notes,
            sale.CompletedAt,
            sale.CreatedAt,
            items,
            payments);

        return Result<SaleDto>.Success(dto);
    }
}
