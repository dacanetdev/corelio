using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Command to update an existing customer.
/// </summary>
public record UpdateCustomerCommand(
    Guid Id,
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
    PaymentMethod? PreferredPaymentMethod) : IRequest<Result<bool>>;
