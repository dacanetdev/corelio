using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<UpdateSupplierCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<bool>.Failure(new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
        {
            return Result<bool>.Failure(new Error("Supplier.NotFound", $"Supplier with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        if (!string.IsNullOrWhiteSpace(request.Rfc))
        {
            var rfcExists = await supplierRepository.RfcExistsAsync(request.Rfc, request.Id, cancellationToken);
            if (rfcExists)
            {
                return Result<bool>.Failure(new Error("Supplier.RfcExists", $"A supplier with RFC '{request.Rfc}' already exists.", ErrorType.Conflict));
            }
        }

        supplier.Name = request.Name;
        supplier.Rfc = request.Rfc;
        supplier.ContactName = request.ContactName;
        supplier.Email = request.Email;
        supplier.Phone = request.Phone;
        supplier.Street = request.Street;
        supplier.City = request.City;
        supplier.State = request.State;
        supplier.ZipCode = request.ZipCode;
        supplier.PaymentTermsDays = request.PaymentTermsDays;
        supplier.TaxRegime = request.TaxRegime;
        supplier.Notes = request.Notes;
        supplier.IsActive = request.IsActive;

        supplierRepository.Update(supplier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
