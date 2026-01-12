namespace Corelio.Domain.Common.Interfaces;

/// <summary>
/// Provides the current user ID for audit tracking.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// Gets the current user ID, or null if not authenticated.
    /// </summary>
    Guid? UserId { get; }
}
