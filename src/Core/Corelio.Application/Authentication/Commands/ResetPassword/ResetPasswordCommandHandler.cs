using Corelio.Application.Common.Interfaces.Authentication;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.ResetPassword;

/// <summary>
/// Handler for the ResetPasswordCommand (stub implementation for MVP).
/// </summary>
public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider) : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Find user by email and reset token
        var user = await userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), null, cancellationToken);

        if (user is null || user.PasswordResetToken != request.ResetToken)
        {
            return Result<bool>.Failure(
                new Error("ResetPassword.InvalidToken", "Invalid or expired reset token.", ErrorType.Validation));
        }

        // Step 2: Validate token expiry
        if (user.PasswordResetExpiresAt.HasValue &&
            user.PasswordResetExpiresAt.Value < timeProvider.GetUtcNow().UtcDateTime)
        {
            return Result<bool>.Failure(
                new Error("ResetPassword.TokenExpired", "Password reset token has expired.", ErrorType.Validation));
        }

        // Step 3: Update password
        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetExpiresAt = null;

        // Reset failed login attempts and unlock account
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
