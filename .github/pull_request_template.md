## Description

<!-- Provide a brief description of the changes in this PR -->

## Type of Change

<!-- Mark with an `x` all that apply -->

- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Refactoring (no functional changes, no API changes)
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code quality improvement

## Related Issues

<!-- Link to related issues -->

Closes #
Relates to #

## Changes Made

<!-- List the main changes made in this PR -->

-
-
-

## Testing

### Test Coverage

- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Multi-tenancy isolation tests included
- [ ] Performance tests added (if applicable)
- [ ] CFDI compliance tests added (if applicable)

### Test Results

<!-- Paste test results or link to CI/CD run -->

```
All tests passing: ✅ / ❌
Code coverage: __%
```

## Screenshots (if applicable)

<!-- Add screenshots to help explain your changes -->

## Database Changes

- [ ] No database changes
- [ ] Migration added (filename: `____________________`)
- [ ] Seed data updated
- [ ] Rollback script provided

## Security Considerations

- [ ] No security implications
- [ ] Security review completed
- [ ] Multi-tenant data isolation verified
- [ ] No hardcoded secrets
- [ ] Proper authorization checks added
- [ ] SQL injection prevention verified
- [ ] XSS prevention verified

## Performance Impact

- [ ] No performance impact
- [ ] Performance tests passing
- [ ] Database queries optimized
- [ ] Caching implemented (if needed)
- [ ] Meets performance targets (< values noted)

## Breaking Changes

<!-- If breaking changes, describe the migration path -->

- [ ] No breaking changes
- [ ] Breaking changes documented below

**Migration Guide:**
<!-- Provide step-by-step migration instructions -->

## Checklist

<!-- Ensure all items are checked before requesting review -->

### Code Quality

- [ ] Code follows C# 14 best practices
- [ ] File-scoped namespaces used
- [ ] Primary constructors used for DI
- [ ] Collection expressions `[]` used
- [ ] XML documentation added for public methods
- [ ] No magic strings or numbers
- [ ] `dotnet format` applied

### Architecture

- [ ] Follows Clean Architecture principles
- [ ] Domain layer has zero dependencies
- [ ] CQRS pattern used correctly
- [ ] Proper separation of concerns

### Frontend (REQUIRED for User-Facing Features)

**⚠️ CRITICAL: Backend-only PRs for user-facing features do NOT satisfy Definition of Done**

#### Blazor UI Implementation
- [ ] **Blazor pages/components created** (Required unless exemption justified below)
- [ ] **Spanish localization implemented** (*.es-MX.resx files created)
- [ ] All UI text uses `IStringLocalizer<T>` (NO hardcoded Spanish strings)
- [ ] MudBlazor components used consistently with design system
- [ ] Forms have validation with Spanish error messages
- [ ] Responsive design (tested on mobile, tablet, desktop)
- [ ] Date format: dd/MM/yyyy (Mexican locale)
- [ ] Currency format: $1,234.56 MXN (when applicable)

#### Service Layer (Blazor)
- [ ] Service interfaces created (e.g., IAuthService, IProductService)
- [ ] Service implementations call WebAPI endpoints via HttpClient
- [ ] HttpClient configured with Aspire service discovery
- [ ] Error handling implemented (Result<T> pattern)
- [ ] Loading states handled in UI (spinners, progress indicators)

#### Component Tests
- [ ] bUnit component tests added
- [ ] Service layer unit tests added (mock HttpClient)
- [ ] Frontend tests passing (>70% coverage goal)

#### Demo-Ready Verification
- [ ] **Feature is demo-able via Blazor UI** (stakeholder can see and interact)
- [ ] **Feature works end-to-end** (no Postman/Swagger/API workarounds)
- [ ] **All UI text in Spanish (es-MX)**
- [ ] User can complete the entire workflow through UI

#### Frontend Exemption (Complete if NO frontend)
**Is this PR backend-only?** ☐ Yes ☐ No

**If yes, this is acceptable ONLY if:**
- [ ] This is an infrastructure story (migrations, services, middleware, etc.)
- [ ] This is a technical debt or refactoring task
- [ ] This is an API-only feature (explicitly documented as such)

**If frontend is deferred, provide:**
- Frontend tracking issue: #_____
- Estimated completion date: YYYY-MM-DD
- Reason for deferral: _______________
- Who approved deferral: @_______________

**WARNING:** User-facing features MUST include frontend. A feature is not "done" if users cannot interact with it via the Blazor UI.

### Multi-Tenancy

- [ ] All business entities implement `ITenantEntity`
- [ ] No `tenant_id` accepted from client input
- [ ] Query filters applied correctly
- [ ] Tenant isolation tests passing

### Documentation

- [ ] README updated (if needed)
- [ ] API documentation updated (Swagger)
- [ ] CHANGELOG updated
- [ ] Migration guide provided (for breaking changes)

### Review

- [ ] Self-reviewed code
- [ ] Code reviewed by at least one other developer
- [ ] All CI/CD checks passing
- [ ] No merge conflicts

## Reviewer Notes

<!-- Any specific areas you'd like reviewers to focus on -->

## Deployment Notes

<!-- Any special deployment considerations -->

---

By submitting this pull request, I confirm that my contribution is made under the terms of the project license.
