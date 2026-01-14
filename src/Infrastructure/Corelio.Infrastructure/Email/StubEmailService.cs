using Corelio.Application.Common.Interfaces.Email;
using Microsoft.Extensions.Logging;

namespace Corelio.Infrastructure.Email;

/// <summary>
/// Stub email service for MVP that logs emails to console instead of sending them.
/// Replace with real SMTP implementation in production.
/// </summary>
public class StubEmailService(ILogger<StubEmailService> logger) : IEmailService
{
    /// <inheritdoc />
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "EMAIL STUB: To={To}, Subject={Subject}, Body={Body}",
            to,
            subject,
            body);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendEmailConfirmationAsync(string to, string confirmationLink, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "EMAIL CONFIRMATION STUB: To={To}, ConfirmationLink={Link}",
            to,
            confirmationLink);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendPasswordResetAsync(string to, string resetLink, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "PASSWORD RESET STUB: To={To}, ResetLink={Link}",
            to,
            resetLink);

        return Task.CompletedTask;
    }
}
