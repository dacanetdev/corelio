using Corelio.Application.Common.Models;
using Corelio.Application.Customers.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Queries.SearchCustomers;

/// <summary>
/// Handler for SearchCustomersQuery.
/// </summary>
public class SearchCustomersQueryHandler(
    ICustomerRepository customerRepository) : IRequestHandler<SearchCustomersQuery, Result<List<CustomerListDto>>>
{
    public async Task<Result<List<CustomerListDto>>> Handle(
        SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Term))
        {
            return Result<List<CustomerListDto>>.Success([]);
        }

        var customers = await customerRepository.SearchAsync(request.Term, cancellationToken);

        var dtos = customers.Select(c => new CustomerListDto(
            c.Id,
            c.CustomerType,
            c.FullName,
            c.Rfc,
            c.Email,
            c.Phone,
            c.CreatedAt)).ToList();

        return Result<List<CustomerListDto>>.Success(dtos);
    }
}
