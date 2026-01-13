using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.RegisterUser;

/// <summary>
/// Command to register a new user within the current tenant.
/// The tenant ID is obtained from ITenantService (never from client input).
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="RoleCodes">The role codes to assign to the user (e.g., ["Cashier", "Seller"]).</param>
public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string[] RoleCodes) : IRequest<Result<Guid>>;
