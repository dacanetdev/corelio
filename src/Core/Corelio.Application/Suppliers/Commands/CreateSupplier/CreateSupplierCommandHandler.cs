using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<CreateSupplierCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        if (!string.IsNullOrWhiteSpace(request.Rfc))
        {
            var rfcExists = await supplierRepository.RfcExistsAsync(request.Rfc, null, cancellationToken);
            if (rfcExists)
            {
                return Result<Guid>.Failure(new Error("Supplier.RfcExists", $"A supplier with RFC '{request.Rfc}' already exists.", ErrorType.Conflict));
            }
        }

        var supplier = new Supplier
        {
            TenantId = tenantId.Value,
            Name = request.Name,
            Rfc = request.Rfc,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            PaymentTermsDays = request.PaymentTermsDays,
            TaxRegime = request.TaxRegime,
            Notes = request.Notes
        };

        supplierRepository.Add(supplier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(supplier.Id);
    }
}
