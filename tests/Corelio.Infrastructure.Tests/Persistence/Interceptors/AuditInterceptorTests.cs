using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;
using Corelio.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Corelio.Infrastructure.Tests.Persistence.Interceptors;

/// <summary>
/// Unit tests for AuditInterceptor.
/// </summary>
public class AuditInterceptorTests
{
    private readonly Mock<ICurrentUserProvider> _currentUserProviderMock;
    private readonly Guid _currentUserId = Guid.NewGuid();

    public AuditInterceptorTests()
    {
        _currentUserProviderMock = new Mock<ICurrentUserProvider>();
    }

    [Fact]
    public async Task SavingChangesAsync_WhenAddingAuditableEntity_SetsCreatedAtAndCreatedBy()
    {
        // Arrange
        _currentUserProviderMock.Setup(x => x.UserId).Returns(_currentUserId);

        var interceptor = new AuditInterceptor(_currentUserProviderMock.Object);
        var beforeTest = DateTime.UtcNow;

        await using var context = CreateTestContext(interceptor);
        var entity = new TestAuditableEntity { Name = "Test" };

        // Act
        context.AuditableEntities.Add(entity);
        await context.SaveChangesAsync();

        var afterTest = DateTime.UtcNow;

        // Assert
        entity.CreatedAt.Should().BeOnOrAfter(beforeTest);
        entity.CreatedAt.Should().BeOnOrBefore(afterTest);
        entity.CreatedBy.Should().Be(_currentUserId);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenModifyingAuditableEntity_SetsUpdatedAtAndUpdatedBy()
    {
        // Arrange
        _currentUserProviderMock.Setup(x => x.UserId).Returns(_currentUserId);

        var interceptor = new AuditInterceptor(_currentUserProviderMock.Object);

        await using var context = CreateTestContext(interceptor);
        var entity = new TestAuditableEntity { Name = "Test" };

        context.AuditableEntities.Add(entity);
        await context.SaveChangesAsync();

        var originalCreatedAt = entity.CreatedAt;
        var originalCreatedBy = entity.CreatedBy;

        // Wait a moment to ensure time difference
        await Task.Delay(10);

        // Act - Modify the entity
        var beforeUpdate = DateTime.UtcNow;
        entity.Name = "Modified";
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
        var afterUpdate = DateTime.UtcNow;

        // Assert
        entity.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
        entity.UpdatedAt.Should().BeOnOrBefore(afterUpdate);
        entity.UpdatedBy.Should().Be(_currentUserId);

        // CreatedAt and CreatedBy should remain unchanged
        entity.CreatedAt.Should().Be(originalCreatedAt);
        entity.CreatedBy.Should().Be(originalCreatedBy);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenNoCurrentUser_SetsNullForCreatedBy()
    {
        // Arrange
        _currentUserProviderMock.Setup(x => x.UserId).Returns((Guid?)null);

        var interceptor = new AuditInterceptor(_currentUserProviderMock.Object);

        await using var context = CreateTestContext(interceptor);
        var entity = new TestAuditableEntity { Name = "Test" };

        // Act
        context.AuditableEntities.Add(entity);
        await context.SaveChangesAsync();

        // Assert
        entity.CreatedBy.Should().BeNull();
    }

    [Fact]
    public async Task SavingChangesAsync_WhenAddingBaseEntity_OnlySetsCreatedAt()
    {
        // Arrange
        _currentUserProviderMock.Setup(x => x.UserId).Returns(_currentUserId);

        var interceptor = new AuditInterceptor(_currentUserProviderMock.Object);
        var beforeTest = DateTime.UtcNow;

        await using var context = CreateTestContext(interceptor);
        var entity = new TestBaseEntity { Name = "Test" };

        // Act
        context.BaseEntities.Add(entity);
        await context.SaveChangesAsync();

        var afterTest = DateTime.UtcNow;

        // Assert
        entity.CreatedAt.Should().BeOnOrAfter(beforeTest);
        entity.CreatedAt.Should().BeOnOrBefore(afterTest);
    }

    [Fact]
    public void SavingChanges_WhenAddingAuditableEntity_SetsCreatedAtAndCreatedBy()
    {
        // Arrange
        _currentUserProviderMock.Setup(x => x.UserId).Returns(_currentUserId);

        var interceptor = new AuditInterceptor(_currentUserProviderMock.Object);
        var beforeTest = DateTime.UtcNow;

        using var context = CreateTestContext(interceptor);
        var entity = new TestAuditableEntity { Name = "Test" };

        // Act
        context.AuditableEntities.Add(entity);
        context.SaveChanges();

        var afterTest = DateTime.UtcNow;

        // Assert
        entity.CreatedAt.Should().BeOnOrAfter(beforeTest);
        entity.CreatedAt.Should().BeOnOrBefore(afterTest);
        entity.CreatedBy.Should().Be(_currentUserId);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenModifying_DoesNotChangeCreatedAtOrCreatedBy()
    {
        // Arrange
        var originalCreator = Guid.NewGuid();
        var newUser = Guid.NewGuid();

        _currentUserProviderMock.SetupSequence(x => x.UserId)
            .Returns(originalCreator)
            .Returns(newUser);

        var interceptor = new AuditInterceptor(_currentUserProviderMock.Object);

        await using var context = CreateTestContext(interceptor);
        var entity = new TestAuditableEntity { Name = "Test" };

        context.AuditableEntities.Add(entity);
        await context.SaveChangesAsync();

        var originalCreatedAt = entity.CreatedAt;

        // Wait to ensure time difference
        await Task.Delay(10);

        // Act - Try to modify CreatedAt and CreatedBy
        entity.Name = "Modified";
        entity.CreatedAt = DateTime.UtcNow.AddDays(-1); // Try to change
        entity.CreatedBy = Guid.NewGuid(); // Try to change
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();

        // Assert - Reload to verify database values
        context.Entry(entity).State = EntityState.Detached;
        var loadedEntity = await context.AuditableEntities.FirstAsync(e => e.Id == entity.Id);

        loadedEntity.CreatedAt.Should().Be(originalCreatedAt);
        loadedEntity.CreatedBy.Should().Be(originalCreator);
        loadedEntity.UpdatedBy.Should().Be(newUser);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenAddingMultipleEntities_SetsAuditFieldsForAll()
    {
        // Arrange
        _currentUserProviderMock.Setup(x => x.UserId).Returns(_currentUserId);

        var interceptor = new AuditInterceptor(_currentUserProviderMock.Object);
        var beforeTest = DateTime.UtcNow;

        await using var context = CreateTestContext(interceptor);
        var entities = new[]
        {
            new TestAuditableEntity { Name = "Test1" },
            new TestAuditableEntity { Name = "Test2" },
            new TestAuditableEntity { Name = "Test3" }
        };

        // Act
        context.AuditableEntities.AddRange(entities);
        await context.SaveChangesAsync();

        var afterTest = DateTime.UtcNow;

        // Assert
        entities.Should().AllSatisfy(e =>
        {
            e.CreatedAt.Should().BeOnOrAfter(beforeTest);
            e.CreatedAt.Should().BeOnOrBefore(afterTest);
            e.CreatedBy.Should().Be(_currentUserId);
        });
    }

    private static AuditTestDbContext CreateTestContext(AuditInterceptor interceptor)
    {
        var options = new DbContextOptionsBuilder<AuditTestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;

        return new AuditTestDbContext(options);
    }
}

/// <summary>
/// Test entity extending AuditableEntity for testing purposes.
/// </summary>
public class TestAuditableEntity : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Test entity extending BaseEntity for testing purposes.
/// </summary>
public class TestBaseEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Test DbContext for audit interceptor testing.
/// </summary>
public class AuditTestDbContext(DbContextOptions<AuditTestDbContext> options) : DbContext(options)
{
    public DbSet<TestAuditableEntity> AuditableEntities => Set<TestAuditableEntity>();
    public DbSet<TestBaseEntity> BaseEntities => Set<TestBaseEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestAuditableEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<TestBaseEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
    }
}
