using Corelio.Application.Common.Models;
using Corelio.Application.Customers.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Queries.SearchCustomers;

/// <summary>
/// Query to search customers by name or RFC (for POS quick lookup).
/// </summary>
public record SearchCustomersQuery(string Term) : IRequest<Result<List<CustomerListDto>>>;
