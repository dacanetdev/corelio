using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a JWT refresh token.
/// </summary>
public class RefreshToken : BaseEntity, ITenantEntity
{
    /// <summary>
    /// The tenant this token belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The user this token belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// SHA256 hash of the refresh token for secure storage.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// JWT ID (jti) for token identification and blacklisting.
    /// </summary>
    public string Jti { get; set; } = string.Empty;

    /// <summary>
    /// When the token expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether the token has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>
    /// When the token was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Who revoked the token.
    /// </summary>
    public Guid? RevokedBy { get; set; }

    /// <summary>
    /// Reason for revocation.
    /// </summary>
    public string? RevocationReason { get; set; }

    /// <summary>
    /// IP address where the token was created.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent of the client that created the token.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Device identifier.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// When the token was last used.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Number of times the token has been used.
    /// </summary>
    public int UseCount { get; set; } = 0;

    /// <summary>
    /// Whether the token is currently valid.
    /// </summary>
    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    // Navigation property
    /// <summary>
    /// The user this token belongs to.
    /// </summary>
    public User User { get; set; } = null!;
}
