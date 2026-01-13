using Corelio.Application.Authentication.Common;
using Corelio.Application.Common.Models;
using MediatR;

namespace Corelio.Application.Authentication.Commands.Login;

/// <summary>
/// Command to authenticate a user and issue JWT tokens.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <param name="TenantSubdomain">Optional tenant subdomain for multi-tenant login (if not provided via JWT claim).</param>
public record LoginCommand(
    string Email,
    string Password,
    string? TenantSubdomain = null) : IRequest<Result<AuthenticationResult>>;
