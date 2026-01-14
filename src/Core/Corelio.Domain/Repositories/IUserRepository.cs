using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for User aggregate.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by email address within a specific tenant.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="tenantId">Optional tenant ID for filtering.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByEmailAsync(string email, Guid? tenantId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by ID with roles and permissions eagerly loaded.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user with roles and permissions if found, otherwise null.</returns>
    Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the given email exists within a tenant.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the email exists, otherwise false.</returns>
    Task<bool> ExistsByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user to update.</param>
    void Update(User user);
}
