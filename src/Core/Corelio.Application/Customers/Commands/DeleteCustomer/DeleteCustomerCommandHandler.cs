using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Handler for DeleteCustomerCommand.
/// </summary>
public class DeleteCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCustomerCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return Result<bool>.Failure(
                new Error("Customer.NotFound", $"Customer with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        customerRepository.Delete(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
