using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.ForgotPassword;

/// <summary>
/// Command to initiate a password reset by sending a reset link to the user's email.
/// </summary>
/// <param name="Email">The email address of the user requesting password reset.</param>
public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;
