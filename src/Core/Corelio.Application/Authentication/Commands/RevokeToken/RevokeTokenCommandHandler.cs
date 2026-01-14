using System.Security.Cryptography;
using System.Text;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.RevokeToken;

/// <summary>
/// Handler for the RevokeTokenCommand that revokes a refresh token (logout).
/// </summary>
public class RevokeTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<RevokeTokenCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        RevokeTokenCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Hash the incoming refresh token to match against database
        var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));

        // Step 2: Find the refresh token in the database
        var refreshTokenEntity = await refreshTokenRepository.GetByTokenHashWithUserAsync(tokenHash, cancellationToken);

        if (refreshTokenEntity is null)
        {
            return Result<bool>.Failure(
                new Error("RefreshToken.NotFound", "Refresh token not found.", ErrorType.NotFound));
        }

        // Step 3: Check if already revoked
        if (refreshTokenEntity.IsRevoked)
        {
            return Result<bool>.Failure(
                new Error("RefreshToken.AlreadyRevoked", "Refresh token has already been revoked.", ErrorType.Validation));
        }

        // Step 4: Revoke the token
        refreshTokenEntity.IsRevoked = true;
        refreshTokenEntity.RevokedAt = timeProvider.GetUtcNow().UtcDateTime;
        refreshTokenEntity.RevokedBy = refreshTokenEntity.UserId; // User revoking their own token
        refreshTokenEntity.RevocationReason = "User logout";

        refreshTokenRepository.Update(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
