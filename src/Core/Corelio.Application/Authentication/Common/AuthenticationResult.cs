namespace Corelio.Application.Authentication.Common;

/// <summary>
/// Represents the complete result of a successful authentication (login).
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="TenantId">The tenant's unique identifier.</param>
/// <param name="Roles">The user's roles.</param>
/// <param name="Permissions">The user's permissions.</param>
/// <param name="Tokens">The authentication tokens (access + refresh).</param>
public record AuthenticationResult(
    Guid UserId,
    string Email,
    Guid TenantId,
    string[] Roles,
    string[] Permissions,
    TokenResponse Tokens);
