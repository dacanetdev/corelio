using Corelio.Application.Common.Models;
using Corelio.Application.Customers.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Query to get a customer by ID.
/// </summary>
public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;
