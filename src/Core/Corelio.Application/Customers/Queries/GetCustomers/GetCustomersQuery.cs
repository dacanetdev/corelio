using Corelio.Application.Common.Models;
using Corelio.Application.Customers.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Queries.GetCustomers;

/// <summary>
/// Query to get a paged list of customers.
/// </summary>
public record GetCustomersQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null) : IRequest<Result<PagedResult<CustomerListDto>>>;
