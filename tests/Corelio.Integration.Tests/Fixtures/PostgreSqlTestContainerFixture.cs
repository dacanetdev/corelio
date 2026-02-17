using Corelio.Infrastructure.Persistence;
using Corelio.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Corelio.Integration.Tests.Fixtures;

/// <summary>
/// Testcontainers fixture for PostgreSQL integration tests.
/// Starts a PostgreSQL 16 container and applies migrations once per test collection.
/// </summary>
public class PostgreSqlTestContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private string? _connectionString;

    /// <summary>
    /// Initializes the PostgreSQL container and applies migrations.
    /// Called once before all tests in the collection.
    /// </summary>
    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("corelio_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();

        // Apply migrations
        await using var dbContext = GetDbContext(Guid.Empty);
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Stops and disposes the PostgreSQL container.
    /// Called once after all tests in the collection.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }

    /// <summary>
    /// Creates a new ApplicationDbContext scoped to the specified tenant.
    /// Each call creates a new context instance with its own tenant provider.
    /// </summary>
    /// <param name="tenantId">The tenant ID for this context's query filters.</param>
    /// <returns>A new ApplicationDbContext instance.</returns>
    public ApplicationDbContext GetDbContext(Guid tenantId)
    {
        if (_connectionString == null)
        {
            throw new InvalidOperationException("Container not initialized. Call InitializeAsync first.");
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        var tenantProvider = new TenantProvider(tenantId);
        var tenantInterceptor = new TenantInterceptor(tenantProvider);
        var currentUserProvider = new CurrentUserProvider(); // No authenticated user in integration tests
        var auditInterceptor = new AuditInterceptor(currentUserProvider);

        return new ApplicationDbContext(options, tenantProvider, tenantInterceptor, auditInterceptor);
    }

    /// <summary>
    /// Gets the connection string to the PostgreSQL test container.
    /// </summary>
    public string ConnectionString => _connectionString ?? throw new InvalidOperationException("Container not initialized.");
}
