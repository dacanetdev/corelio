using Corelio.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Corelio.Infrastructure.Services;

/// <summary>
/// Provides the current user ID from HTTP context for audit tracking.
/// </summary>
public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    /// <inheritdoc />
    public Guid? UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
