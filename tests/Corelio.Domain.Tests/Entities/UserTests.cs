using Corelio.Domain.Entities;
using FluentAssertions;

namespace Corelio.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var user = new User();

        // Assert
        user.Id.Should().NotBeEmpty();
        user.IsActive.Should().BeTrue();
        user.IsEmailConfirmed.Should().BeFalse();
        user.TwoFactorEnabled.Should().BeFalse();
        user.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public void FullName_ShouldCombineFirstAndLastName()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Juan",
            LastName = "García"
        };

        // Assert
        user.FullName.Should().Be("Juan García");
    }

    [Fact]
    public void FullName_ShouldTrimWhitespace()
    {
        // Arrange
        var user = new User
        {
            FirstName = "María",
            LastName = ""
        };

        // Assert
        user.FullName.Should().Be("María");
    }

    [Fact]
    public void IsLocked_ShouldReturnTrue_WhenLockedUntilIsInFuture()
    {
        // Arrange
        var user = new User
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(30)
        };

        // Assert
        user.IsLocked.Should().BeTrue();
    }

    [Fact]
    public void IsLocked_ShouldReturnFalse_WhenLockedUntilIsInPast()
    {
        // Arrange
        var user = new User
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(-30)
        };

        // Assert
        user.IsLocked.Should().BeFalse();
    }

    [Fact]
    public void IsLocked_ShouldReturnFalse_WhenLockedUntilIsNull()
    {
        // Arrange
        var user = new User
        {
            LockedUntil = null
        };

        // Assert
        user.IsLocked.Should().BeFalse();
    }

    [Fact]
    public void UserRoles_ShouldBeEmptyByDefault()
    {
        // Act
        var user = new User();

        // Assert
        user.UserRoles.Should().NotBeNull();
        user.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void RefreshTokens_ShouldBeEmptyByDefault()
    {
        // Act
        var user = new User();

        // Assert
        user.RefreshTokens.Should().NotBeNull();
        user.RefreshTokens.Should().BeEmpty();
    }
}
