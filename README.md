# Corelio - Multi-Tenant SaaS ERP

> **A modern, cloud-native ERP system for Mexican SMEs**

Corelio is a multi-tenant SaaS ERP designed specifically for Mexican small and medium-sized enterprises (SMEs), starting with hardware stores (ferreterías). It provides a unified platform for Point of Sale (POS), inventory management, customer relationship management, and CFDI (Mexican tax) compliance.

## 🎯 Project Status

**Status:** Planning Complete - Ready for Implementation

**Current Phase:** Phase 1 - Foundation & .NET Aspire Setup

## ✨ Key Features

- 🏪 **Fast Point of Sale (POS)** - Sub-5-second checkout optimized for high-volume retail
- 📦 **Inventory Management** - Real-time stock tracking across multiple warehouses
- 👥 **Customer Management** - Complete CRM with RFC validation for CFDI
- 🧾 **CFDI 4.0 Compliance** - Automated Mexican tax-compliant invoicing
- 🌐 **Multi-Tenant Architecture** - Complete data isolation with row-level security
- 📊 **Real-time Analytics** - Business insights with OpenTelemetry via .NET Aspire
- 🔐 **Role-Based Access Control** - Granular permissions (Owner, Cashier, Accountant, etc.)
- 🚀 **Cloud-Native** - Built with .NET Aspire for modern deployment

## 🛠️ Technology Stack

### Backend
- **.NET 10** - Latest framework with 40% faster JSON serialization
- **C# 14** - Modern language features (primary constructors, collection expressions)
- **.NET Aspire** - Cloud-native orchestration and observability
- **PostgreSQL 16** - Robust ACID-compliant database
- **EF Core 10** - ORM with migrations
- **Redis 7** - Distributed caching

### Frontend
- **Blazor Server** - Real-time UI with SignalR
- **MudBlazor 8.x** - Material Design component library

### Architecture
- **Clean Architecture** - Clear separation of concerns
- **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- **Multi-Tenancy** - Row-level security with automatic filtering

## 📚 Documentation

Comprehensive planning documentation is available in [`docs/planning/`](./docs/planning/):

| Document | Description |
|----------|-------------|
| [**README.md**](./docs/planning/README.md) | Main implementation plan with 12-week roadmap |
| [**00-architecture-specification.md**](./docs/planning/00-architecture-specification.md) | Complete system architecture and design patterns |
| [**01-database-schema-design.md**](./docs/planning/01-database-schema-design.md) | PostgreSQL schema with all 24 tables |
| [**02-api-specification.md**](./docs/planning/02-api-specification.md) | REST API endpoints and documentation |
| [**03-multi-tenancy-implementation-guide.md**](./docs/planning/03-multi-tenancy-implementation-guide.md) | Multi-tenant isolation guide |
| [**04-cfdi-integration-specification.md**](./docs/planning/04-cfdi-integration-specification.md) | CFDI 4.0 compliance specification |

**📖 [View Documentation Index](./docs/planning/INDEX.md)**

## 🚀 Quick Start

### Prerequisites

1. **.NET 10 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
2. **Visual Studio 2022 17.12+** or **JetBrains Rider 2024.3+**
3. **Docker Desktop** (for .NET Aspire containers)
4. **.NET Aspire Workload:**
   ```bash
   dotnet workload update
   dotnet workload install aspire
   ```

### Installation

```bash
# Clone the repository
git clone https://github.com/your-org/corelio.git
cd corelio

# Create .NET Aspire solution structure
dotnet new sln -n Corelio

# Create Aspire orchestration
dotnet new aspire-apphost -n Corelio.AppHost -o src/Aspire/Corelio.AppHost
dotnet new aspire-servicedefaults -n Corelio.ServiceDefaults -o src/Aspire/Corelio.ServiceDefaults

# Create domain projects
dotnet new classlib -n Corelio.Domain -o src/Core/Corelio.Domain
dotnet new classlib -n Corelio.Application -o src/Core/Corelio.Application

# Create infrastructure
dotnet new classlib -n Corelio.Infrastructure -o src/Infrastructure/Corelio.Infrastructure

# Create API
dotnet new webapi -n Corelio.WebAPI -o src/Presentation/Corelio.WebAPI

# Create Blazor app
dotnet new blazor -n Corelio.BlazorApp -o src/Presentation/Corelio.BlazorApp

# Add all projects to solution
dotnet sln add src/Aspire/Corelio.AppHost/Corelio.AppHost.csproj
dotnet sln add src/Aspire/Corelio.ServiceDefaults/Corelio.ServiceDefaults.csproj
dotnet sln add src/Core/Corelio.Domain/Corelio.Domain.csproj
dotnet sln add src/Core/Corelio.Application/Corelio.Application.csproj
dotnet sln add src/Infrastructure/Corelio.Infrastructure/Corelio.Infrastructure.csproj
dotnet sln add src/Presentation/Corelio.WebAPI/Corelio.WebAPI.csproj
dotnet sln add src/Presentation/Corelio.BlazorApp/Corelio.BlazorApp.csproj

# Run the Aspire AppHost (starts everything!)
dotnet run --project src/Aspire/Corelio.AppHost
```

### Access Points

Once running:
- **Aspire Dashboard:** http://localhost:15888 (monitoring & observability)
- **API:** Auto-assigned port (check Aspire dashboard)
- **Blazor App:** Auto-assigned port (check Aspire dashboard)

## 📦 Project Structure

```
Corelio/
├── src/
│   ├── Aspire/
│   │   ├── Corelio.AppHost/              # .NET Aspire orchestration
│   │   └── Corelio.ServiceDefaults/      # Shared configurations
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
    └── planning/                          # All planning documentation
```

## 🎯 Implementation Roadmap

| Phase | Duration | Deliverables |
|-------|----------|--------------|
| **Phase 1** | Weeks 1-2 | Foundation & Aspire Setup, Multi-Tenancy, Authentication |
| **Phase 2** | Weeks 3-4 | Product & Inventory Management |
| **Phase 3** | Week 5 | Customer Management |
| **Phase 4** | Weeks 6-7 | POS System |
| **Phase 5** | Week 8 | Sales Management (Quotes, Credit Notes) |
| **Phase 6** | Weeks 9-10 | CFDI Integration |
| **Phase 7** | Week 11 | Testing & Refinement |
| **Phase 8** | Week 12 | Deployment & MVP Launch |

**Total Timeline:** 12 weeks to MVP

## 🔑 Key Differentiators

1. **Truly Unified** - Single source of truth for all business operations
2. **México-Specific** - Built-in CFDI 4.0 compliance with PAC integration
3. **Cloud-Native** - Modern .NET Aspire architecture from day one
4. **Multi-Tenant** - Efficient resource sharing with complete data isolation
5. **Fast POS** - Sub-5-second checkout optimized for high-volume retail
6. **Modern Stack** - .NET 10, C# 14, latest best practices

## 👥 Target Users

- **Primary:** Hardware store owners/managers in México
- **Roles Supported:**
  - Owner/Manager (full access)
  - Cashier (POS operations)
  - Inventory Manager (products & stock)
  - Accountant (CFDI & reports)
  - Administrator (user management)
  - Seller (sales & customers)

## 📊 Performance Targets

| Operation | Target | Status |
|-----------|--------|--------|
| Product search | < 0.5s | ⏳ Planned |
| Add to cart | < 0.2s | ⏳ Planned |
| Complete sale | < 3s | ⏳ Planned |
| CFDI stamping | < 5s | ⏳ Planned |

## 🔐 Security

- **Multi-Tenant Isolation:** 100% data separation with row-level security
- **Authentication:** JWT with tenant claims
- **Authorization:** Permission-based RBAC
- **Encryption:** TLS 1.3 in transit, database encryption at rest
- **CFDI Certificates:** Encrypted storage for CSD credentials

## 📈 Scalability

- **Horizontal Scaling:** Load-balanced API instances
- **Database:** PostgreSQL with read replicas
- **Caching:** Redis cluster
- **Target Capacity:** 100-500 tenants per server

## 🤝 Contributing

This is a private commercial project. For access, please contact the project team.

## 📄 License

Proprietary - Copyright © 2025 DeventSoft

## 🆘 Support

For questions or support:
- **Email:** support@deventsoft.com
- **Documentation:** [docs/planning/](./docs/planning/)

---

**Built with ❤️ in México for Mexican SMEs**

**Powered by:** .NET 10 + C# 14 + .NET Aspire + PostgreSQL + Blazor
