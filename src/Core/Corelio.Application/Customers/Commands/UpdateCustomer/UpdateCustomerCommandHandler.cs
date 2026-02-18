using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Handler for UpdateCustomerCommand.
/// </summary>
public class UpdateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCustomerCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return Result<bool>.Failure(
                new Error("Customer.NotFound", $"Customer with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        // Check RFC uniqueness if changed
        if (!string.IsNullOrWhiteSpace(request.Rfc) && request.Rfc != customer.Rfc)
        {
            var rfcExists = await customerRepository.RfcExistsAsync(request.Rfc, request.Id, cancellationToken);
            if (rfcExists)
            {
                return Result<bool>.Failure(
                    new Error("Customer.RfcExists", $"A customer with RFC '{request.Rfc}' already exists.", ErrorType.Conflict));
            }
        }

        customer.CustomerType = request.CustomerType;
        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.BusinessName = request.BusinessName;
        customer.Rfc = request.Rfc;
        customer.Curp = request.Curp;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.TaxRegime = request.TaxRegime;
        customer.CfdiUse = request.CfdiUse;
        customer.PreferredPaymentMethod = request.PreferredPaymentMethod;

        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
