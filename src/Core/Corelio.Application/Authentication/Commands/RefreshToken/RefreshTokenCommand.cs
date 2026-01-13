using Corelio.Application.Authentication.Common;
using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.RefreshToken;

/// <summary>
/// Command to refresh an access token using a valid refresh token.
/// Implements token rotation: generates new tokens and revokes the old refresh token.
/// </summary>
/// <param name="RefreshToken">The refresh token to validate and use for generating new tokens.</param>
public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponse>>;
