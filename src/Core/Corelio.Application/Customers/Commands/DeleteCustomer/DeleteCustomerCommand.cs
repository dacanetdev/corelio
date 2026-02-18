using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Command to soft-delete a customer.
/// </summary>
public record DeleteCustomerCommand(Guid Id) : IRequest<Result<bool>>;
