using Corelio.Domain.Entities;
using FluentAssertions;

namespace Corelio.Domain.Tests.Entities;

public class RefreshTokenTests
{
    [Fact]
    public void IsValid_ShouldReturnTrue_WhenNotRevokedAndNotExpired()
    {
        // Arrange
        var token = new RefreshToken
        {
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Assert
        token.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenRevoked()
    {
        // Arrange
        var token = new RefreshToken
        {
            IsRevoked = true,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Assert
        token.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenExpired()
    {
        // Arrange
        var token = new RefreshToken
        {
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        // Assert
        token.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var token = new RefreshToken();

        // Assert
        token.Id.Should().NotBeEmpty();
        token.IsRevoked.Should().BeFalse();
        token.UseCount.Should().Be(0);
    }
}
