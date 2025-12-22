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
