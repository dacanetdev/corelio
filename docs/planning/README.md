# Corelio - Multi-Tenant SaaS ERP Implementation Plan

## Project Overview
**Corelio** is a multi-tenant SaaS ERP for Mexican SMEs (starting with hardware stores), providing unified POS, Inventory, and CFDI (Mexican tax) compliance.

## Technology Stack
- **Backend:** .NET 10 + C# 14
- **Frontend:** Blazor Server + MudBlazor
- **Orchestration:** .NET Aspire (latest version)
- **Database:** PostgreSQL 16 (via Aspire.Npgsql.EntityFrameworkCore.PostgreSQL)
- **Authentication:** Custom JWT with permission-based authorization
- **Caching:** Redis (via Aspire.StackExchange.Redis)
- **CFDI:** Custom implementation with PAC integration (Finkel/Divertia)
- **Observability:** Built-in Aspire dashboard with OpenTelemetry

## Architecture Approach
**Clean Architecture** with CQRS pattern, row-level multi-tenancy, and .NET Aspire orchestration

```
src/
├── Aspire/
│   ├── Corelio.AppHost/           # .NET Aspire orchestration (service discovery, config)
│   └── Corelio.ServiceDefaults/   # Shared Aspire configurations (telemetry, health checks)
├── Core/
│   ├── Corelio.Domain/            # Entities, value objects, enums
│   └── Corelio.Application/       # CQRS handlers, interfaces, DTOs
├── Infrastructure/
│   └── Corelio.Infrastructure/    # EF Core, external services, CFDI
└── Presentation/
    ├── Corelio.WebAPI/            # REST API
    └── Corelio.BlazorApp/         # Blazor Server UI
```

### .NET Aspire Benefits
- **Service Discovery:** Automatic service-to-service communication
- **Observability:** Built-in OpenTelemetry with metrics, traces, logs
- **Health Checks:** Automatic health monitoring for all services
- **Configuration:** Centralized configuration management
- **Local Development:** Aspire dashboard for debugging (http://localhost:15888)
- **Resilience:** Built-in retry policies and circuit breakers
- **Resource Management:** Automated PostgreSQL and Redis container orchestration

### C# 14 Features to Leverage
- **Primary Constructors:** Simplified entity constructors and dependency injection
- **Collection Expressions:** Cleaner collection initialization `[item1, item2]`
- **Inline Arrays:** Performance improvements for fixed-size arrays
- **Lambda Default Parameters:** Cleaner lambda expressions
- **Nameof Scope Extensions:** Better compile-time safety
- **ref readonly Parameters:** Performance optimization for large structs
- **Params Collections:** More flexible params with any collection type
- **Lock Object Type:** Better performance for lock statements
- **Interceptors:** Advanced compile-time metaprogramming

## Multi-Tenancy Strategy
**Row-Level Security** (single database, shared schema)

- **Tenant Resolution:** JWT claims → Header → Subdomain → Custom domain
- **Data Isolation:** EF Core global query filters + save interceptors
- **Tenant Context:** Scoped service with tenant ID from authenticated user
- **Performance:** Tenant data cached in Redis (30-min TTL)

## Critical Files to Create

### 0. .NET Aspire Orchestration Layer (NEW)
- `src/Aspire/Corelio.AppHost/Program.cs` - Aspire orchestration entry point
- `src/Aspire/Corelio.ServiceDefaults/Extensions.cs` - Shared service configuration
- `src/Aspire/Corelio.AppHost/appsettings.json` - Aspire configuration

### 1. Foundation Layer
- `src/Core/Corelio.Domain/Common/ITenantEntity.cs` - Interface for tenant-scoped entities
- `src/Core/Corelio.Domain/Common/BaseEntity.cs` - Base entity with ID, timestamps (using C# 14 features)
- `src/Core/Corelio.Domain/Common/AuditableEntity.cs` - Base with audit fields
- `src/Core/Corelio.Domain/Entities/Tenants/Tenant.cs` - Tenant entity
- `src/Core/Corelio.Domain/Entities/Identity/User.cs` - User entity
- `src/Core/Corelio.Domain/Entities/Identity/Role.cs` - Role entity with permissions

### 2. Infrastructure Layer
- `src/Infrastructure/Corelio.Infrastructure/Persistence/ApplicationDbContext.cs` - Main EF Core context
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Interceptors/TenantInterceptor.cs` - Enforce tenant on save
- `src/Infrastructure/Corelio.Infrastructure/MultiTenancy/TenantService.cs` - Tenant resolution
- `src/Infrastructure/Corelio.Infrastructure/MultiTenancy/TenantMiddleware.cs` - HTTP middleware
- `src/Infrastructure/Corelio.Infrastructure/Identity/JwtTokenGenerator.cs` - JWT generation
- `src/Infrastructure/Corelio.Infrastructure/Identity/IdentityService.cs` - Auth logic

### 3. API Layer
- `src/Presentation/Corelio.WebAPI/Program.cs` - Startup configuration
- `src/Presentation/Corelio.WebAPI/Controllers/AuthController.cs` - Login/register
- `src/Presentation/Corelio.WebAPI/Controllers/TenantsController.cs` - Tenant management

### 4. Blazor Layer
- `src/Presentation/Corelio.BlazorApp/Program.cs` - Blazor configuration
- `src/Presentation/Corelio.BlazorApp/Services/ApiClient.cs` - HTTP client wrapper
- `src/Presentation/Corelio.BlazorApp/Services/AuthService.cs` - Auth state management

## Database Schema Highlights

### Core Tables
- `tenants` - Tenant organizations with subdomain, RFC, subscription
- `tenant_configurations` - CFDI settings, POS settings, features
- `users` - Per-tenant users with email/password
- `roles` - System and custom roles
- `permissions` - Granular permissions (e.g., "sales.create")
- `role_permissions` - Many-to-many mapping

### Business Tables
- `products` - Product catalog with SAT codes, pricing, SKU/barcode
- `product_categories` - Hierarchical categories
- `inventory_items` - Per-warehouse stock levels with reserved quantity
- `inventory_transactions` - Stock movement history
- `warehouses` - Physical locations for inventory
- `customers` - Customer master with RFC/CURP for CFDI
- `customer_addresses` - Billing/shipping addresses
- `sales` - All transaction types (POS, invoice, quote, credit note)
- `sale_items` - Line items with tax calculation
- `payments` - Payment records with method/reference
- `cfdi_invoices` - CFDI 4.0 invoices with UUID, stamp data
- `cfdi_invoice_items` - Invoice line items with SAT codes

### Key Indexes
- Tenant isolation: `idx_products_tenant`, `idx_sales_tenant`, etc.
- Search: GIN indexes on name fields (Spanish full-text)
- Lookups: `idx_products_barcode`, `idx_customers_rfc`, `idx_cfdi_uuid`

## Authentication & Authorization

### JWT Token Structure
```json
{
  "sub": "user-uuid",
  "email": "user@example.com",
  "tenant_id": "tenant-uuid",
  "roles": ["Owner", "Cashier"],
  "permissions": ["sales.create", "products.read"],
  "exp": 1701446400
}
```

### Default Roles
- **Owner:** Full system access (all permissions)
- **Cashier:** POS operations only (sales.create, products.view, customers.view)
- **InventoryManager:** Product and inventory management
- **Accountant:** Financial reports and CFDI generation
- **Administrator:** User and configuration management
- **Seller:** Sales and customer management

### Permission Examples
- `products.view`, `products.create`, `products.edit`, `products.delete`, `products.pricing`
- `sales.create`, `sales.view`, `sales.cancel`, `sales.discount`
- `inventory.view`, `inventory.adjust`
- `cfdi.generate`, `cfdi.cancel`
- `customers.view`, `customers.create`
- `reports.view`, `reports.financial`

## POS System Design

### Speed Requirements
- Product search: < 0.5 seconds
- Add to cart: < 0.2 seconds
- Complete sale: < 3 seconds total

### Key Features
- Barcode scanner integration
- Keyboard shortcuts (F2=search, F12=pay, ESC=clear)
- Autocomplete product search
- Real-time inventory validation
- Multiple payment methods (cash, card, transfer, mixed)
- Thermal printer support (ESC/POS commands)
- Receipt generation (PDF + thermal)
- Quick customer creation
- Offline mode (future enhancement)

### Payment Methods
- Cash (with change calculation)
- Card (Visa, Mastercard, AmEx)
- Bank transfer
- Check
- Credit (customer account)
- Mixed payments

## CFDI Integration (Mexican Tax Compliance)

### CFDI 4.0 Requirements
- PAC provider integration (Finkel or Divertia)
- XML generation per SAT schema
- Digital signature with CSD certificate
- UUID stamping from SAT
- QR code generation
- PDF invoice with fiscal stamp
- Cancellation workflow
- Email delivery

### CFDI Workflow
1. Create sale in POS
2. Generate CFDI draft from sale
3. Validate customer RFC and fiscal data
4. Generate CFDI 4.0 XML
5. Send to PAC for stamping
6. Receive UUID and digital signatures
7. Store stamped XML
8. Generate PDF with QR code
9. Email to customer
10. Update sale with invoice reference

### SAT Catalogs Required
- Product/Service codes (ClaveProdServ)
- Unit codes (ClaveUnidad)
- Payment forms (FormaPago)
- Payment methods (MetodoPago)
- CFDI use codes (UsoCFDI)
- Tax regime codes (RegimenFiscal)

## API Structure

### Base URL
`https://api.corelio.com.mx/api/v1`

### Key Endpoints

**Authentication:**
- `POST /auth/login` - User login
- `POST /auth/refresh` - Refresh token
- `POST /auth/logout` - Logout

**Tenants:**
- `POST /tenants/register` - New tenant signup
- `GET /tenants/current` - Current tenant info
- `GET /tenants/current/config` - Tenant CFDI/POS configuration

**Products:**
- `GET /products` - List products (paginated, filtered by tenant)
- `POST /products` - Create product
- `GET /products/barcode/{code}` - Search by barcode
- `GET /products/search?q={query}` - Full-text search

**Inventory:**
- `GET /inventory` - Inventory levels
- `POST /inventory/adjust` - Adjust stock
- `GET /inventory/low-stock` - Low stock alerts

**Customers:**
- `GET /customers` - List customers
- `POST /customers` - Create customer
- `GET /customers/search?q={query}` - Search by name/RFC

**Sales:**
- `POST /sales` - Create POS sale
- `GET /sales` - Sales history
- `POST /sales/{id}/payments` - Add payment
- `GET /sales/{id}/receipt` - Download receipt PDF

**Quotes:**
- `POST /quotes` - Create quote
- `POST /quotes/{id}/convert` - Convert quote to sale

**CFDI:**
- `POST /invoices` - Generate invoice from sale
- `POST /invoices/{id}/stamp` - Stamp with PAC
- `POST /invoices/{id}/cancel` - Cancel invoice
- `GET /invoices/{id}/xml` - Download XML
- `GET /invoices/{id}/pdf` - Download PDF

### Response Format
```json
{
  "success": true,
  "data": { ... },
  "message": null
}
```

## Blazor UI Components (MudBlazor)

### Key Pages
- `/` - Dashboard (sales stats, alerts, recent activity)
- `/pos` - Point of Sale checkout
- `/products` - Product catalog management
- `/inventory` - Inventory dashboard
- `/customers` - Customer management
- `/sales` - Sales history and reports
- `/invoices` - CFDI invoice management
- `/settings` - Tenant configuration

### Critical Components
- `POSCheckout.razor` - Main POS interface with cart
- `ProductScanner.razor` - Barcode scanner input
- `PaymentPanel.razor` - Payment method selection
- `InvoiceForm.razor` - CFDI invoice generation
- `CustomerQuickCreate.razor` - Fast customer creation modal
- `ReceiptPreview.razor` - Receipt preview before print

## Implementation Phases

### Phase 1: Foundation & Aspire Setup (Weeks 1-2)
**Deliverables:**
- **.NET Aspire solution structure** with AppHost and ServiceDefaults
- **Aspire dashboard** running with PostgreSQL and Redis containers
- **Service discovery** configured between API and Blazor app
- Multi-tenancy infrastructure (resolver, filters, interceptors) **with C# 14 primary constructors**
- JWT authentication working
- Tenant registration and login API
- Base entities using **C# 14 features** (primary constructors, collection expressions)
- **OpenTelemetry** automatically configured via Aspire
- Logging and error handling with structured logs to Aspire dashboard

**C# 14 Usage Examples:**
```csharp
// Primary constructor for dependency injection
public class TenantService(IHttpContextAccessor httpContextAccessor,
                          IMemoryCache cache) : ITenantService
{
    public string GetCurrentTenantId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.FindFirst("tenant_id")?.Value
            ?? throw new TenantNotFoundException();
    }
}

// Collection expressions for role permissions
public static class DefaultRoles
{
    public static readonly Role Owner = new()
    {
        Name = "Owner",
        Permissions = ["*"] // Collection expression instead of new[] { "*" }
    };

    public static readonly Role Cashier = new()
    {
        Name = "Cashier",
        Permissions = [
            Permissions.SalesCreate,
            Permissions.SalesView,
            Permissions.ProductsView
        ] // Collection expression
    };
}

// Lock object type for better performance
private readonly Lock _inventoryLock = new();

public async Task AdjustInventory(Guid productId, decimal quantity)
{
    lock(_inventoryLock)
    {
        // Thread-safe inventory adjustment
    }
}
```

### Phase 2: Product & Inventory (Weeks 3-4)
**Deliverables:**
- Product CRUD with SAT codes
- Category management
- Product search (SKU, barcode, name)
- Inventory tracking per warehouse
- Stock adjustments
- Blazor product management UI

### Phase 3: Customer Management (Week 5)
**Deliverables:**
- Customer CRUD with RFC validation
- Address management
- CFDI preference settings
- Customer search
- Quick customer creation for POS

### Phase 4: POS System (Weeks 6-7)
**Deliverables:**
- Shopping cart state management
- Product search with autocomplete
- Barcode scanner integration
- Payment processing (all methods)
- Receipt generation (PDF + thermal)
- Keyboard shortcuts
- Fast checkout flow (< 5 seconds)

### Phase 5: Sales Management (Week 8)
**Deliverables:**
- Quote creation and conversion
- Credit note generation
- Sales history with filters
- Payment tracking
- Mixed payment support

### Phase 6: CFDI Integration (Weeks 9-10)
**Deliverables:**
- CFDI 4.0 XML generation
- PAC integration (Finkel/Divertia)
- Invoice stamping workflow
- Certificate management (CSD upload)
- PDF generation with QR code
- Invoice cancellation
- Email delivery
- SAT catalog integration

### Phase 7: Testing & Refinement (Week 11)
**Deliverables:**
- Unit tests (>70% coverage)
- Integration tests (API + DB)
- Multi-tenancy isolation tests
- Performance optimization
- Security audit
- UI/UX polish

### Phase 8: Deployment (Week 12)
**Deliverables:**
- Production infrastructure setup
- CI/CD pipeline (GitHub Actions)
- Monitoring (Sentry + Application Insights)
- Documentation (API + user manual)
- Demo tenant with sample data
- MVP launch

## NuGet Packages Required

### .NET Aspire (Latest)
- **Aspire.Hosting.AppHost** - Orchestration runtime
- **Aspire.Hosting.PostgreSQL** - PostgreSQL container orchestration
- **Aspire.Hosting.Redis** - Redis container orchestration
- **Aspire.Npgsql.EntityFrameworkCore.PostgreSQL** - EF Core + PostgreSQL with Aspire integration
- **Aspire.StackExchange.Redis** - Redis with Aspire integration
- **Aspire.Hosting.Azure.Storage** - Optional: Azure Blob Storage (for CFDI files)

### Core (.NET 10 compatible)
- MediatR 13.x (latest) - CQRS
- FluentValidation 12.x (latest) - Validation
- AutoMapper 13.x (latest) - Object mapping

### Database (.NET 10 compatible)
- Npgsql.EntityFrameworkCore.PostgreSQL 10.x (latest)
- Microsoft.EntityFrameworkCore.Design 10.x
- Dapper 2.1.x (latest) - Performance queries

### Authentication (.NET 10 compatible)
- Microsoft.AspNetCore.Authentication.JwtBearer 10.x
- System.IdentityModel.Tokens.Jwt 8.x (latest)

### API (.NET 10 compatible)
- Asp.Versioning.Mvc.ApiExplorer (replaces Microsoft.AspNetCore.Mvc.Versioning)
- Swashbuckle.AspNetCore 7.x (latest) - Swagger docs

### Blazor (.NET 10 compatible)
- MudBlazor 8.x (latest for .NET 10) - UI components
- Blazored.LocalStorage 5.x (latest)

### CFDI
- System.Security.Cryptography.Xml 10.x
- iTextSharp.LGPLv2.Core (latest) - PDF generation
- QRCoder 2.x (latest) - QR codes

### Email
- MailKit 5.x (latest)

### Background Jobs
- Hangfire.AspNetCore 1.8.x (latest)
- Hangfire.PostgreSql 1.20.x (latest)

### Testing (.NET 10 compatible)
- xUnit 2.9.x (latest)
- Moq 4.20.x (latest)
- FluentAssertions 7.x (latest)
- bunit 2.x (latest) - Blazor testing
- Microsoft.EntityFrameworkCore.InMemory 10.x
- Testcontainers.PostgreSql 4.x (latest)

### OpenTelemetry (Built into Aspire)
- Already included via Aspire.ServiceDefaults
- Automatic metrics, traces, and logs collection
- Dashboard at http://localhost:15888

## Third-Party Services

### PAC Provider (CFDI Stamping)
**Recommended:** Finkel (https://finkel.com.mx)
- Cost: ~$0.50-$1.00 MXN per invoice
- Reliable API
- Good documentation

**Alternative:** Divertia

### Email
**SendGrid** - 100 emails/day free tier

### Hosting (MVP)
**DigitalOcean:**
- $12/mo - API droplet (2GB RAM)
- $15/mo - Managed PostgreSQL
- $5/mo - Spaces (object storage)
- **Total: ~$32/month**

### CDN
**Cloudflare** - Free tier (SSL, DDoS protection)

### Monitoring
**Sentry** - 5,000 events/month free

## Development Environment Setup

### .NET Aspire Handles Everything!
With .NET Aspire, you no longer need manual Docker Compose files. Aspire automatically:
- Spins up PostgreSQL container
- Spins up Redis container
- Configures service discovery
- Sets up OpenTelemetry
- Provides dashboard at http://localhost:15888

### AppHost Program.cs Example
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL with persistent volume
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("corelioDb");

// Add Redis for caching
var redis = builder.AddRedis("redis");

// Add API service with dependencies
var apiService = builder.AddProject<Projects.Corelio_WebAPI>("api")
    .WithReference(postgres)
    .WithReference(redis);

// Add Blazor app with API reference
builder.AddProject<Projects.Corelio_BlazorApp>("webapp")
    .WithReference(apiService)
    .WithReference(redis);

builder.Build().Run();
```

### What You Get
- **Aspire Dashboard:** http://localhost:15888 (metrics, traces, logs, resources)
- **API:** Auto-assigned port with service discovery
- **Blazor App:** Auto-assigned port
- **PostgreSQL:** Automatically configured and connected
- **Redis:** Automatically configured and connected
- **Live Reload:** Built-in hot reload for all services

## Critical Success Factors

1. **Multi-Tenancy Isolation:** 100% data isolation - extensive testing required
2. **POS Performance:** Sub-5-second checkout consistently
3. **CFDI Compliance:** CFDI 4.0 compliant, validate with accountants
4. **Security:** HTTPS everywhere, encrypted credentials, security audits
5. **Backup:** Automated daily backups with point-in-time recovery

## Key Risks & Mitigation

| Risk | Mitigation |
|------|------------|
| PAC service downtime | Retry logic, queue system, backup PAC provider |
| Multi-tenant data leak | Extensive testing, query filter validation, security audits |
| Poor POS performance | Load testing, query optimization, Redis caching |
| CFDI regulation changes | Monitor SAT announcements, flexible architecture |

## First Steps

### Prerequisites
1. **Install .NET 10 SDK** (latest version)
2. **Install Visual Studio 2022 17.12+** or **Rider 2024.3+** (for .NET 10 and Aspire support)
3. **Install Docker Desktop** (required for Aspire to run containers)
4. **Install .NET Aspire workload:**
   ```bash
   dotnet workload update
   dotnet workload install aspire
   ```

### Initial Setup

1. **Initialize Git repository**
   ```bash
   git init
   git remote add origin <repository-url>
   ```

2. **Create .NET Aspire solution structure**
   ```bash
   # Create solution
   dotnet new sln -n Corelio

   # Create Aspire orchestration (IMPORTANT: Do this first!)
   dotnet new aspire-apphost -n Corelio.AppHost -o src/Aspire/Corelio.AppHost
   dotnet new aspire-servicedefaults -n Corelio.ServiceDefaults -o src/Aspire/Corelio.ServiceDefaults

   # Create domain projects
   dotnet new classlib -n Corelio.Domain -o src/Core/Corelio.Domain
   dotnet new classlib -n Corelio.Application -o src/Core/Corelio.Application

   # Create infrastructure
   dotnet new classlib -n Corelio.Infrastructure -o src/Infrastructure/Corelio.Infrastructure

   # Create API with Aspire integration
   dotnet new webapi -n Corelio.WebAPI -o src/Presentation/Corelio.WebAPI

   # Create Blazor Server with Aspire integration
   dotnet new blazor -n Corelio.BlazorApp -o src/Presentation/Corelio.BlazorApp

   # Add all to solution
   dotnet sln add src/Aspire/Corelio.AppHost/Corelio.AppHost.csproj
   dotnet sln add src/Aspire/Corelio.ServiceDefaults/Corelio.ServiceDefaults.csproj
   dotnet sln add src/Core/Corelio.Domain/Corelio.Domain.csproj
   dotnet sln add src/Core/Corelio.Application/Corelio.Application.csproj
   dotnet sln add src/Infrastructure/Corelio.Infrastructure/Corelio.Infrastructure.csproj
   dotnet sln add src/Presentation/Corelio.WebAPI/Corelio.WebAPI.csproj
   dotnet sln add src/Presentation/Corelio.BlazorApp/Corelio.BlazorApp.csproj

   # Add project references to AppHost
   cd src/Aspire/Corelio.AppHost
   dotnet add reference ../../Presentation/Corelio.WebAPI/Corelio.WebAPI.csproj
   dotnet add reference ../../Presentation/Corelio.BlazorApp/Corelio.BlazorApp.csproj
   cd ../../..

   # Add ServiceDefaults to API and Blazor
   cd src/Presentation/Corelio.WebAPI
   dotnet add reference ../../Aspire/Corelio.ServiceDefaults/Corelio.ServiceDefaults.csproj
   cd ../Corelio.BlazorApp
   dotnet add reference ../../Aspire/Corelio.ServiceDefaults/Corelio.ServiceDefaults.csproj
   cd ../../..
   ```

3. **Add Aspire packages to AppHost**
   ```bash
   cd src/Aspire/Corelio.AppHost
   dotnet add package Aspire.Hosting.AppHost
   dotnet add package Aspire.Hosting.PostgreSQL
   dotnet add package Aspire.Hosting.Redis
   cd ../../..
   ```

4. **Add Aspire integrations to WebAPI and Infrastructure**
   ```bash
   cd src/Infrastructure/Corelio.Infrastructure
   dotnet add package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
   dotnet add package Aspire.StackExchange.Redis
   cd ../../..
   ```

5. **Configure AppHost** (edit `src/Aspire/Corelio.AppHost/Program.cs`)

6. **Create base entities** using C# 14 features (BaseEntity, ITenantEntity, Tenant, User, Role)

7. **Implement ApplicationDbContext** with tenant filtering and Aspire integration

8. **Build authentication** (JWT service, login endpoint)

9. **Create first migration** and seed data

10. **Run Aspire AppHost** (this starts everything!)
    ```bash
    dotnet run --project src/Aspire/Corelio.AppHost
    ```

11. **Access Aspire Dashboard** at http://localhost:15888 to monitor all services

12. **Test multi-tenancy isolation** with multiple tenants

## Why .NET 10 + Aspire is Perfect for Corelio

### Performance Benefits (.NET 10)
- **40% faster** JSON serialization (critical for API responses)
- **Native AOT** ready (future optimization for startup time)
- **Improved garbage collection** for high-throughput POS scenarios
- **Better async/await** performance for database queries
- **SIMD improvements** for complex calculations (tax, discounts)

### Developer Experience (.NET 10 + C# 14)
- **Primary constructors** reduce boilerplate by 30-40%
- **Collection expressions** make code cleaner and more readable
- **Aspire dashboard** provides instant observability without configuration
- **Hot reload** across all services speeds up development
- **Better type inference** reduces explicit type declarations

### Production Benefits (Aspire)
- **Built-in telemetry** catches issues before users report them
- **Service mesh ready** for future scaling
- **Health checks** prevent cascading failures
- **Automatic retries** for transient database/Redis failures
- **Resource limits** prevent runaway containers

### Cost Savings
- **No additional monitoring tools** needed (Aspire dashboard + OpenTelemetry)
- **Faster development** = lower development costs
- **Better performance** = smaller hosting footprint
- **Fewer production issues** = less support overhead

## Summary: Modern Stack Advantages

| Traditional Stack | Corelio with .NET 10 + Aspire |
|-------------------|-------------------------------|
| Manual Docker Compose | Automatic container orchestration |
| Separate logging setup | Built-in structured logging |
| Third-party monitoring | Aspire dashboard included |
| Manual service discovery | Automatic via Aspire |
| Complex configuration | Centralized in AppHost |
| Slow local startup | Instant with Aspire |
| Manual health checks | Automatic monitoring |
| External telemetry costs | Free OpenTelemetry built-in |

## Next Actions

Once this plan is approved, I will:
1. **Create the .NET Aspire solution structure** with all projects
2. **Configure AppHost** with PostgreSQL, Redis, and service orchestration
3. **Set up ServiceDefaults** for shared telemetry and configuration
4. **Implement multi-tenancy foundation** using C# 14 primary constructors
5. **Create the database schema** with EF Core 10 migrations
6. **Build authentication system** with JWT
7. **Develop the first module (Products)** following CQRS pattern
8. **Implement Blazor UI** with MudBlazor 8.x
9. **Build the POS system** with optimized performance
10. **Integrate CFDI compliance** with PAC provider

**The MVP will be complete in approximately 12 weeks** with:
- ✅ Modern .NET 10 + C# 14 codebase
- ✅ Cloud-native architecture via Aspire
- ✅ Built-in observability and monitoring
- ✅ Production-ready multi-tenancy
- ✅ Full CFDI 4.0 compliance
- ✅ Fast, keyboard-driven POS system
- ✅ Foundation for e-commerce expansion