using Corelio.Domain.Common;
using FluentAssertions;

namespace Corelio.Domain.Tests.Common;

public class AuditableEntityTests
{
    private class TestAuditableEntity : AuditableEntity { }

    [Fact]
    public void Constructor_ShouldSetUpdatedAtToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = new TestAuditableEntity();

        // Assert
        entity.UpdatedAt.Should().BeAfter(beforeCreation);
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Constructor_ShouldSetCreatedByToNull()
    {
        // Act
        var entity = new TestAuditableEntity();

        // Assert
        entity.CreatedBy.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldSetUpdatedByToNull()
    {
        // Act
        var entity = new TestAuditableEntity();

        // Assert
        entity.UpdatedBy.Should().BeNull();
    }

    [Fact]
    public void CreatedAtAndUpdatedAt_ShouldBeEqual_WhenNewlyCreated()
    {
        // Act
        var entity = new TestAuditableEntity();

        // Assert
        entity.CreatedAt.Should().BeCloseTo(entity.UpdatedAt, TimeSpan.FromMilliseconds(100));
    }
}
