namespace Corelio.Application.Common.Interfaces.Email;

/// <summary>
/// Service for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The email body (HTML or plain text).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email confirmation link to a user.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="confirmationLink">The email confirmation link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendEmailConfirmationAsync(string to, string confirmationLink, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset link to a user.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="resetLink">The password reset link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendPasswordResetAsync(string to, string resetLink, CancellationToken cancellationToken = default);
}
