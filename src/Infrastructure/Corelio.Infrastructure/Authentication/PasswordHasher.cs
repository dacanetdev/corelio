using Corelio.Application.Common.Interfaces.Authentication;

namespace Corelio.Infrastructure.Authentication;

/// <summary>
/// Service for securely hashing and verifying passwords using BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// The BCrypt work factor (cost parameter).
    /// Higher values increase security but also processing time.
    /// Recommended: 12 for most applications.
    /// </summary>
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
