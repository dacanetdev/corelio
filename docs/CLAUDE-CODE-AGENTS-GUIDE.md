# Claude Code Agents & Skills Guide

This document explains how to use the specialized agents and skills configured for the Corelio project.

## What are Agents and Skills?

**Agents** are specialized personas that Claude Code can adopt to provide expert guidance in specific areas.

**Skills** are quality checks and best practices that Claude Code enforces during development.

---

## Available Agents

### 1. Backend Architect (`backend-architect`)

**Expertise:** Clean Architecture, DDD, CQRS, .NET 10

**When to use:**
- Designing or implementing Domain layer entities
- Creating Application layer CQRS handlers
- Ensuring Clean Architecture principles are followed
- Implementing domain events or value objects

**How to invoke:**
```
@backend-architect Please review this entity for Clean Architecture compliance
```

**Key focus areas:**
- Zero dependencies in Domain layer
- CQRS pattern with MediatR
- Multi-tenancy (ITenantEntity)
- C# 14 features (required members, primary constructors)

---

### 2. Frontend Developer (`frontend-developer`)

**Expertise:** Blazor Server, MudBlazor, component development

**When to use:**
- Creating or modifying Blazor components
- Implementing UI forms with validation
- Building data grids or tables
- Implementing keyboard shortcuts for POS

**How to invoke:**
```
@frontend-developer Help me create a MudBlazor form for product entry
```

**Key focus areas:**
- MudBlazor component best practices
- Proper component lifecycle methods
- Keyboard accessibility
- Performance optimization (rendering, StateHasChanged)

---

### 3. Database Architect (`database-architect`)

**Expertise:** PostgreSQL, EF Core 10, migrations, multi-tenant database design

**When to use:**
- Creating or modifying database entities
- Writing EF Core configurations
- Creating migrations
- Optimizing database queries
- Implementing indexes

**How to invoke:**
```
@database-architect Review this migration for multi-tenancy and performance
```

**Key focus areas:**
- Multi-tenant query filters and interceptors
- Proper indexing strategies
- PostgreSQL-specific features
- Migration best practices
- Query performance optimization

---

### 4. Security Auditor (`security-auditor`)

**Expertise:** Multi-tenant security, OWASP, data isolation, authentication

**When to use:**
- Reviewing code for security vulnerabilities
- Implementing authentication/authorization
- Ensuring multi-tenant data isolation
- Validating input security
- Before merging critical security-related PRs

**How to invoke:**
```
@security-auditor Audit this code for multi-tenant security vulnerabilities
```

**Key focus areas:**
- 100% tenant isolation (zero data leaks)
- SQL injection prevention
- XSS prevention
- Proper JWT validation
- Secret management (no hardcoded secrets)

---

### 5. CFDI Specialist (`cfdi-specialist`)

**Expertise:** CFDI 4.0, Mexican tax regulations, SAT compliance

**When to use:**
- Implementing CFDI invoice generation
- Validating RFC formats
- Implementing SAT code validation
- Working with PAC providers
- Implementing certificate management

**How to invoke:**
```
@cfdi-specialist Help me implement CFDI 4.0 invoice generation
```

**Key focus areas:**
- CFDI 4.0 XML structure
- SAT validation rules
- Certificate security (Azure Key Vault)
- Tax calculations (IVA 16%)
- PAC integration

---

## Available Skills

### 1. Code Style (`dotnet-quality-check`)

**Purpose:** Enforces C# 14/15 best practices and .NET 10 standards

**Automatically checks:**
- File-scoped namespaces
- Primary constructors for dependency injection
- Collection expressions `[]`
- Proper async/await patterns
- XML documentation on public methods
- No hardcoded secrets
- SonarQube quality gate

**How to invoke:**
```
@dotnet-quality-check Review this code for quality standards
```

---

### 2. Testing (`comprehensive-testing`)

**Purpose:** Ensures comprehensive test coverage

**Automatically checks:**
- Unit test coverage (target >80% for Domain/Application)
- Integration tests with Testcontainers
- Multi-tenancy isolation tests
- Performance tests
- Security tests
- CFDI compliance tests

**How to invoke:**
```
@comprehensive-testing Help me write tests for this handler
```

---

### 3. Performance (`performance-optimization`)

**Purpose:** Ensures optimal performance

**Automatically checks:**
- Database query optimization (AsNoTracking, indexes)
- Proper caching strategies (Redis)
- API performance (response caching, compression)
- Blazor rendering optimization
- Memory management

**Performance targets:**
- Product search: <500ms (P95)
- POS checkout: <3 seconds (P95)
- CFDI generation: <5 seconds (P95)

**How to invoke:**
```
@performance-optimization Review this code for performance issues
```

---

### 4. Documentation (`documentation-standards`)

**Purpose:** Ensures comprehensive documentation

**Automatically checks:**
- XML documentation on all public members
- README files for modules
- API documentation (Swagger)
- Architecture documentation
- Code comments quality

**How to invoke:**
```
@documentation-standards Review documentation for this module
```

---

## Usage Examples

### Example 1: Creating a new entity

```
@backend-architect and @database-architect

I need to create a Product entity with the following fields:
- Name (string, required)
- SKU (string, unique per tenant)
- Price (decimal)
- Category (foreign key)

Please help me create the entity following Clean Architecture
and multi-tenancy best practices.
```

### Example 2: Implementing a CFDI invoice

```
@cfdi-specialist

I need to implement CFDI invoice generation from a Sale entity.
Please guide me through:
1. XML structure
2. SAT validation
3. Certificate signing
4. PAC integration

Ensure compliance with CFDI 4.0 standards.
```

### Example 3: Building a Blazor form

```
@frontend-developer

Create a MudBlazor form for creating a new customer with:
- Name (required)
- RFC (required, validated)
- Email (required)
- Phone (optional)

Include proper validation, error messages, and keyboard navigation.
```

### Example 4: Security review before PR

```
@security-auditor

Please review this PR for:
1. Multi-tenant data isolation
2. SQL injection vulnerabilities
3. XSS vulnerabilities
4. Proper authorization checks
5. Secret management

This is for the customer management module.
```

### Example 5: Performance optimization

```
@performance-optimization and @database-architect

This query is taking 2 seconds to return results:

[paste code]

Please help optimize it to meet our <500ms target.
```

---

## Combining Multiple Agents/Skills

You can invoke multiple agents or skills in a single request:

```
@backend-architect @security-auditor @dotnet-quality-check

Please review this CQRS handler for:
1. Clean Architecture compliance
2. Security vulnerabilities
3. Code quality standards
```

---

## Best Practices

1. **Be specific:** Tell the agent exactly what you need
2. **Provide context:** Share relevant code or requirements
3. **Use for reviews:** Invoke agents before committing critical code
4. **Combine expertise:** Use multiple agents when needed
5. **Learn from feedback:** Agents provide educational guidance

---

## File Locations

All agent and skill definitions are in:

```
.claude/
├── agents/
│   ├── architect.md              # Backend architect
│   ├── cfdi-specialist.md        # CFDI expert
│   ├── database-architect.md     # Database expert
│   ├── frontend-developer.md     # Blazor expert
│   └── security-auditor.md       # Security expert
└── skills/
    ├── code-style.md             # Code quality checks
    ├── documentation.md          # Documentation standards
    ├── performance.md            # Performance optimization
    └── testing.md                # Testing standards
```

---

## Modifying Agents/Skills

To modify an agent or skill:

1. Edit the corresponding `.md` file
2. Update the instructions section
3. Commit and push changes
4. Claude Code will use the updated version immediately

---

## Common Workflows

### Before Starting a Feature
```
@backend-architect

I'm about to implement [feature]. What's the best architectural approach?
```

### During Development
```
@dotnet-quality-check

Review this code for quality standards before I commit.
```

### Before Creating PR
```
@security-auditor @comprehensive-testing

Review this feature for security and test coverage.
```

### Performance Issues
```
@performance-optimization

This endpoint is slow. Help me optimize it.
```

---

## Support

For questions about agents and skills:
- Review this guide
- Check agent/skill definition files in `.claude/`
- Consult CLAUDE.md for general development guidelines

---

**Last Updated:** 2025-12-22
**Maintained By:** Development Team
