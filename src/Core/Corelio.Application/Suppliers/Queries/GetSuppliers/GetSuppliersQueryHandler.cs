using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Queries.GetSuppliers;

public class GetSuppliersQueryHandler(
    ISupplierRepository supplierRepository) : IRequestHandler<GetSuppliersQuery, Result<PagedResult<SupplierListDto>>>
{
    public async Task<Result<PagedResult<SupplierListDto>>> Handle(
        GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await supplierRepository.GetPagedAsync(
            request.Page,
            request.Size,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items.Select(s => new SupplierListDto(
            s.Id,
            s.Name,
            s.Rfc,
            s.ContactName,
            s.Email,
            s.Phone,
            s.IsActive,
            s.CreatedAt)).ToList();

        return Result<PagedResult<SupplierListDto>>.Success(
            PagedResult<SupplierListDto>.Create(dtos, request.Page, request.Size, totalCount));
    }
}
