namespace Corelio.Application.Common.Interfaces.Authentication;

/// <summary>
/// Service for generating secure refresh tokens.
/// </summary>
public interface IRefreshTokenGenerator
{
    /// <summary>
    /// Generates a cryptographically secure refresh token and its hash.
    /// </summary>
    /// <returns>
    /// A tuple containing:
    /// - Token: The refresh token to be sent to the client (Base64-encoded).
    /// - TokenHash: The SHA256 hash of the token to be stored in the database.
    /// </returns>
    (string Token, string TokenHash) GenerateToken();
}
