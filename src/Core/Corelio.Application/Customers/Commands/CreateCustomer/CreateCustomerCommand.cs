using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Command to create a new customer.
/// </summary>
public record CreateCustomerCommand(
    CustomerType CustomerType,
    string FirstName,
    string LastName,
    string? BusinessName,
    string? Rfc,
    string? Curp,
    string? Email,
    string? Phone,
    string? TaxRegime,
    string? CfdiUse,
    PaymentMethod? PreferredPaymentMethod) : IRequest<Result<Guid>>;
