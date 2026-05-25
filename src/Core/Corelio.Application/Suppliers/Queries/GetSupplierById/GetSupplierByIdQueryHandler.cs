using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(
    ISupplierRepository supplierRepository) : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
        {
            return Result<SupplierDto>.Failure(new Error("Supplier.NotFound", $"Supplier with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        return Result<SupplierDto>.Success(new SupplierDto(
            supplier.Id,
            supplier.Name,
            supplier.Rfc,
            supplier.ContactName,
            supplier.Email,
            supplier.Phone,
            supplier.Street,
            supplier.City,
            supplier.State,
            supplier.ZipCode,
            supplier.PaymentTermsDays,
            supplier.TaxRegime,
            supplier.Notes,
            supplier.IsActive,
            supplier.CreatedAt,
            supplier.UpdatedAt));
    }
}
