using System.Security.Cryptography;
using System.Text;
using Corelio.Application.Common.Interfaces.Authentication;

namespace Corelio.Infrastructure.Authentication;

/// <summary>
/// Service for generating secure refresh tokens.
/// </summary>
public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    /// <summary>
    /// The size of the random token in bytes (64 bytes = 512 bits).
    /// </summary>
    private const int TokenSizeBytes = 64;

    /// <inheritdoc />
    public (string Token, string TokenHash) GenerateToken()
    {
        // Generate a cryptographically secure random token
        var tokenBytes = RandomNumberGenerator.GetBytes(TokenSizeBytes);
        var token = Convert.ToBase64String(tokenBytes);

        // Hash the token with SHA256 for database storage
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        return (token, tokenHash);
    }
}
