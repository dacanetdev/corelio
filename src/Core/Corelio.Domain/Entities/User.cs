using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a user account in the system.
/// </summary>
public class User : AuditableEntity, ITenantEntity
{
    /// <summary>
    /// The tenant this user belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    // Credentials
    /// <summary>
    /// User's email address (unique per tenant).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's username (unique per tenant).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    // Personal Information
    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name (computed).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// User's phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// User's mobile number.
    /// </summary>
    public string? Mobile { get; set; }

    // Employment Info
    /// <summary>
    /// Employee code/ID.
    /// </summary>
    public string? EmployeeCode { get; set; }

    /// <summary>
    /// Job position/title.
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Date of hire.
    /// </summary>
    public DateOnly? HireDate { get; set; }

    // Security
    /// <summary>
    /// Whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the email has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; set; } = false;

    /// <summary>
    /// Email confirmation token.
    /// </summary>
    public string? EmailConfirmationToken { get; set; }

    /// <summary>
    /// Password reset token.
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// When the password reset token expires.
    /// </summary>
    public DateTime? PasswordResetExpiresAt { get; set; }

    /// <summary>
    /// Whether two-factor authentication is enabled.
    /// </summary>
    public bool TwoFactorEnabled { get; set; } = false;

    /// <summary>
    /// Two-factor authentication secret.
    /// </summary>
    public string? TwoFactorSecret { get; set; }

    // Login Tracking
    /// <summary>
    /// When the user last logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// IP address of last login.
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// Number of consecutive failed login attempts.
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// When the account lockout expires.
    /// </summary>
    public DateTime? LockedUntil { get; set; }

    /// <summary>
    /// Whether the account is currently locked.
    /// </summary>
    public bool IsLocked => LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// The tenant this user belongs to.
    /// </summary>
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Roles assigned to this user.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = [];

    /// <summary>
    /// Refresh tokens for this user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
