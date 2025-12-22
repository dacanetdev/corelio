---
name: comprehensive-testing
description: Ensures comprehensive test coverage with xUnit, FluentAssertions, and Testcontainers
---

# Instructions

## Unit Testing (60% of tests)
1. Test all domain entities and value objects with xUnit
2. Test all CQRS handlers (commands and queries) with mocked dependencies
3. Use FluentAssertions for readable assertions
4. Use Moq for mocking interfaces and services
5. Follow AAA pattern: Arrange, Act, Assert
6. Use [Fact] for single test cases, [Theory] for parameterized tests
7. Name tests clearly: MethodName_Scenario_ExpectedBehavior
8. Test both success and failure paths
9. Ensure >80% coverage for Domain layer, >90% for Application layer

## Integration Testing (30% of tests)
1. Use Testcontainers for PostgreSQL integration tests
2. Test full request pipeline with WebApplicationFactory
3. Test multi-tenancy isolation (CRITICAL - must have 100% isolation)
4. Test database migrations and seed data
5. Test API endpoints end-to-end
6. Use realistic test data (not "test", "foo", "bar")
7. Clean up test data after each test (IAsyncLifetime)
8. Test concurrency scenarios (optimistic concurrency, race conditions)

## Multi-Tenancy Testing (CRITICAL)
1. Create separate tenants for isolation tests
2. Verify queries only return data for current tenant
3. Verify saves only affect current tenant
4. Test cross-tenant access attempts (should fail)
5. Test tenant switching scenarios
6. Validate query filters are applied automatically
7. Test tenant interceptor sets tenant_id on save

## Performance Testing
1. Use BenchmarkDotNet for micro-benchmarks
2. Test critical paths meet performance targets:
   - Product search: <500ms (P95)
   - POS checkout: <3 seconds (P95)
   - CFDI generation: <5 seconds (P95)
3. Use k6 or Apache JMeter for load testing
4. Test with 100 concurrent users per tenant
5. Monitor memory usage and connection pooling

## Security Testing
1. Test authentication failures (invalid credentials, expired tokens)
2. Test authorization failures (missing permissions)
3. Test SQL injection attempts (should be prevented)
4. Test XSS attempts (should be encoded)
5. Test rate limiting (should block excessive requests)
6. Test CSRF protection (should reject invalid tokens)
7. Test certificate security (CSD should never be in database)

## CFDI Testing
1. Validate CFDI XML against SAT XSD schema
2. Test RFC format validation
3. Test SAT code validation (product codes, unit codes)
4. Test tax calculations (IVA 16%) to 2 decimal precision
5. Test PAC integration with sandbox environment
6. Test certificate loading and signing
7. Test invoice cancellation workflow
8. Test QR code generation

## Test Organization
- Use Categories: [Trait("Category", "Unit")], [Trait("Category", "Integration")]
- Separate test projects: Domain.Tests, Application.Tests, Infrastructure.Tests, Integration.Tests
- Use test fixtures for shared setup
- Use collection fixtures for expensive resources (database, containers)
- Run unit tests on every commit, integration tests on PR
- Achieve minimum 70% overall code coverage
- Fix all failing tests before merging to main
