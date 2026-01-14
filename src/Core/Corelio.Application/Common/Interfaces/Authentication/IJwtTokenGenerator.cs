namespace Corelio.Application.Common.Interfaces.Authentication;

/// <summary>
/// Service for generating JWT access tokens.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT access token with the specified claims.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="tenantId">The tenant's unique identifier.</param>
    /// <param name="roles">The user's roles.</param>
    /// <param name="permissions">The user's permissions.</param>
    /// <returns>A JWT access token string.</returns>
    string GenerateToken(
        Guid userId,
        string email,
        Guid tenantId,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);
}
