using Corelio.Domain.Entities;
using FluentAssertions;

namespace Corelio.Domain.Tests.Entities;

public class UserRoleTests
{
    [Fact]
    public void Constructor_ShouldSetAssignedAtToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var userRole = new UserRole();

        // Assert
        userRole.AssignedAt.Should().BeAfter(beforeCreation);
        userRole.AssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void IsExpired_ShouldReturnTrue_WhenExpiresAtIsInPast()
    {
        // Arrange
        var userRole = new UserRole
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        // Assert
        userRole.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_ShouldReturnFalse_WhenExpiresAtIsInFuture()
    {
        // Arrange
        var userRole = new UserRole
        {
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        // Assert
        userRole.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ShouldReturnFalse_WhenExpiresAtIsNull()
    {
        // Arrange
        var userRole = new UserRole
        {
            ExpiresAt = null
        };

        // Assert
        userRole.IsExpired.Should().BeFalse();
    }
}
