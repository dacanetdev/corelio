using Corelio.Application.Common.Interfaces.Email;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.ForgotPassword;

/// <summary>
/// Handler for the ForgotPasswordCommand (stub implementation for MVP).
/// </summary>
public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    TimeProvider timeProvider) : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Find user by email (security: don't reveal if email exists)
        var user = await userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), null, cancellationToken);

        if (user is null)
        {
            // Security: Return success even if user doesn't exist (prevent email enumeration)
            return Result<bool>.Success(true);
        }

        // Step 2: Generate password reset token
        user.PasswordResetToken = Guid.NewGuid().ToString("N");
        user.PasswordResetExpiresAt = timeProvider.GetUtcNow().AddHours(1).UtcDateTime; // 1-hour expiry

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 3: Send password reset email
        var resetLink = $"https://app.corelio.com.mx/reset-password?token={user.PasswordResetToken}";
        await emailService.SendPasswordResetAsync(user.Email, resetLink, cancellationToken);

        return Result<bool>.Success(true);
    }
}
