
**Project:** Corelio Multi-Tenant SaaS ERP
**QA Lead:** [TBD]
**Last Updated:** 2025-12-21

---

## QA Objectives

1. **Zero Data Leaks:** 100% multi-tenant isolation verified
2. **Performance:** Meet all performance targets (sub-3s checkout, sub-500ms search)
3. **Security:** Pass OWASP Top 10 security audit
4. **CFDI Compliance:** 100% SAT validation success rate
5. **Code Quality:** >70% test coverage, SonarQube A rating
6. **Availability:** Achieve 99.9% uptime in production

---

## Testing Pyramid

```
           /\
          /  \
         / E2E\      10% - End-to-End Tests (critical paths)
        /______\
       /        \
      /Integration\ 30% - Integration Tests (API + DB)
     /____________\
    /              \
   /   Unit Tests   \ 60% - Unit Tests (business logic)
  /__________________\
```

**Target Test Coverage:** >70% overall

---

## Testing Types

### 1. Unit Testing

**Scope:** Domain layer, Application layer (CQRS handlers)

**Tools:**
- xUnit
- FluentAssertions
- Moq (for mocking)

**Coverage Targets:**
- Domain entities: >80%
- CQRS handlers: >90%
- Services: >75%

**Example Test:**
```csharp
[Fact]
public void CreateProductCommand_ValidData_ReturnsSuccess()
{
    // Arrange
    var command = new CreateProductCommand("Martillo", 150.00m);
    var handler = new CreateProductCommandHandler(_dbContext, _tenantService);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
    _dbContext.Products.Should().HaveCount(1);
}
```

**Continuous Execution:**
- Run on every commit (CI pipeline)
- Developers run locally before push
- Must pass for PR approval

### 2. Integration Testing

**Scope:** API endpoints, database interactions, multi-tenancy isolation

**Tools:**
- xUnit
- Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)
- Testcontainers (PostgreSQL)
- FluentAssertions

**Key Test Scenarios:**
```csharp
[Fact]
public async Task GetProducts_OnlyReturnsTenantProducts()
{
    // Arrange: Create 2 tenants with products
    var tenant1 = await CreateTenant("Tenant1");
    var tenant2 = await CreateTenant("Tenant2");

    await SeedProducts(tenant1.Id, count: 5);
    await SeedProducts(tenant2.Id, count: 3);

    SetCurrentTenant(tenant1.Id);

    // Act: Request products API
    var response = await _client.GetAsync("/api/v1/products");
    var products = await response.Content.ReadAsAsync<List<ProductDto>>();

    // Assert: Only tenant1's products returned
    products.Should().HaveCount(5);
    products.Should().OnlyContain(p => p.TenantId == tenant1.Id);
}
```

**Execution:**
- Run on PR creation
- Run nightly on main branch
- Must pass for merge to main

### 3. Performance Testing

**Tools:**
- BenchmarkDotNet (micro-benchmarks)
- k6 or Apache JMeter (load testing)
- Application Insights (production monitoring)

**Performance Test Scenarios:**

| Scenario | Target | Tool |
|----------|--------|------|
| Product search (autocomplete) | <500ms (P95) | k6 |
| Add to cart | <200ms (P95) | k6 |
| Complete sale (POS) | <3s (P95) | k6 |
| CFDI generation | <5s (P95) | k6 |
| Dashboard load | <2s (P95) | k6 |
| Concurrent users (per tenant) | 100 users | k6 |

**Load Test Example (k6):**
```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '2m', target: 100 }, // Ramp up to 100 users
    { duration: '5m', target: 100 }, // Stay at 100 users
    { duration: '2m', target: 0 },   // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% of requests < 500ms
  },
};

export default function () {
  let response = http.get('https://api.corelio.com.mx/api/v1/products/search?q=martillo');
  check(response, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
  sleep(1);
}
```

**Execution:**
- Run before each release
- Run after performance optimizations
- Continuous monitoring in production

### 4. Security Testing

**Focus Areas:**
1. Multi-tenant data isolation
2. Authentication & authorization
3. SQL injection prevention
4. XSS prevention
5. CSRF protection
6. Certificate security (CSD in Key Vault)
7. Secrets management

**Tools:**
- SonarQube (static analysis)
- OWASP ZAP (penetration testing)
- Snyk (dependency vulnerabilities)
- Manual code reviews

**Security Test Cases:**

| Test | Verification |
|------|--------------|
| Cross-tenant data access | Attempt to access other tenant's data via API (should fail 401/403) |
| SQL injection | Submit malicious SQL in search fields (should be sanitized) |
| XSS | Submit `<script>` tags in inputs (should be encoded) |
| Weak passwords | Try to create user with "password123" (should fail validation) |
| Expired JWT | Use expired token (should return 401) |
| Missing authorization | Call protected endpoint without permission (should return 403) |

**Execution:**
- SonarQube on every commit
- OWASP ZAP before each release
- External security audit before MVP launch

### 5. Multi-Tenancy Isolation Testing

**Critical Tests:**

```csharp
public class MultiTenancyIsolationTests
{
    [Fact]
    public async Task CannotAccessOtherTenantData()
    {
        // Test for every entity type
        var entities = new[] {
            typeof(Product),
            typeof(Customer),
            typeof(Sale),
            typeof(Invoice)
        };

        foreach (var entityType in entities)
        {
            await VerifyIsolation(entityType);
        }
    }

    [Fact]
    public async Task CannotModifyOtherTenantData()
    {
        // Attempt to update product from different tenant
        var tenant1Product = await CreateProduct(tenant1.Id);

        SetCurrentTenant(tenant2.Id);

        var updateCommand = new UpdateProductCommand(tenant1Product.Id, "Hacked");
        var result = await _mediator.Send(updateCommand);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found"); // or 403 Forbidden
    }

    [Fact]
    public async Task CannotDeleteOtherTenantData()
    {
        // Similar test for delete operations
    }
}
```

**Execution:**
- Run on every commit
- Additional manual testing before each release
- Mandatory for PR approval

### 6. CFDI Compliance Testing

**SAT Validation Tests:**

| Test | Verification |
|------|--------------|
| XML Schema Validation | Generated XML validates against SAT XSD schema |
| RFC Format | Issuer and receiver RFC match SAT regex |
| SAT Codes | Product codes (ClaveProdServ) exist in SAT catalog |
| Unit Codes | Unit codes (ClaveUnidad) exist in SAT catalog |
| Tax Calculations | IVA calculations accurate to 2 decimal places |
| Digital Signature | CSD signature valid and verifiable |
| UUID Format | UUID matches SAT format (36 chars with dashes) |

**Tools:**
- SAT XSD schemas for validation
- PAC sandbox environment
- Custom CFDI validator

**Execution:**
- Unit tests for XML generation
- Integration tests with PAC sandbox
- Manual testing with real CSD certificates (test environment)

### 7. User Acceptance Testing (UAT)

**Participants:**
- Product Owner
- Internal stakeholders (1-2 people familiar with hardware store operations)
- Optionally: Pilot customer (early adopter)

**UAT Scenarios:**

| Scenario | Steps | Expected Outcome |
|----------|-------|------------------|
| Register New Tenant | 1. Fill registration form<br>2. Verify email<br>3. Login | Account created, default data seeded |
| Create Products | 1. Navigate to Products<br>2. Add 10 products with categories | Products visible in catalog |
| Perform Sale (POS) | 1. Open POS<br>2. Search product<br>3. Add to cart<br>4. Complete payment | Sale completed in <5 seconds |
| Generate CFDI | 1. Select sale<br>2. Generate invoice<br>3. Stamp with PAC | Invoice stamped with UUID, PDF generated |
| Stock Adjustment | 1. Navigate to Inventory<br>2. Adjust stock<br>3. View history | Stock updated, transaction logged |

**Duration:** 2 days (Week 11, Days 53-54)

**Success Criteria:**
- All critical scenarios pass
- No severity 1 or 2 bugs found
- User feedback incorporated

### 8. Regression Testing

**Scope:** Re-test existing functionality after new features added

**Approach:**
- Automated regression suite (integration + E2E tests)
- Manual smoke tests for critical paths

**Automated Regression Suite:**
- Authentication flow
- Multi-tenancy isolation
- Product CRUD
- POS checkout
- CFDI generation
- Payment processing

**Execution:**
- After each merge to main
- Before each release

### 9. Exploratory Testing

**Scope:** Unscripted testing to find edge cases

**Focus Areas:**
- UI/UX issues
- Error message clarity
- Performance under unusual conditions
- Browser compatibility (Chrome, Edge, Firefox)

**Duration:** 1 day (Week 11, Day 54)

**Participants:** QA Engineer + Developers

---

## Test Automation Strategy

**Automation Priorities:**

1. **High Priority (Must Automate):**
   - Multi-tenancy isolation tests
   - Authentication/authorization tests
   - Critical business logic (sale creation, inventory deduction)
   - CFDI XML generation

2. **Medium Priority (Should Automate):**
   - CRUD operations for all entities
   - Search functionality
   - Reporting

3. **Low Priority (Manual OK):**
   - UI visual regression
   - Exploratory testing

**CI/CD Integration:**

```yaml
# GitHub Actions Workflow
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '10.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Unit Tests
        run: dotnet test --no-build --filter Category=Unit --logger trx

      - name: Integration Tests
        run: dotnet test --no-build --filter Category=Integration --logger trx

      - name: Code Coverage
        run: |
          dotnet test --no-build --collect:"XPlat Code Coverage"
          reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html

      - name: SonarQube Scan
        run: |
          dotnet sonarscanner begin /k:"Corelio" /d:sonar.host.url="$SONAR_HOST" /d:sonar.login="$SONAR_TOKEN"
          dotnet build
          dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"

      - name: Upload Coverage
        uses: codecov/codecov-action@v2
```

---

## Defect Management

**Severity Levels:**

| Severity | Description | Response Time | Example |
|----------|-------------|---------------|---------|
| Sev 1 - Critical | Data loss, security breach, system down | <2 hours | Multi-tenant data leak |
| Sev 2 - High | Major feature broken, no workaround | <1 day | POS checkout fails |
| Sev 3 - Medium | Feature partially broken, workaround exists | <3 days | Search autocomplete slow |
| Sev 4 - Low | Minor UI issue, cosmetic | <1 week | Button alignment off |

**Bug Workflow:**
1. **Report:** Create GitHub issue with severity label
2. **Triage:** QA Lead assigns priority
3. **Fix:** Developer implements fix
4. **Verify:** QA verifies fix in test environment
5. **Close:** Merge to main, close issue

**Metrics Tracked:**
- Defect density (bugs per 1000 lines of code)
- Defect escape rate (bugs found in production)
- Mean time to resolution (MTTR)

---

## Quality Gates

**Definition of Done (DoD):**

For a feature to be considered complete:
- [ ] Code reviewed and approved
- [ ] Unit tests written (>70% coverage for new code)
- [ ] Integration tests written (for API endpoints)
- [ ] SonarQube quality gate passed (A rating)
- [ ] Manual testing completed
- [ ] Documentation updated
- [ ] No Sev 1 or Sev 2 bugs open

**Release Criteria:**

For a release to production:
- [ ] All features meet DoD
- [ ] >70% overall test coverage
- [ ] All automated tests passing
- [ ] Performance targets met (load tests passed)
- [ ] Security scan passed (OWASP ZAP clean)
- [ ] UAT completed and signed off
- [ ] No Sev 1 bugs, <3 Sev 2 bugs
- [ ] Deployment runbook reviewed
- [ ] Rollback plan documented

---

## Monitoring & Observability (Production)

**Tools:**
- Application Insights (Azure)
- Aspire Dashboard (development)
- Sentry (error tracking)

**Metrics Tracked:**

| Metric | Target | Alert Threshold |
|--------|--------|-----------------|
| Uptime | 99.9% | <99.5% in 24h |
| Response Time (P95) | <500ms | >1s for 5 min |
| Error Rate | <0.1% | >1% for 5 min |
| Database CPU | <70% | >90% for 5 min |
| Memory Usage | <80% | >95% for 5 min |

**Alerting:**
- Email + SMS for Sev 1 issues
- Email for Sev 2 issues
- Dashboard for Sev 3/4 issues

---

## Post-Launch Quality Plan

**Week 1 Post-Launch:**
- Daily health checks
- Monitor error logs
- Customer feedback collection
- Hotfix deployment ready (if needed)

**Ongoing:**
- Weekly review of metrics
- Monthly security scans
- Quarterly load testing
- Continuous monitoring

---

**Last Updated:** 2025-12-21
