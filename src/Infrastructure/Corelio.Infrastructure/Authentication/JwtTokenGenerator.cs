using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Corelio.Application.Common.Interfaces.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Corelio.Infrastructure.Authentication;

/// <summary>
/// Service for generating JWT access tokens.
/// </summary>
public class JwtTokenGenerator(
    IOptions<JwtSettings> jwtSettings,
    TimeProvider timeProvider) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    /// <inheritdoc />
    public string GenerateToken(
        Guid userId,
        string email,
        Guid tenantId,
        IEnumerable<string> roles,
        IEnumerable<string> permissions)
    {
        // Create claims for the JWT token
        var claims = new List<Claim>
        {
            // Standard JWT claims
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Custom claims
            new("tenant_id", tenantId.ToString())
        };

        // Add role claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add permission claims
        claims.AddRange(permissions.Select(permission => new Claim("permissions", permission)));

        // Create signing credentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Calculate expiry time
        var expiryDateTime = timeProvider.GetUtcNow().AddMinutes(_jwtSettings.ExpiryMinutes).UtcDateTime;

        // Create the JWT token
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiryDateTime,
            signingCredentials: credentials);

        // Serialize the token to a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
