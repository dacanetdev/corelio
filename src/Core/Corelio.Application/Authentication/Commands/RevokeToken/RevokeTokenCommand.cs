using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.RevokeToken;

/// <summary>
/// Command to revoke a refresh token (logout).
/// </summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
public record RevokeTokenCommand(string RefreshToken) : IRequest<Result<bool>>;
