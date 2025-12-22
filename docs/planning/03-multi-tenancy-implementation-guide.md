# Corelio - Multi-Tenancy Implementation Guide

## Document Information
- **Project:** Corelio Multi-Tenant SaaS ERP
- **Strategy:** Row-Level Security (Shared Database, Shared Schema)
- **Framework:** .NET 10 + EF Core 10
- **Date:** 2025-12-20

---

## Table of Contents
1. [Multi-Tenancy Overview](#multi-tenancy-overview)
2. [Implementation Strategy](#implementation-strategy)
3. [Step-by-Step Implementation](#step-by-step-implementation)
4. [Security Considerations](#security-considerations)
5. [Testing Multi-Tenant Isolation](#testing-multi-tenant-isolation)
6. [Performance Optimization](#performance-optimization)

---

## Multi-Tenancy Overview

### What is Multi-Tenancy?
A single instance of the application serves multiple tenants (customers), with complete data isolation between them.

### Why Row-Level Security?
**Pros:**
- Lower infrastructure costs (single database)
- Easier maintenance and upgrades
- Efficient resource utilization
- Simpler backup/restore
- Cost-effective for SME SaaS

**Cons:**
- Requires careful implementation
- Shared resource contention
- Risk of data leakage if implemented incorrectly

**Alternative Approaches:**
- **Schema-per-tenant:** More isolation but complex management
- **Database-per-tenant:** Maximum isolation but expensive at scale

### Decision: Row-Level Security
For Corelio, we chose **row-level security** because:
1. Target market (SMEs) doesn't require extreme isolation
2. Cost-effectiveness is critical for market competitiveness
3. Easier horizontal scaling with connection pooling
4. PostgreSQL RLS + EF Core provides sufficient security

---

## Implementation Strategy

### Components of Multi-Tenancy

```
┌─────────────────────────────────────────────────────────┐
│                    HTTP Request                         │
└────────────────────┬────────────────────────────────────┘
                     │
          ┌──────────▼──────────┐
          │ TenantMiddleware    │ 1. Extract tenant context
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │ TenantResolver      │ 2. Resolve tenant ID
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │ TenantService       │ 3. Store in scoped service
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │ DbContext           │ 4. Apply query filters
          │ (Query Filters)     │
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │ SaveInterceptor     │ 5. Enforce on writes
          └──────────┬──────────┘
                     │
          ┌──────────▼──────────┐
          │ PostgreSQL          │ 6. Execute filtered query
          └─────────────────────┘
```

### Tenant Resolution Priority
1. **JWT Claims** (authenticated requests) - Most secure
2. **HTTP Header** (`X-Tenant-Id`) - API clients
3. **Subdomain** (e.g., `ferreteria-lopez.corelio.com.mx`)
4. **Custom Domain** (e.g., `ferreteria-lopez.com`)

---

## Step-by-Step Implementation

### Step 1: Create Core Interfaces

**File:** `src/Core/Corelio.Domain/Common/ITenantEntity.cs`

```csharp
namespace Corelio.Domain.Common;

/// <summary>
/// Interface for all entities that belong to a tenant
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// Tenant ID - automatically set by save interceptor
    /// </summary>
    Guid TenantId { get; set; }
}
```

**File:** `src/Core/Corelio.Application/Common/Interfaces/ITenantService.cs`

```csharp
namespace Corelio.Application.Common.Interfaces;

public interface ITenantService
{
    /// <summary>
    /// Get the current tenant ID from the request context
    /// </summary>
    /// <returns>Tenant ID</returns>
    /// <exception cref="TenantNotFoundException">No tenant context found</exception>
    Guid GetCurrentTenantId();

    /// <summary>
    /// Get the current tenant entity with configuration
    /// </summary>
    Task<Tenant?> GetCurrentTenantAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if current request has a valid tenant context
    /// </summary>
    bool HasTenantContext();

    /// <summary>
    /// Bypass tenant filtering for system operations (use with extreme caution!)
    /// </summary>
    IDisposable BypassTenantFilter();
}
```

---

### Step 2: Implement Tenant Service

**File:** `src/Infrastructure/Corelio.Infrastructure/MultiTenancy/TenantService.cs`

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Entities.Tenants;
using Corelio.Infrastructure.Persistence;

namespace Corelio.Infrastructure.MultiTenancy;

/// <summary>
/// C# 14 Primary Constructor for DI
/// </summary>
public class TenantService(
    IHttpContextAccessor httpContextAccessor,
    IMemoryCache cache,
    ApplicationDbContext dbContext) : ITenantService
{
    private Guid? _currentTenantId;
    private bool _bypassFilter;

    public Guid GetCurrentTenantId()
    {
        // Return cached value if already resolved
        if (_currentTenantId.HasValue)
            return _currentTenantId.Value;

        // Priority 1: Check JWT claims
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = user.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantClaim, out var tenantId))
            {
                _currentTenantId = tenantId;
                return tenantId;
            }
        }

        // Priority 2: Check HTTP header
        if (httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue) == true)
        {
            if (Guid.TryParse(headerValue.ToString(), out var tenantId))
            {
                _currentTenantId = tenantId;
                return tenantId;
            }
        }

        throw new TenantNotFoundException("No tenant context found in request");
    }

    public async Task<Tenant?> GetCurrentTenantAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = GetCurrentTenantId();

        // Cache tenant for 30 minutes
        var cacheKey = $"tenant:{tenantId}";

        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            var tenant = await dbContext.Tenants
                .Include(t => t.Configuration)
                .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

            if (tenant == null)
                throw new TenantNotFoundException($"Tenant {tenantId} not found");

            if (!tenant.IsActive)
                throw new TenantInactiveException($"Tenant {tenantId} is inactive");

            return tenant;
        });
    }

    public bool HasTenantContext()
    {
        try
        {
            GetCurrentTenantId();
            return true;
        }
        catch (TenantNotFoundException)
        {
            return false;
        }
    }

    public IDisposable BypassTenantFilter()
    {
        _bypassFilter = true;
        return new DisposableAction(() => _bypassFilter = false);
    }

    internal bool IsTenantFilterBypassed() => _bypassFilter;
}

/// <summary>
/// Helper class for cleanup actions
/// </summary>
internal class DisposableAction(Action action) : IDisposable
{
    public void Dispose() => action();
}
```

---

### Step 3: Configure EF Core Query Filters

**File:** `src/Infrastructure/Corelio.Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Corelio.Domain.Common;
using Corelio.Domain.Entities.Products;
using Corelio.Domain.Entities.Customers;
using Corelio.Infrastructure.MultiTenancy;

namespace Corelio.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ITenantService tenantService) : DbContext(options)
{
    // DbSets
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    // ... other DbSets

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply global query filter to all ITenantEntity types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if entity implements ITenantEntity
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Create lambda: entity => entity.TenantId == tenantService.GetCurrentTenantId()
                // CRITICAL: Must call GetCurrentTenantId() at QUERY TIME, not configuration time
                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var tenantIdProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));

                // Create expression to call tenantService.GetCurrentTenantId() at query execution
                var tenantServiceExpression = Expression.Constant(tenantService);
                var getTenantIdMethod = typeof(ITenantService).GetMethod(nameof(ITenantService.GetCurrentTenantId))!;
                var currentTenantId = Expression.Call(tenantServiceExpression, getTenantIdMethod);

                var equalExpression = Expression.Equal(tenantIdProperty, currentTenantId);
                var lambda = Expression.Lambda(equalExpression, parameter);

                // Apply filter
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        // Tenant table has NO filter (it's global)
        modelBuilder.Entity<Tenant>().HasQueryFilter(null);
    }
}
```

---

### Step 4: Create Save Interceptor

**File:** `src/Infrastructure/Corelio.Infrastructure/Persistence/Interceptors/TenantInterceptor.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Corelio.Domain.Common;
using Corelio.Application.Common.Interfaces;

namespace Corelio.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Automatically sets TenantId on entity creation and prevents cross-tenant updates
/// </summary>
public class TenantInterceptor(ITenantService tenantService) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return result;

        var tenantId = tenantService.GetCurrentTenantId();

        foreach (var entry in eventData.Context.ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Automatically set tenant ID on new entities
                    entry.Entity.TenantId = tenantId;
                    break;

                case EntityState.Modified:
                case EntityState.Deleted:
                    // Prevent cross-tenant modification/deletion
                    if (entry.Entity.TenantId != tenantId)
                    {
                        throw new TenantSecurityException(
                            $"Attempt to modify entity from different tenant. " +
                            $"Entity tenant: {entry.Entity.TenantId}, Current tenant: {tenantId}");
                    }
                    break;
            }
        }

        return result;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // Same logic as synchronous version
        return SavingChanges(eventData, result);
    }
}
```

---

### Step 5: Register Services

**File:** `src/Infrastructure/Corelio.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Corelio.Application.Common.Interfaces;
using Corelio.Infrastructure.MultiTenancy;
using Corelio.Infrastructure.Persistence;
using Corelio.Infrastructure.Persistence.Interceptors;

namespace Corelio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Multi-Tenancy (Scoped - per request)
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<TenantInterceptor>();

        // Database
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var tenantInterceptor = serviceProvider.GetRequiredService<TenantInterceptor>();

            options.UseNpgsql(connectionString)
                   .AddInterceptors(tenantInterceptor); // Register interceptor
        });

        return services;
    }
}
```

---

### Step 6: Tenant Resolution Middleware

**File:** `src/Infrastructure/Corelio.Infrastructure/MultiTenancy/TenantMiddleware.cs`

```csharp
using Microsoft.AspNetCore.Http;
using Corelio.Application.Common.Interfaces;

namespace Corelio.Infrastructure.MultiTenancy;

/// <summary>
/// Middleware to resolve and validate tenant early in the request pipeline
/// </summary>
public class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // Skip tenant resolution for public endpoints
        if (IsPublicEndpoint(context.Request.Path))
        {
            await next(context);
            return;
        }

        try
        {
            // Force tenant resolution early
            var tenantId = tenantService.GetCurrentTenantId();

            // Validate tenant is active
            var tenant = await tenantService.GetCurrentTenantAsync();

            // Add tenant info to response headers (for debugging)
            context.Response.Headers.Append("X-Tenant-Id", tenantId.ToString());

            await next(context);
        }
        catch (TenantNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "TENANT_NOT_FOUND",
                message = ex.Message
            });
        }
        catch (TenantInactiveException ex)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "TENANT_INACTIVE",
                message = ex.Message
            });
        }
    }

    private static bool IsPublicEndpoint(PathString path)
    {
        // Public endpoints that don't require tenant context
        return path.StartsWithSegments("/api/v1/auth/login")
            || path.StartsWithSegments("/api/v1/tenants/register")
            || path.StartsWithSegments("/health")
            || path.StartsWithSegments("/swagger");
    }
}
```

---

## Security Considerations

### 1. JWT Token Security
```csharp
// JWT should include tenant_id claim
var claims = new[]
{
    new Claim("sub", user.Id.ToString()),
    new Claim("email", user.Email),
    new Claim("tenant_id", user.TenantId.ToString()), // CRITICAL
    new Claim("roles", JsonSerializer.Serialize(user.Roles))
};
```

### 2. Prevent Tenant ID Tampering
```csharp
// WRONG: Never trust tenant ID from client input
public async Task<Product> GetProduct(Guid tenantId, Guid productId) // ❌

// CORRECT: Always use tenant service
public async Task<Product> GetProduct(Guid productId) // ✅
{
    var tenantId = _tenantService.GetCurrentTenantId();
    return await _dbContext.Products.FindAsync(productId);
    // Query filter automatically applies tenant restriction
}
```

### 3. Cross-Tenant Reference Prevention
```csharp
// Prevent foreign key references across tenants
public class Product : ITenantEntity
{
    public Guid CategoryId { get; set; }

    // Ensure category belongs to same tenant
    public void SetCategory(Category category)
    {
        if (category.TenantId != this.TenantId)
            throw new TenantSecurityException("Cannot reference category from different tenant");

        CategoryId = category.Id;
    }
}
```

### 4. System Operations (Bypass Filter)
```csharp
// Only for system operations like migration or batch jobs
public async Task MigrateAllTenants()
{
    using (_tenantService.BypassTenantFilter())
    {
        var allTenants = await _dbContext.Tenants.ToListAsync();
        // Process all tenants
    }
}
```

---

## Testing Multi-Tenant Isolation

### Unit Test Example

```csharp
[Fact]
public async Task GetProducts_OnlyReturnsTenantProducts()
{
    // Arrange
    var tenant1 = new Guid("11111111-1111-1111-1111-111111111111");
    var tenant2 = new Guid("22222222-2222-2222-2222-222222222222");

    await _dbContext.Products.AddRangeAsync([
        new Product { Id = Guid.NewGuid(), TenantId = tenant1, Name = "Product Tenant 1" },
        new Product { Id = Guid.NewGuid(), TenantId = tenant2, Name = "Product Tenant 2" }
    ]);
    await _dbContext.SaveChangesAsync();

    // Set current tenant
    _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(tenant1);

    // Act
    var products = await _dbContext.Products.ToListAsync();

    // Assert
    products.Should().HaveCount(1);
    products.First().TenantId.Should().Be(tenant1);
}
```

---

## Performance Optimization

### 1. Tenant Caching
```csharp
// Cache tenant configuration for 30 minutes
var tenant = await cache.GetOrCreateAsync($"tenant:{tenantId}", async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
    return await _dbContext.Tenants.Include(t => t.Configuration)
                                    .FirstOrDefaultAsync(t => t.Id == tenantId);
});
```

### 2. Index Optimization
```sql
-- Always include tenant_id as the first column in composite indexes
CREATE INDEX idx_products_tenant_category ON products(tenant_id, category_id);
CREATE INDEX idx_sales_tenant_date ON sales(tenant_id, created_at DESC);
```

### 3. Query Performance
```csharp
// Good: Filter applied early
var products = await _dbContext.Products
    .Where(p => p.CategoryId == categoryId) // tenant_id filter auto-applied
    .ToListAsync();

// Bad: Loading all tenants' data first
var allProducts = await _dbContext.Products.ToListAsync(); // ❌ Disaster!
```