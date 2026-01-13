using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.ResetPassword;

/// <summary>
/// Command to reset a user's password using a valid reset token.
/// </summary>
/// <param name="Email">The email address of the user.</param>
/// <param name="ResetToken">The password reset token received via email.</param>
/// <param name="NewPassword">The new password to set.</param>
public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword) : IRequest<Result<bool>>;
