using System.Security.Cryptography;
using System.Text;
using Corelio.Application.Authentication.Common;
using Corelio.Application.Common.Interfaces.Authentication;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.RefreshToken;

/// <summary>
/// Handler for the RefreshTokenCommand that validates a refresh token and issues new tokens.
/// </summary>
public class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    TimeProvider timeProvider) : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Hash the incoming refresh token to match against database
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(request.RefreshToken)));

        // Step 2: Find the refresh token in the database with user and role information
        var refreshTokenEntity = await refreshTokenRepository.GetByTokenHashWithUserAsync(tokenHash, cancellationToken);

        if (refreshTokenEntity is null)
        {
            return Result<TokenResponse>.Failure(
                new Error("RefreshToken.Invalid", "Invalid refresh token.", ErrorType.Unauthorized));
        }

        // Step 3: Validate the refresh token
        if (!refreshTokenEntity.IsValid)
        {
            var reason = refreshTokenEntity.IsRevoked
                ? "Refresh token has been revoked."
                : "Refresh token has expired.";

            return Result<TokenResponse>.Failure(
                new Error("RefreshToken.Invalid", reason, ErrorType.Unauthorized));
        }

        // Step 4: Check user account status
        var user = refreshTokenEntity.User;
        if (!user.IsActive)
        {
            return Result<TokenResponse>.Failure(
                new Error("User.Inactive", "User account is inactive.", ErrorType.Unauthorized));
        }

        if (user.IsLocked)
        {
            return Result<TokenResponse>.Failure(
                new Error("User.Locked", "User account is locked.", ErrorType.Unauthorized));
        }

        // Step 5: Extract roles and permissions
        var roles = user.UserRoles
            .Where(ur => !ur.IsExpired)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToArray();

        var permissions = user.UserRoles
            .Where(ur => !ur.IsExpired)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToArray();

        // Step 6: Generate new access token
        var accessToken = jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.TenantId,
            roles,
            permissions);

        var expiresAt = timeProvider.GetUtcNow().AddMinutes(60).UtcDateTime; // 1 hour

        // Step 7: Token rotation - Generate new refresh token and revoke old one
        var (newRefreshToken, newRefreshTokenHash) = refreshTokenGenerator.GenerateToken();

        var newRefreshTokenEntity = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            TokenHash = newRefreshTokenHash,
            Jti = Guid.NewGuid().ToString(),
            ExpiresAt = timeProvider.GetUtcNow().AddDays(7).UtcDateTime, // 7 days
            IsRevoked = false,
            IpAddress = null, // TODO: Get from HttpContext
            UserAgent = null, // TODO: Get from HttpContext
            DeviceId = refreshTokenEntity.DeviceId, // Preserve device ID
            UseCount = 0
        };

        await refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);

        // Revoke the old refresh token
        refreshTokenEntity.IsRevoked = true;
        refreshTokenEntity.RevokedAt = timeProvider.GetUtcNow().UtcDateTime;
        refreshTokenEntity.RevokedBy = user.Id;
        refreshTokenEntity.RevocationReason = "Token rotated";

        // Update last used tracking for the old token
        refreshTokenEntity.LastUsedAt = timeProvider.GetUtcNow().UtcDateTime;
        refreshTokenEntity.UseCount++;

        refreshTokenRepository.Update(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 8: Return new tokens
        return Result<TokenResponse>.Success(
            new TokenResponse(accessToken, newRefreshToken, expiresAt));
    }
}
