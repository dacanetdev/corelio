using Corelio.Domain.Common.Interfaces;
using Corelio.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Corelio.Infrastructure.Tests.Persistence.Interceptors;

/// <summary>
/// Unit tests for TenantInterceptor.
/// </summary>
[Trait("Category", "Unit")]
public class TenantInterceptorTests
{
    private readonly Mock<ITenantProvider> _tenantProviderMock;
    private readonly Guid _tenantId = Guid.NewGuid();

    public TenantInterceptorTests()
    {
        _tenantProviderMock = new Mock<ITenantProvider>();
    }

    [Fact]
    public async Task SavingChangesAsync_WhenAddingEntity_SetsTenantId()
    {
        // Arrange
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(true);
        _tenantProviderMock.Setup(x => x.TenantId).Returns(_tenantId);

        var interceptor = new TenantInterceptor(_tenantProviderMock.Object);

        await using var context = CreateTestContext(interceptor);
        var entity = new TestTenantEntity { Name = "Test" };

        // Act
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        // Assert
        entity.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenNoTenantContext_DoesNotSetTenantId()
    {
        // Arrange
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(false);

        var interceptor = new TenantInterceptor(_tenantProviderMock.Object);

        await using var context = CreateTestContext(interceptor);
        var entity = new TestTenantEntity { Name = "Test", TenantId = Guid.Empty };

        // Act
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        // Assert
        entity.TenantId.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenModifyingEntity_MarksTenantIdAsUnmodified()
    {
        // Arrange
        var originalTenantId = Guid.NewGuid();
        var newTenantId = Guid.NewGuid();
        var tenantIdIsModifiedAfterInterceptor = true;

        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(true);
        _tenantProviderMock.Setup(x => x.TenantId).Returns(newTenantId);

        // Create a capturing interceptor that records IsModified state after TenantInterceptor runs
        var capturingInterceptor = new CapturingInterceptor(
            _tenantProviderMock.Object,
            (context) =>
            {
                var entry = context.ChangeTracker.Entries<ITenantEntity>()
                    .FirstOrDefault(e => e.State == EntityState.Modified);
                if (entry is not null)
                {
                    tenantIdIsModifiedAfterInterceptor = entry.Property(nameof(ITenantEntity.TenantId)).IsModified;
                }
            });

        await using var context = new TestDbContext(
            new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .AddInterceptors(capturingInterceptor)
                .Options);

        var entity = new TestTenantEntity { Name = "Test", TenantId = originalTenantId };
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        // Act - Try to modify the entity including TenantId
        entity.TenantId = newTenantId;
        entity.Name = "Modified";
        context.Entry(entity).State = EntityState.Modified;

        await context.SaveChangesAsync();

        // Assert - TenantId should have been marked as not modified by the interceptor
        tenantIdIsModifiedAfterInterceptor.Should().BeFalse("TenantInterceptor should mark TenantId as not modified");
    }

    [Fact]
    public void SavingChanges_WhenAddingEntity_SetsTenantId()
    {
        // Arrange
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(true);
        _tenantProviderMock.Setup(x => x.TenantId).Returns(_tenantId);

        var interceptor = new TenantInterceptor(_tenantProviderMock.Object);

        using var context = CreateTestContext(interceptor);
        var entity = new TestTenantEntity { Name = "Test" };

        // Act
        context.TestEntities.Add(entity);
        context.SaveChanges();

        // Assert
        entity.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenAddingMultipleEntities_SetsAllTenantIds()
    {
        // Arrange
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(true);
        _tenantProviderMock.Setup(x => x.TenantId).Returns(_tenantId);

        var interceptor = new TenantInterceptor(_tenantProviderMock.Object);

        await using var context = CreateTestContext(interceptor);
        var entities = new[]
        {
            new TestTenantEntity { Name = "Test1" },
            new TestTenantEntity { Name = "Test2" },
            new TestTenantEntity { Name = "Test3" }
        };

        // Act
        context.TestEntities.AddRange(entities);
        await context.SaveChangesAsync();

        // Assert
        entities.Should().AllSatisfy(e => e.TenantId.Should().Be(_tenantId));
    }

    private static TestDbContext CreateTestContext(TenantInterceptor interceptor)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;

        return new TestDbContext(options);
    }
}

/// <summary>
/// Test entity implementing ITenantEntity for testing purposes.
/// </summary>
public class TestTenantEntity : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
}

/// <summary>
/// Test DbContext for interceptor testing.
/// </summary>
public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestTenantEntity> TestEntities => Set<TestTenantEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestTenantEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
    }
}

/// <summary>
/// Interceptor that extends TenantInterceptor to capture state after processing.
/// </summary>
public class CapturingInterceptor(ITenantProvider tenantProvider, Action<DbContext> captureAction)
    : TenantInterceptor(tenantProvider)
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var baseResult = base.SavingChangesAsync(eventData, result, cancellationToken);
        if (eventData.Context is not null)
        {
            captureAction(eventData.Context);
        }
        return baseResult;
    }
}
