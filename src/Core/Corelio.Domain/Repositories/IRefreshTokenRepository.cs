using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for RefreshToken aggregate.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Gets a refresh token by its hash with the associated user (including roles and permissions).
    /// </summary>
    /// <param name="tokenHash">The SHA256 hash of the token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refresh token with user data if found, otherwise null.</returns>
    Task<RefreshToken?> GetByTokenHashWithUserAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new refresh token to the repository.
    /// </summary>
    /// <param name="refreshToken">The refresh token to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to update.</param>
    void Update(RefreshToken refreshToken);
}
