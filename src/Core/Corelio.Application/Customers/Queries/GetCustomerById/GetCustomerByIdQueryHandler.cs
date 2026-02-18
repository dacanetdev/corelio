using Corelio.Application.Common.Models;
using Corelio.Application.Customers.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Handler for GetCustomerByIdQuery.
/// </summary>
public class GetCustomerByIdQueryHandler(
    ICustomerRepository customerRepository) : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerDto>.Failure(
                new Error("Customer.NotFound", $"Customer with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        var dto = new CustomerDto(
            customer.Id,
            customer.CustomerType,
            customer.FirstName,
            customer.LastName,
            customer.BusinessName,
            customer.FullName,
            customer.Rfc,
            customer.Curp,
            customer.Email,
            customer.Phone,
            customer.TaxRegime,
            customer.CfdiUse,
            customer.PreferredPaymentMethod,
            customer.CreatedAt,
            customer.UpdatedAt);

        return Result<CustomerDto>.Success(dto);
    }
}
