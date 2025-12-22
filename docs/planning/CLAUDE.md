# Claude AI Development Guide for Corelio

## Project Overview
**Corelio** is a multi-tenant SaaS ERP for Mexican SMEs (hardware stores) built with:
- .NET 10 + C# 14
- .NET Aspire (cloud-native orchestration)
- PostgreSQL 16 + EF Core 10
- Blazor Server + MudBlazor
- Clean Architecture + CQRS

---

## Tech Stack

### Framework & Language
- **Framework:** .NET 10 (C# 14)
- **Language Features:** Primary constructors, collection expressions, file-scoped namespaces
- **Orchestration:** .NET Aspire (cloud-native apps)

### Architecture & Patterns
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, WebAPI, BlazorApp)
- **Patterns:** CQRS with MediatR, Repository Pattern, Dependency Injection
- **Multi-Tenancy:** Database-level isolation with query filters

### Database & ORM
- **Database:** PostgreSQL 16
- **ORM:** Entity Framework Core 10
- **Migrations:** Code-first with EF Core migrations
- **Caching:** Redis (via Aspire)

### Testing
- **Framework:** xUnit v3
- **Assertions:** FluentAssertions
- **Mocking:** Moq
- **Integration Tests:** Testcontainers (PostgreSQL)
- **Coverage:** >70% target

### Frontend
- **UI Framework:** Blazor Server
- **Component Library:** MudBlazor
- **State Management:** Blazor built-in + Fluxor (if needed)

### Security & Compliance
- **Authentication:** JWT tokens with refresh tokens
- **Authorization:** Role-based access control (RBAC)
- **Certificates:** Azure Key Vault (CSD for CFDI)
- **CFDI:** 4.0 compliance with PAC integration

---

## Setup Standards

### Code Style
- Use **file-scoped namespaces** for all files
- Use **primary constructors** for dependency injection
- Use **collection expressions** `[]` instead of `new List<T>()`
- Follow **C# 14 best practices** consistently

### Secrets Management
- **Development:** Use `dotnet user-secrets` for local secrets
- **Production:** Use Azure Key Vault or environment variables
- **NEVER** use `.env` files or hardcoded keys
- **NEVER** commit secrets to version control

### Solution Structure
- Maintain strict `.sln` structure with proper project references
- Domain → Application → Infrastructure → Presentation (one-way dependencies)
- Each project has clear responsibility and boundaries

### Project Organization
```
src/
├── Corelio.Domain/              # Core business logic (zero dependencies)
├── Corelio.Application/         # Use cases, CQRS handlers
├── Corelio.Infrastructure/      # EF Core, external services
├── Corelio.WebAPI/              # REST API controllers
├── Corelio.BlazorApp/           # Blazor Server UI
└── Aspire/
    └── Corelio.AppHost/         # Aspire orchestration
tests/
├── Corelio.Domain.Tests/
├── Corelio.Application.Tests/
├── Corelio.Infrastructure.Tests/
└── Corelio.Integration.Tests/
```

---

## Commands

### Build & Run
```bash
# Build entire solution
dotnet build

# Run with Aspire (recommended)
dotnet run --project src/Aspire/Corelio.AppHost

# Run API only (without Aspire)
dotnet run --project src/Corelio.WebAPI

# Run Blazor app only
dotnet run --project src/Corelio.BlazorApp
```

### Testing
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Corelio.Application.Tests

# Run tests with filter
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add [MigrationName] --project src/Corelio.Infrastructure --startup-project src/Corelio.WebAPI

# Update database
dotnet ef database update --project src/Corelio.Infrastructure --startup-project src/Corelio.WebAPI

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/Corelio.Infrastructure --startup-project src/Corelio.WebAPI

# Generate SQL script
dotnet ef migrations script --project src/Corelio.Infrastructure --startup-project src/Corelio.WebAPI --output migration.sql
```

### User Secrets
```bash
# Initialize user secrets for a project
dotnet user-secrets init --project src/Corelio.WebAPI

# Set a secret
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=corelio;..." --project src/Corelio.WebAPI

# List all secrets
dotnet user-secrets list --project src/Corelio.WebAPI

# Remove a secret
dotnet user-secrets remove "SecretKey" --project src/Corelio.WebAPI

# Clear all secrets
dotnet user-secrets clear --project src/Corelio.WebAPI
```

### Package Management
```bash
# Add package to project
dotnet add src/Corelio.Application package MediatR

# Update all packages
dotnet restore

# List outdated packages
dotnet list package --outdated
```

### Code Quality
```bash
# Format code
dotnet format

# Run SonarQube scanner (requires SonarQube setup)
dotnet sonarscanner begin /k:"Corelio" /d:sonar.host.url="http://localhost:9000"
dotnet build
dotnet sonarscanner end
```

---

## Claude Code MCP Server Configuration

To enhance Claude Code's capabilities for this project, install the following MCP (Model Context Protocol) servers:

### Required MCP Servers

```bash
# Add the .NET SDK management tool
claude mcp add "dotnet-sdk" -- npx -y dotnet-mcp@latest

# Add the Microsoft Learn Docs for .NET 10 context
claude mcp add "ms-docs" -- npx -y @microsoft/mcp-server-learn-docs

# Add PostgreSQL support for schema management
claude mcp add "postgres" -- npx -y @modelcontextprotocol/server-postgres
```

### What These MCP Servers Provide

**dotnet-sdk:**
- Access to .NET SDK commands and tools
- Project scaffolding and management
- Package management assistance
- Build and test automation

**ms-docs:**
- Context-aware Microsoft Learn documentation
- .NET 10 API reference
- C# 14 language features documentation
- Aspire and EF Core 10 documentation
- Real-time access to latest Microsoft docs

**postgres:**
- PostgreSQL schema inspection and management
- Database query execution and analysis
- Migration assistance
- Performance optimization suggestions
- Connection string management

### Verify Installation

```bash
# List installed MCP servers
claude mcp list

# Test an MCP server
claude mcp test dotnet-sdk
claude mcp test ms-docs
claude mcp test postgres
```

### Usage Tips

- **dotnet-sdk:** Use for project setup, package management, and build tasks
- **ms-docs:** Reference when implementing .NET 10 features or Aspire patterns
- **postgres:** Use for database schema design, migration review, and query optimization

---

## Architecture Principles

### 1. Clean Architecture Layers
**Dependency Flow:** Presentation → Application → Domain ← Infrastructure

```
Corelio.Domain (Core)
  ↑ depends on
Corelio.Application (Use Cases)
  ↑ depends on
Corelio.Infrastructure (Implementation)
Corelio.WebAPI + Corelio.BlazorApp (UI)
```

**Rules:**
- Domain has ZERO dependencies
- Application depends only on Domain
- Infrastructure implements Application interfaces
- Presentation depends on Application (not Infrastructure directly)

### 2. Multi-Tenancy
**Critical:** Every entity that stores business data MUST implement `ITenantEntity`.

```csharp
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
```

**Security:**
- NEVER trust tenant ID from client input
- ALWAYS get tenant from `ITenantService.GetCurrentTenantId()`
- Query filters automatically apply `WHERE tenant_id = ?`
- Save interceptor automatically sets TenantId on INSERT

**Example:**
```csharp
// ❌ WRONG - Accepts tenant from client
public async Task<Product> GetProduct(Guid tenantId, Guid productId)

// ✅ CORRECT - Uses service
public async Task<Product> GetProduct(Guid productId)
{
    // Tenant filter applied automatically by EF Core
    return await _dbContext.Products.FindAsync(productId);
}
```

### 3. CQRS Pattern with MediatR

**Commands:** Modify state (Create, Update, Delete)
**Queries:** Read state (no side effects)

```csharp
// Command
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<Guid>>;

public class CreateProductCommandHandler(ApplicationDbContext dbContext)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            SalePrice = request.Price
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);

        return Result<Guid>.Success(product.Id);
    }
}
```

### 4. C# 14 Best Practices

**Use Primary Constructors:**
```csharp
// ✅ GOOD - Primary constructor
public class ProductService(ApplicationDbContext dbContext, ITenantService tenantService)
    : IProductService
{
    // Fields automatically created
}

// ❌ OLD WAY
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITenantService _tenantService;

    public ProductService(ApplicationDbContext dbContext, ITenantService tenantService)
    {
        _dbContext = dbContext;
        _tenantService = tenantService;
    }
}
```

**Use Collection Expressions:**
```csharp
// ✅ GOOD
var items = [item1, item2, item3];
var permissions = ["products.view", "products.create"];

// ❌ OLD WAY
var items = new[] { item1, item2, item3 };
var permissions = new List<string> { "products.view", "products.create" };
```

### 5. .NET Aspire Integration

**Service Configuration:**
- All configuration goes through Aspire AppHost
- Use service discovery (not hardcoded URLs)
- PostgreSQL and Redis managed by Aspire

**Example AppHost:**
```csharp
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("corelioDb");

var redis = builder.AddRedis("redis");

var apiService = builder.AddProject<Projects.Corelio_WebAPI>("api")
    .WithReference(postgres)
    .WithReference(redis);
```

### 6. CFDI (Mexican Tax) Compliance

**Certificate Security:**
- NEVER store CSD certificates in database
- ALWAYS use Azure Key Vault
- Certificate loaded in memory only during signing

**Workflow:**
1. Create sale → 2. Generate CFDI XML → 3. Load cert from Key Vault →
4. Sign XML → 5. Send to PAC → 6. Store UUID → 7. Dispose certificate

### 7. Database Migrations

**Creating Migrations:**
```bash
dotnet ef migrations add MigrationName --project src/Infrastructure/Corelio.Infrastructure
```

**Naming Convention:**
- Use descriptive names: `AddCustomerAddressTable`, `AddCFDIFields`
- Never delete migrations that have been deployed

**Seed Data:**
- System roles and permissions seeded in migration
- Tenant-specific data created via API

### 8. Testing Strategy

**Unit Tests:**
- Test Domain logic (entities, value objects)
- Test Application handlers (commands/queries)
- Mock external dependencies

**Integration Tests:**
- Use Testcontainers for PostgreSQL
- Test full request pipeline
- Verify multi-tenancy isolation

**Example Multi-Tenancy Test:**
```csharp
[Fact]
public async Task GetProducts_OnlyReturnsTenantProducts()
{
    // Arrange
    var tenant1 = Guid.NewGuid();
    var tenant2 = Guid.NewGuid();

    await SeedProducts(tenant1, count: 5);
    await SeedProducts(tenant2, count: 3);

    _tenantService.Setup(x => x.GetCurrentTenantId()).Returns(tenant1);

    // Act
    var products = await _dbContext.Products.ToListAsync();

    // Assert
    products.Should().HaveCount(5);
    products.Should().OnlyContain(p => p.TenantId == tenant1);
}
```

### 9. API Development

**Controller Guidelines:**
- Keep controllers thin (delegate to MediatR)
- Use attribute routing
- Return `Result<T>` pattern
- Always validate input with FluentValidation

**Example:**
```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand(request.Name, request.Price);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }
}
```

### 10. Blazor UI Development

**Component Structure:**
```
Pages/
  Products/
    ProductList.razor      # List view
    ProductDetail.razor    # Detail view
    ProductForm.razor      # Create/Edit form
Components/
  Shared/
    DataGrid.razor         # Reusable components
```

**State Management:**
- Use Blazor's built-in state container
- API calls via HttpClient wrapper
- SignalR for real-time updates (future)

### 11. Performance Optimization

**Caching:**
- Cache tenant configuration (Redis, 30 min TTL)
- Cache SAT catalogs (rarely change)
- Use EF Core compiled queries for hot paths

**Database:**
- Use pagination for large result sets
- Include proper indexes (see schema docs)
- Use AsNoTracking() for read-only queries

### 12. Security Checklist

Before merging:
- [ ] No hardcoded secrets (use Key Vault or user secrets)
- [ ] All business entities implement ITenantEntity
- [ ] No SQL injection vulnerabilities
- [ ] Input validation with FluentValidation
- [ ] Authorization checks on all endpoints
- [ ] Sensitive data encrypted (passwords, certificates)

### 13. Common Pitfalls to Avoid

1. **Bypassing Tenant Filter**
   - ❌ Using `IgnoreQueryFilters()`
   - ✅ Use `ITenantService.BypassTenantFilter()` with extreme caution

2. **Accepting Tenant from Client**
   - ❌ `public async Task Create(Guid tenantId, CreateRequest request)`
   - ✅ Get tenant from `ITenantService`

3. **Forgetting Concurrency Tokens**
   - Add `[Timestamp]` attribute to critical entities
   - Handle `DbUpdateConcurrencyException`

4. **Not Using Primary Constructors**
   - Use C# 14 features consistently

5. **Hardcoding Configuration**
   - Use IConfiguration or Aspire service discovery

### 14. Code Review Checklist

- [ ] Follows Clean Architecture layers
- [ ] Multi-tenancy properly enforced
- [ ] C# 14 features used (primary constructors, collection expressions)
- [ ] CQRS pattern followed (commands vs queries)
- [ ] Proper error handling with Result<T> pattern
- [ ] Input validation implemented
- [ ] Unit tests added for business logic
- [ ] Integration tests for multi-tenancy isolation
- [ ] No security vulnerabilities (use SonarQube)
- [ ] Performance considerations (caching, indexes)

### 15. Development Workflow

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/product-management
   ```

2. **Implement Domain Layer First**
   - Create entities, value objects, enums
   - Add domain events if needed

3. **Add Application Layer**
   - Create commands/queries
   - Implement handlers
   - Add validators

4. **Update Infrastructure**
   - Add EF Core configurations
   - Create migrations

5. **Build API Endpoints**
   - Add controllers
   - Implement DTOs

6. **Create Blazor UI**
   - Build pages/components
   - Wire up API calls

7. **Write Tests**
   - Unit tests for handlers
   - Integration tests for isolation

8. **Run Aspire AppHost**
   ```bash
   dotnet run --project src/Aspire/Corelio.AppHost
   ```

9. **Test in Aspire Dashboard**
   - Check metrics, traces, logs at http://localhost:15888

10. **Create Pull Request**
    - Code review required
    - All tests must pass
    - SonarQube quality gate passed

### 16. Troubleshooting

**Multi-Tenancy Not Working:**
- Check `OnModelCreating` has query filters
- Verify `TenantInterceptor` is registered
- Ensure `TenantMiddleware` is in pipeline
- Check JWT contains `tenant_id` claim

**Aspire Not Starting:**
- Ensure Docker Desktop is running
- Check port conflicts (15888, PostgreSQL, Redis)
- Verify .NET 10 SDK installed
- Run `dotnet workload update`

**Database Migrations Failing:**
- Check connection string in AppHost
- Ensure PostgreSQL container is running
- Verify migration syntax (PostgreSQL-specific)

### 17. Resources

- **Architecture:** See `docs/planning/00-architecture-specification.md`
- **Database:** See `docs/planning/01-database-schema-design.md`
- **API:** See `docs/planning/02-api-specification.md`
- **Multi-Tenancy:** See `docs/planning/03-multi-tenancy-implementation-guide.md`
- **CFDI:** See `docs/planning/04-cfdi-integration-specification.md`

**External Links:**
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [MudBlazor Documentation](https://mudblazor.com/)
- [SAT CFDI Portal](https://www.sat.gob.mx/consulta/71875/comprobante-fiscal-digital-por-internet-(cfdi))

---
**Last Updated:** 2025-12-21
**Maintainer:** Development Team
