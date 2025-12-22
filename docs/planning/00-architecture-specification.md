# Corelio - System Architecture Specification

## Document Information
- **Project:** Corelio Multi-Tenant SaaS ERP
- **Target:** Mexican SMEs (Hardware Stores)
- **Technology:** .NET 10 + C# 14 + .NET Aspire
- **Date:** 2025-12-20
- **Version:** 1.0

---

## Executive Summary

Corelio is a cloud-native, multi-tenant SaaS ERP system designed specifically for Mexican SMEs, starting with hardware stores (ferreterías). It provides a unified platform for Point of Sale (POS), inventory management, customer relationship management, and CFDI (Mexican tax) compliance.

### Key Differentiators
1. **Truly Unified:** Single source of truth for all business operations
2. **México-Specific:** Built-in CFDI 4.0 compliance
3. **Cloud-Native:** Modern .NET Aspire architecture
4. **Multi-Tenant:** Efficient resource sharing with complete data isolation
5. **Fast POS:** Sub-5-second checkout optimized for high-volume retail

---

## System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                      Internet (HTTPS)                           │
└────────────────────┬───────────────┬────────────────────────────┘
                     │               │
              ┌──────▼──────┐  ┌────▼─────────┐
              │   Blazor    │  │   REST API   │
              │ Server App  │  │  (Mobile/3rd)│
              └──────┬──────┘  └────┬─────────┘
                     │               │
         ┌───────────▼───────────────▼────────────┐
         │     .NET Aspire App Host               │
         │  (Service Discovery + Orchestration)   │
         └───┬─────────┬─────────┬─────────┬──────┘
             │         │         │         │
      ┌──────▼──┐  ┌──▼──┐  ┌───▼───┐  ┌─▼─────┐
      │   API   │  │Redis│  │ PgSQL │  │ Blob  │
      │ Service │  │Cache│  │  DB   │  │Storage│
      └────┬────┘  └─────┘  └───────┘  └───────┘
           │
    ┌──────▼─────────────┐
    │  Multi-Tenancy     │
    │  Infrastructure    │
    │  - Tenant Resolver │
    │  - Query Filters   │
    │  - Auth/AuthZ      │
    └─────┬──────────────┘
          │
    ┌─────▼──────────────────────────┐
    │    Business Logic Layers       │
    │  ┌──────────────────────────┐  │
    │  │   Application (CQRS)     │  │
    │  │   - Commands/Queries     │  │
    │  │   - Validation           │  │
    │  │   - Business Rules       │  │
    │  └────────┬─────────────────┘  │
    │  ┌────────▼─────────────────┐  │
    │  │   Domain (Core Logic)    │  │
    │  │   - Entities             │  │
    │  │   - Value Objects        │  │
    │  │   - Domain Events        │  │
    │  └──────────────────────────┘  │
    └────────────────────────────────┘
```

### Project Structure

```
Corelio/
├── src/
│   ├── Aspire/
│   │   ├── Corelio.AppHost/              # .NET Aspire orchestration
│   │   └── Corelio.ServiceDefaults/      # Shared Aspire config
│   ├── Core/
│   │   ├── Corelio.Domain/               # Domain entities & logic
│   │   └── Corelio.Application/          # CQRS handlers
│   ├── Infrastructure/
│   │   └── Corelio.Infrastructure/       # EF Core, external services
│   └── Presentation/
│       ├── Corelio.WebAPI/               # REST API
│       └── Corelio.BlazorApp/            # Blazor Server UI
├── tests/
│   ├── Corelio.Domain.Tests/
│   ├── Corelio.Application.Tests/
│   ├── Corelio.Infrastructure.Tests/
│   └── Corelio.Integration.Tests/
└── docs/
    ├── 00-architecture-specification.md   # This file
    ├── 01-database-schema-design.md
    ├── 02-api-specification.md
    ├── 03-multi-tenancy-implementation-guide.md
    └── 04-cfdi-integration-specification.md
```

---

## Technology Stack

### Core Framework
- **.NET 10** - Latest framework with performance improvements
- **C# 14** - Modern language features (primary constructors, collection expressions)
- **.NET Aspire** - Cloud-native orchestration and observability

### Frontend
- **Blazor Server** - Real-time UI with SignalR
- **MudBlazor 8.x** - Material Design component library

### Backend
- **ASP.NET Core 10** - Web API
- **MediatR 13.x** - CQRS pattern
- **FluentValidation 12.x** - Input validation
- **AutoMapper 13.x** - Object mapping

### Database
- **PostgreSQL 16** - Primary data store
- **EF Core 10** - ORM with migrations
- **Dapper** - Performance-critical queries

### Caching
- **Redis 7** - Distributed cache (via Aspire)

### Authentication
- **Custom JWT** - Lightweight auth with tenant claims

### CFDI (Mexican Tax)
- **Custom XML Generation** - CFDI 4.0 compliant
- **Finkel/Divertia PAC** - Stamping providers
- **X.509 Certificates** - CSD digital signatures

### Observability
- **OpenTelemetry** - Metrics, traces, logs (via Aspire)
- **Aspire Dashboard** - Real-time monitoring
- **Serilog** - Structured logging

### Testing
- **xUnit 2.9.x** - Unit tests
- **Moq 4.20.x** - Mocking
- **FluentAssertions 7.x** - Assertions
- **bUnit 2.x** - Blazor component tests

---

## Architectural Patterns

### 1. Clean Architecture

```
┌────────────────────────────────────────────┐
│          Presentation Layer                │
│  - Controllers (API)                       │
│  - Razor Components (Blazor)               │
└──────────────┬─────────────────────────────┘
               │
┌──────────────▼─────────────────────────────┐
│        Application Layer (CQRS)            │
│  - Commands (write operations)             │
│  - Queries (read operations)               │
│  - Validators                              │
│  - DTOs                                    │
└──────────────┬─────────────────────────────┘
               │
┌──────────────▼─────────────────────────────┐
│           Domain Layer                     │
│  - Entities (Product, Sale, Customer)      │
│  - Value Objects (Money, RFC)              │
│  - Domain Events                           │
│  - Business Rules                          │
└────────────────────────────────────────────┘
               ▲
               │
┌──────────────┴─────────────────────────────┐
│        Infrastructure Layer                │
│  - EF Core (Data Access)                   │
│  - External Services (PAC, Email)          │
│  - Multi-Tenancy (Filters, Interceptors)   │
└────────────────────────────────────────────┘
```

**Benefits:**
- **Testability:** Domain logic isolated from infrastructure
- **Maintainability:** Clear separation of concerns
- **Flexibility:** Easy to swap implementations

### 2. CQRS (Command Query Responsibility Segregation)

**Commands (Write):**
```csharp
// CreateProduct command
public record CreateProductCommand(
    string Sku,
    string Name,
    decimal SalePrice
) : IRequest<Result<Guid>>;

// Handler
public class CreateProductHandler(ApplicationDbContext db)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Sku = request.Sku,
            Name = request.Name,
            SalePrice = request.SalePrice
        };

        await db.Products.AddAsync(product, ct);
        await db.SaveChangesAsync(ct);

        return Result.Success(product.Id);
    }
}
```

**Queries (Read):**
```csharp
// GetProducts query
public record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null
) : IRequest<PaginatedList<ProductDto>>;

// Handler (using Dapper for performance)
public class GetProductsHandler(IDbConnection db, ITenantService tenant)
    : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var tenantId = tenant.GetCurrentTenantId();

        var sql = """
            SELECT id, sku, name, sale_price
            FROM products
            WHERE tenant_id = @TenantId
              AND (@Search IS NULL OR name ILIKE @Search)
            ORDER BY name
            LIMIT @PageSize OFFSET @Offset
            """;

        var products = await db.QueryAsync<ProductDto>(sql, new
        {
            TenantId = tenantId,
            Search = request.Search != null ? $"%{request.Search}%" : null,
            PageSize = request.PageSize,
            Offset = (request.Page - 1) * request.PageSize
        });

        return new PaginatedList<ProductDto>(products.ToList(), request.Page, request.PageSize);
    }
}
```

### 3. Multi-Tenancy (Row-Level Security)

**Key Components:**
1. **ITenantEntity** interface on all business entities
2. **TenantService** resolves current tenant from JWT
3. **Query Filters** automatically applied by EF Core
4. **Save Interceptor** enforces tenant on writes
5. **Middleware** validates tenant early in pipeline

**Example:**
```csharp
// Automatic tenant filtering
var products = await _db.Products.ToListAsync();
// SQL: SELECT * FROM products WHERE tenant_id = '<current-tenant>'

// Automatic tenant assignment
var product = new Product { Name = "Martillo" };
await _db.Products.AddAsync(product);
await _db.SaveChangesAsync();
// Interceptor automatically sets product.TenantId
```

**See:** `03-multi-tenancy-implementation-guide.md` for full details

---

## Security Architecture

### 1. Authentication Flow

```
┌──────────┐
│  Login   │
│ Request  │
└────┬─────┘
     │
     ▼
┌─────────────────────┐
│ Validate Credentials│
│ - Email/Password    │
│ - Check tenant      │
└────┬────────────────┘
     │
     ▼
┌─────────────────────┐
│  Generate JWT       │
│  Claims:            │
│  - user_id          │
│  - tenant_id ✓      │
│  - roles            │
│  - permissions      │
└────┬────────────────┘
     │
     ▼
┌─────────────────────┐
│ Return Token +      │
│ User Info           │
└─────────────────────┘
```

### 2. Authorization (Permission-Based)

**Permission Hierarchy:**
```
Owner (*)
├── Administrator
│   ├── users.view
│   ├── users.create
│   ├── users.edit
│   ├── settings.edit
│   └── ...
├── Cashier
│   ├── sales.create
│   ├── sales.view
│   ├── products.view
│   └── customers.view
├── InventoryManager
│   ├── products.create
│   ├── products.edit
│   ├── inventory.adjust
│   └── ...
└── Accountant
    ├── cfdi.generate
    ├── cfdi.cancel
    ├── reports.view
    └── reports.financial
```

**Usage:**
```csharp
[HttpPost]
[RequirePermission("sales.create")]
public async Task<IActionResult> CreateSale(CreateSaleCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

### 3. Data Protection

**Encryption:**
- **In Transit:** TLS 1.3
- **At Rest:** Database encryption (PostgreSQL pgcrypto)
- **Sensitive Fields:** CSD passwords, API keys encrypted

**Secrets Management:**
- **Development:** User Secrets / appsettings.Development.json
- **Production:** Azure Key Vault or environment variables

---

## Performance Strategy

### 1. Database Optimization

**Indexing Strategy:**
```sql
-- Tenant isolation queries
CREATE INDEX idx_products_tenant ON products(tenant_id)
WHERE is_deleted = false;

-- Composite indexes (tenant first!)
CREATE INDEX idx_sales_tenant_date ON sales(tenant_id, created_at DESC);

-- Full-text search
CREATE INDEX idx_products_name_search
ON products USING gin(to_tsvector('spanish', name));

-- Partial indexes for active records
CREATE INDEX idx_products_active
ON products(is_active) WHERE is_active = true AND is_deleted = false;
```

**Query Optimization:**
- Use Dapper for read-heavy operations
- Implement caching for frequently accessed data
- Avoid N+1 queries with `.Include()` / `.ThenInclude()`

### 2. Caching Strategy

**Redis Caching:**
```csharp
// Tenant configuration (30 min TTL)
var config = await _cache.GetOrCreateAsync($"tenant:{tenantId}:config", async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
    return await _db.TenantConfigurations.FindAsync(tenantId);
});

// Product catalog (5 min TTL, invalidate on update)
var products = await _cache.GetOrCreateAsync($"products:{tenantId}:all", async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    return await _db.Products.Where(p => p.TenantId == tenantId).ToListAsync();
});
```

### 3. POS Performance Targets

| Operation | Target | Strategy |
|-----------|--------|----------|
| Product search | < 0.5s | Indexed search + caching |
| Add to cart | < 0.2s | In-memory state |
| Complete sale | < 3s | Optimized write + async inventory |
| Receipt generation | < 1s | Background job |
| CFDI stamping | < 5s | Async with webhook |

---

## Scalability Strategy

### Horizontal Scaling

```
                    ┌───────────────┐
                    │ Load Balancer │
                    └───────┬───────┘
                            │
          ┌─────────────────┼─────────────────┐
          │                 │                 │
    ┌─────▼─────┐     ┌─────▼─────┐     ┌─────▼─────┐
    │ API       │     │ API       │     │ API       │
    │ Instance 1│     │ Instance 2│     │ Instance 3│
    └─────┬─────┘     └─────┬─────┘     └─────┬─────┘
          │                 │                 │
          └─────────────────┼─────────────────┘
                            │
                ┌───────────┴───────────┐
                │                       │
          ┌─────▼──────┐         ┌──────▼─────┐
          │ PostgreSQL │         │   Redis    │
          │ (Primary)  │◄────────┤  (Cluster) │
          └─────┬──────┘         └────────────┘
                │
          ┌─────▼──────┐
          │ PostgreSQL │
          │ (Replica)  │
          └────────────┘
```

### Capacity Planning

**Per Tenant Estimates:**
- **Storage:** ~1.5 GB/year (5K sales/month)
- **Database Connections:** 2-5 concurrent
- **API Requests:** 100-500/hour peak

**System Capacity (Single Server):**
- **Tenants:** 100-500 (depending on activity)
- **Concurrent Users:** 500-1000
- **API Throughput:** 10K requests/minute

---

## Deployment Architecture

### Development Environment

```
┌─────────────────────────────────────┐
│  Developer Machine                  │
│                                     │
│  ┌───────────────────────────────┐ │
│  │   .NET Aspire App Host        │ │
│  │   (Single Command: F5)        │ │
│  └───┬───────────────────────────┘ │
│      │ Automatically Starts:       │
│      ├─ API Service                │
│      ├─ Blazor App                 │
│      ├─ PostgreSQL (Docker)        │
│      ├─ Redis (Docker)             │
│      └─ Aspire Dashboard           │
│                                     │
│  http://localhost:15888             │
│  (Aspire Dashboard)                 │
└─────────────────────────────────────┘
```

### Production Environment (Azure)

```
┌────────────────────────────────────────────┐
│  Azure Cloud                               │
│  ┌──────────────────────────────────────┐ │
│  │  Azure App Service                   │ │
│  │  - API + Blazor                      │ │
│  │  - Auto-scaling                      │ │
│  └──────────────┬───────────────────────┘ │
│                 │                          │
│  ┌──────────────┼───────────────────────┐ │
│  │              │                       │ │
│  │  ┌───────────▼──────────┐  ┌────────▼──────┐
│  │  │ Azure Database for   │  │ Azure Cache   │
│  │  │ PostgreSQL           │  │ for Redis     │
│  │  └──────────────────────┘  └───────────────┘
│  │                                       │ │
│  │  ┌────────────────────────────────┐  │ │
│  │  │ Azure Blob Storage             │  │ │
│  │  │ - CFDI XML/PDF files           │  │ │
│  │  └────────────────────────────────┘  │ │
│  │                                       │ │
│  │  ┌────────────────────────────────┐  │ │
│  │  │ Application Insights           │  │ │
│  │  │ - Monitoring & Logging         │  │ │
│  │  └────────────────────────────────┘  │ │
│  └───────────────────────────────────────┘ │
└────────────────────────────────────────────┘
```

---

## Disaster Recovery & Backup

### Backup Strategy

**Database:**
- **Full Backup:** Daily at 2 AM (local time)
- **Incremental:** Every 6 hours
- **Retention:** 30 days
- **Off-site:** Azure Blob Storage (geo-redundant)

**CFDI Files:**
- **Immediate replication** to secondary region
- **Retention:** 5 years (legal requirement in México)

**Point-in-Time Recovery:**
- **RPO:** 15 minutes (via WAL archiving)
- **RTO:** < 4 hours

### High Availability

**Target SLA:** 99.5% uptime

| Component | HA Strategy |
|-----------|-------------|
| API | Multi-instance with load balancer |
| Database | Primary + read replica |
| Redis | Redis Cluster (3 nodes) |
| Files | Azure Blob (geo-redundant) |

---

## Compliance & Legal

### CFDI Requirements (México)
- **CFDI 4.0** mandatory since Jan 1, 2022
- **XML retention:** 5 years
- **Digital signature:** CSD certificate
- **PAC provider:** Authorized by SAT

### Data Protection
- **GDPR-like compliance** for Mexican data protection laws
- **Data residency:** All data stored in México (optional)
- **Tenant data isolation:** 100% guaranteed

### Financial Compliance
- **Audit trail:** All financial transactions logged
- **Immutable records:** No deletion of completed sales/invoices
- **Tax reporting:** CFDI export for accountants

---

## Monitoring & Observability

### .NET Aspire Dashboard

**Real-time monitoring at http://localhost:15888 (dev) / production URL**

**Features:**
- Service health status
- Resource utilization (CPU, memory, disk)
- Request traces (distributed tracing)
- Logs aggregation
- Metrics (custom + system)

### Key Metrics

**Business Metrics:**
- Sales per hour
- Average transaction value
- Top-selling products
- Low stock alerts
- CFDI stamping success rate

**Technical Metrics:**
- API response time (p50, p95, p99)
- Database query performance
- Cache hit rate
- Error rate
- Tenant request distribution

**Alerts:**
- CFDI stamping failures
- Database connection pool exhaustion
- API error rate > 1%
- Inventory below minimum threshold
- Certificate expiration (30 days warning)

---

## Development Workflow

### CI/CD Pipeline

```
┌──────────────┐
│ Git Push     │
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│ GitHub Actions   │
├──────────────────┤
│ 1. Build         │
│ 2. Unit Tests    │
│ 3. Integration   │
│ 4. Code Quality  │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│ Staging Deploy   │
│ - Smoke tests    │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│ Manual Approval  │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│ Production       │
│ - Blue/Green     │
│ - Rollback ready │
└──────────────────┘
```

---

## Related Documentation

1. **Database Schema Design** (`01-database-schema-design.md`)
   - Complete PostgreSQL schema
   - All tables, indexes, constraints
   - Performance optimization

2. **API Specification** (`02-api-specification.md`)
   - REST API endpoints
   - Request/response formats
   - Error handling
   - Rate limiting

3. **Multi-Tenancy Guide** (`03-multi-tenancy-implementation-guide.md`)
   - Step-by-step implementation
   - Security best practices
   - Testing strategies

4. **CFDI Integration** (`04-cfdi-integration-specification.md`)
   - CFDI 4.0 requirements
   - XML generation
   - PAC integration
   - Testing procedures

---

## Conclusion

Corelio's architecture is designed for:
- **Scalability:** Horizontal scaling to support thousands of tenants
- **Performance:** Sub-5-second POS transactions
- **Security:** Complete multi-tenant data isolation
- **Compliance:** Full CFDI 4.0 support
- **Maintainability:** Clean architecture with clear separation
- **Observability:** Built-in monitoring via .NET Aspire

The use of .NET 10, C# 14, and .NET Aspire positions Corelio as a modern, cloud-native solution ready for the future.