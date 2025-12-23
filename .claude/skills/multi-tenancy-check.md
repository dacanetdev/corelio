---
name: multi-tenancy-check
description: Validates multi-tenant isolation and prevents data leakage (CRITICAL)
---

# Multi-Tenancy Security Check

## CRITICAL RULES (Zero Tolerance)

### ✅ ALWAYS Required

1. **All business entities MUST implement `ITenantEntity`**
   ```csharp
   public class Product : BaseAuditableEntity, ITenantEntity
   {
       public Guid TenantId { get; set; }
       public required string Name { get; set; }
   }
   ```

2. **ALWAYS get TenantId from ITenantService**
   ```csharp
   // ✅ CORRECT
   var tenantId = _tenantService.GetCurrentTenantId();

   // ❌ NEVER ACCEPT FROM CLIENT
   public record CreateProductCommand(string Name, Guid TenantId); // WRONG!
   ```

3. **Query filters MUST be applied in ApplicationDbContext**
   ```csharp
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       modelBuilder.Entity<Product>()
           .HasQueryFilter(p => p.TenantId == _tenantService.GetCurrentTenantId());
   }
   ```

4. **Save interceptors MUST auto-set TenantId**
   ```csharp
   public override InterceptionResult<int> SavingChanges(...)
   {
       foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
       {
           if (entry.State == EntityState.Added)
           {
               entry.Entity.TenantId = _tenantService.GetCurrentTenantId();
           }
       }
   }
   ```

### ❌ NEVER Allowed

1. **NEVER accept tenant_id from client input**
   - Not in command DTOs
   - Not in query parameters
   - Not in request bodies
   - Not in route parameters

2. **NEVER bypass query filters unless explicitly needed**
   ```csharp
   // ❌ DANGEROUS - Only for admin operations
   var allProducts = await _context.Products.IgnoreQueryFilters().ToListAsync();
   ```

3. **NEVER use tenant_id in WHERE clauses manually**
   ```csharp
   // ❌ WRONG - Query filter handles this
   var products = await _context.Products
       .Where(p => p.TenantId == tenantId)
       .ToListAsync();

   // ✅ CORRECT - Query filter auto-applies
   var products = await _context.Products.ToListAsync();
   ```

## Validation Checklist

### Entity Design
- [ ] All business entities implement `ITenantEntity`
- [ ] TenantId is of type `Guid` (not `int` or `string`)
- [ ] No public setters for TenantId (set by interceptor only)
- [ ] Lookup/reference data tables considered (tenant-specific vs global)

### Command/Query Design
- [ ] No `TenantId` parameter in command records
- [ ] No `TenantId` parameter in query records
- [ ] TenantId obtained from `ITenantService` in handlers
- [ ] Validation rules don't reference other tenants' data

### Database Configuration
- [ ] Query filters configured for all tenant entities
- [ ] Save interceptor sets TenantId on insert
- [ ] Foreign key relationships preserve tenant isolation
- [ ] Indexes include TenantId for performance

### API Endpoints
- [ ] No `tenant_id` in route parameters
- [ ] No `tenant_id` in query strings
- [ ] No `tenant_id` in request bodies
- [ ] Authorization checks tenant ownership

### Background Jobs
- [ ] Jobs that process tenant data use `IServiceScope`
- [ ] TenantId explicitly set in job context
- [ ] Jobs don't leak data between tenants
- [ ] Scheduled tasks iterate tenants correctly

### Reporting/Exports
- [ ] Reports scoped to current tenant
- [ ] Export files include tenant validation
- [ ] Cross-tenant aggregation prohibited (unless admin)
- [ ] File names don't expose tenant information

## Common Vulnerabilities

### 1. Insecure Direct Object Reference (IDOR)
```csharp
// ❌ VULNERABLE
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(Guid id)
{
    var product = await _context.Products.FindAsync(id);
    return Ok(product); // Could return another tenant's product!
}

// ✅ SECURE
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(Guid id)
{
    var product = await _context.Products.FindAsync(id);
    // Query filter ensures it's the current tenant's product
    if (product == null) return NotFound();
    return Ok(product);
}
```

### 2. Bulk Operations Without Tenant Filter
```csharp
// ❌ VULNERABLE
public async Task DeleteAllProducts()
{
    await _context.Products.ExecuteDeleteAsync(); // Deletes ALL tenants!
}

// ✅ SECURE
public async Task DeleteAllProducts()
{
    var tenantId = _tenantService.GetCurrentTenantId();
    await _context.Products
        .Where(p => p.TenantId == tenantId)
        .ExecuteDeleteAsync();
}
```

### 3. Join Queries Without Filters
```csharp
// ❌ VULNERABLE - Raw SQL bypasses filters
var result = await _context.Database.SqlQueryRaw<ProductDto>(
    "SELECT * FROM Products p JOIN Categories c ON p.CategoryId = c.Id"
).ToListAsync();

// ✅ SECURE - Use LINQ (applies filters)
var result = await _context.Products
    .Include(p => p.Category)
    .Select(p => new ProductDto { ... })
    .ToListAsync();
```

### 4. Global Lookup Data Confusion
```csharp
// ⚠️ DECISION REQUIRED
public class SatProductCode // SAT catalog - same for all tenants
{
    // Should this have TenantId? NO - it's global reference data
    public string Code { get; set; }
    public string Description { get; set; }
}

public class ProductCategory // Business-specific - per tenant
{
    public Guid TenantId { get; set; } // YES - this is tenant-specific
    public string Name { get; set; }
}
```

## Testing Requirements

### Unit Tests
```csharp
[Fact]
public async Task CreateProduct_ShouldSetTenantIdAutomatically()
{
    // Arrange
    var tenantId = Guid.NewGuid();
    _tenantService.GetCurrentTenantId().Returns(tenantId);

    // Act
    var command = new CreateProductCommand("Product A");
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Value.TenantId.Should().Be(tenantId);
}
```

### Integration Tests (MANDATORY)
```csharp
[Fact]
public async Task GetProducts_ShouldOnlyReturnCurrentTenantProducts()
{
    // Arrange - Create products for 2 different tenants
    var tenant1Id = Guid.NewGuid();
    var tenant2Id = Guid.NewGuid();

    await AddProductsForTenant(tenant1Id, 3);
    await AddProductsForTenant(tenant2Id, 2);

    // Act - Query as Tenant 1
    SetCurrentTenant(tenant1Id);
    var result = await _client.GetAsync("/api/products");
    var products = await result.Content.ReadFromJsonAsync<List<ProductDto>>();

    // Assert - Should only see Tenant 1's products
    products.Should().HaveCount(3);
    products.Should().AllSatisfy(p => p.TenantId.Should().Be(tenant1Id));
}

[Fact]
public async Task GetProduct_ShouldReturn404ForOtherTenantsProduct()
{
    // Arrange
    var tenant1Id = Guid.NewGuid();
    var tenant2Id = Guid.NewGuid();

    var productId = await CreateProductForTenant(tenant1Id);

    // Act - Try to access as Tenant 2
    SetCurrentTenant(tenant2Id);
    var result = await _client.GetAsync($"/api/products/{productId}");

    // Assert - Should not find the product
    result.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
```

## Code Review Focus Areas

When reviewing code, ALWAYS check:

1. **New entity classes** - Do they implement `ITenantEntity`?
2. **Command/Query DTOs** - Do they have `TenantId` parameters? (They shouldn't!)
3. **Raw SQL queries** - Do they bypass query filters?
4. **IgnoreQueryFilters()** calls - Are they justified and safe?
5. **Background jobs** - Do they properly scope tenant context?
6. **Bulk operations** - Do they include tenant filters?
7. **File uploads/downloads** - Do they validate tenant ownership?
8. **Cache keys** - Do they include tenant_id to prevent leakage?

## Performance Considerations

### Indexes Must Include TenantId
```csharp
modelBuilder.Entity<Product>()
    .HasIndex(p => new { p.TenantId, p.Sku })
    .IsUnique(); // Unique SKU per tenant

modelBuilder.Entity<Sale>()
    .HasIndex(s => new { s.TenantId, s.SaleDate })
    .IncludeProperties(s => new { s.TotalAmount }); // Covering index
```

### Partition Large Tables
```sql
-- Consider partitioning for multi-million row tables
CREATE TABLE sales (
    tenant_id UUID NOT NULL,
    sale_date DATE NOT NULL,
    ...
) PARTITION BY HASH (tenant_id);
```

## Emergency Response

### Suspected Data Leak
1. **IMMEDIATE**: Disable affected tenant's access
2. **AUDIT**: Review logs for unauthorized access
3. **INVESTIGATE**: Identify root cause
4. **NOTIFY**: Inform affected tenant (legal requirement)
5. **REMEDIATE**: Fix vulnerability
6. **VERIFY**: Run full isolation test suite
7. **DOCUMENT**: Post-incident report

### Audit Query
```sql
-- Find entities without TenantId (PostgreSQL)
SELECT table_name
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name NOT IN ('sat_product_codes', 'sat_tax_rates') -- Global tables
  AND table_name NOT LIKE '%_audit'
GROUP BY table_name
HAVING COUNT(CASE WHEN column_name = 'tenant_id' THEN 1 END) = 0;
```

## Success Metrics

- **Zero data leaks** between tenants (ever)
- **100% test coverage** for tenant isolation
- **All entities** implement `ITenantEntity` (except global lookups)
- **No client-provided** `tenant_id` parameters
- **Query filters applied** to all tenant entities
- **Integration tests pass** for cross-tenant isolation

---

**Remember**: Multi-tenancy violations are **CRITICAL SECURITY ISSUES**. When in doubt, consult the security team.
