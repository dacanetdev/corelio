using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Corelio.Infrastructure.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Corelio.Infrastructure.Tests.Authentication;

public class JwtTokenGeneratorTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenGenerator _tokenGenerator;

    public JwtTokenGeneratorTests()
    {
        _jwtSettings = new JwtSettings
        {
            Secret = "ThisIsAVerySecretKeyForTestingPurposesWithAtLeast256Bits!!",
            Issuer = "Corelio",
            Audience = "Corelio.WebAPI",
            ExpiryMinutes = 60
        };

        var options = Options.Create(_jwtSettings);
        _tokenGenerator = new JwtTokenGenerator(options, TimeProvider.System);
    }

    [Fact]
    public void GenerateToken_WithValidData_ReturnsJwtToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner", "Admin" };
        var permissions = new[] { "users.view", "users.create", "products.view" };

        // Act
        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        // Assert
        token.Should().NotBeNullOrEmpty();
        ValidateTokenStructure(token).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_ContainsCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner" };
        var permissions = new[] { "users.view", "users.create" };

        // Act
        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        // Assert
        var claims = DecodeToken(token);
        claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        claims.Should().ContainSingle(c => c.Type == "tenant_id" && c.Value == tenantId.ToString());
        claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Owner");
        claims.Should().Contain(c => c.Type == "permissions" && c.Value == "users.view");
        claims.Should().Contain(c => c.Type == "permissions" && c.Value == "users.create");
    }

    [Fact]
    public void GenerateToken_ContainsMultipleRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner", "Admin", "User" };
        var permissions = new[] { "users.view" };

        // Act
        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        // Assert
        var claims = DecodeToken(token);
        var roleClaims = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
        roleClaims.Should().HaveCount(3);
        roleClaims.Should().Contain("Owner");
        roleClaims.Should().Contain("Admin");
        roleClaims.Should().Contain("User");
    }

    [Fact]
    public void GenerateToken_ContainsMultiplePermissions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner" };
        var permissions = new[] { "users.view", "users.create", "products.view", "products.create", "sales.view" };

        // Act
        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        // Assert
        var claims = DecodeToken(token);
        var permissionClaims = claims.Where(c => c.Type == "permissions").Select(c => c.Value).ToArray();
        permissionClaims.Should().HaveCount(5);
        permissionClaims.Should().Contain(permissions);
    }

    [Fact]
    public void GenerateToken_ContainsCorrectIssuerAndAudience()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner" };
        var permissions = new[] { "users.view" };

        // Act
        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        // Assert
        var claims = DecodeToken(token);
        claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Iss && c.Value == _jwtSettings.Issuer);
        claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == _jwtSettings.Audience);
    }

    [Fact]
    public void GenerateToken_HasCorrectExpiry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner" };
        var permissions = new[] { "users.view" };

        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        var afterGeneration = DateTime.UtcNow;

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiry = beforeGeneration.AddMinutes(_jwtSettings.ExpiryMinutes);

        // Allow 1 second tolerance for test execution time
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GenerateToken_WithEmptyRolesAndPermissions_CreatesValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = Array.Empty<string>();
        var permissions = Array.Empty<string>();

        // Act
        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var claims = DecodeToken(token);
        claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
        claims.Should().NotContain(c => c.Type == "permissions");
    }

    [Fact]
    public void GenerateToken_IsDifferentForDifferentUsers()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner" };
        var permissions = new[] { "users.view" };

        // Act
        var token1 = _tokenGenerator.GenerateToken(userId1, email, tenantId, roles, permissions);
        var token2 = _tokenGenerator.GenerateToken(userId2, email, tenantId, roles, permissions);

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GenerateToken_CanBeValidated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Owner" };
        var permissions = new[] { "users.view" };

        var token = _tokenGenerator.GenerateToken(userId, email, tenantId, roles, permissions);

        // Act
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();

        // Assert
        var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
        principal.Should().NotBeNull();
        validatedToken.Should().NotBeNull();
    }

    private static bool ValidateTokenStructure(string token)
    {
        var parts = token.Split('.');
        return parts.Length == 3; // Header.Payload.Signature
    }

    private static IEnumerable<Claim> DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims;
    }
}
