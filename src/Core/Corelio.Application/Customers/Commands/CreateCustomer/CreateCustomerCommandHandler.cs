using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Handler for CreateCustomerCommand.
/// </summary>
public class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        // Validate RFC uniqueness if provided
        if (!string.IsNullOrWhiteSpace(request.Rfc))
        {
            var rfcExists = await customerRepository.RfcExistsAsync(request.Rfc, null, cancellationToken);
            if (rfcExists)
            {
                return Result<Guid>.Failure(
                    new Error("Customer.RfcExists", $"A customer with RFC '{request.Rfc}' already exists.", ErrorType.Conflict));
            }
        }

        var customer = new Customer
        {
            TenantId = tenantId.Value,
            CustomerType = request.CustomerType,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BusinessName = request.BusinessName,
            Rfc = request.Rfc,
            Curp = request.Curp,
            Email = request.Email,
            Phone = request.Phone,
            TaxRegime = request.TaxRegime,
            CfdiUse = request.CfdiUse,
            PreferredPaymentMethod = request.PreferredPaymentMethod
        };

        customerRepository.Add(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(customer.Id);
    }
}
