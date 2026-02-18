using Corelio.Application.Common.Models;
using Corelio.Application.Customers.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Queries.GetCustomers;

/// <summary>
/// Handler for GetCustomersQuery.
/// </summary>
public class GetCustomersQueryHandler(
    ICustomerRepository customerRepository) : IRequestHandler<GetCustomersQuery, Result<PagedResult<CustomerListDto>>>
{
    public async Task<Result<PagedResult<CustomerListDto>>> Handle(
        GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await customerRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            cancellationToken);

        var dtos = items.Select(c => new CustomerListDto(
            c.Id,
            c.CustomerType,
            c.FullName,
            c.Rfc,
            c.Email,
            c.Phone,
            c.CreatedAt)).ToList();

        var pagedResult = PagedResult<CustomerListDto>.Create(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result<PagedResult<CustomerListDto>>.Success(pagedResult);
    }
}
