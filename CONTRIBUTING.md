# Contributing to Corelio

Thank you for your interest in contributing to Corelio! This document provides guidelines for contributing to the project.

## Code of Conduct

Be respectful, professional, and constructive in all interactions.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 17.12+ or JetBrains Rider 2024.3+
- Docker Desktop (for .NET Aspire)
- Git

### Setup Development Environment

1. **Clone the repository**
   ```bash
   git clone https://github.com/dacanetdev/corelio.git
   cd corelio
   ```

2. **Install .NET Aspire workload**
   ```bash
   dotnet workload update
   dotnet workload install aspire
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run with Aspire**
   ```bash
   dotnet run --project src/Aspire/Corelio.AppHost
   ```

5. **Read the developer guide**
   See [CLAUDE.md](./CLAUDE.md) for architecture and coding standards.

## How to Contribute

### Reporting Bugs

1. Check existing issues to avoid duplicates
2. Use the **Bug Report** template
3. Provide detailed reproduction steps
4. Include logs and screenshots if applicable
5. Label with appropriate module and severity

### Suggesting Features

1. Check existing feature requests
2. Use the **Feature Request** template
3. Explain the problem and proposed solution
4. Consider impact on existing features
5. Be open to discussion and alternatives

### Pull Requests

1. **Fork** the repository
2. **Create a branch** from `develop`
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes** following our coding standards
4. **Write tests** (aim for >70% coverage)
5. **Run tests locally**
   ```bash
   dotnet test
   ```

6. **Format your code**
   ```bash
   dotnet format
   ```

7. **Commit with conventional commits format**
   ```bash
   git commit -m "feat(pos): add barcode scanning support"
   ```

8. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

9. **Open a Pull Request** against `develop` branch

## Coding Standards

### C# 14 Best Practices

- ✅ Use **file-scoped namespaces**
- ✅ Use **primary constructors** for dependency injection
- ✅ Use **collection expressions** `[]` instead of `new List<T>()`
- ✅ Use **required** members for mandatory properties
- ✅ Add **XML documentation** for all public methods
- ✅ Follow **naming conventions** (PascalCase for classes/methods, _camelCase for private fields)

### Architecture

- ✅ Follow **Clean Architecture** principles
- ✅ Keep **Domain layer** dependency-free
- ✅ Use **CQRS pattern** with MediatR
- ✅ Implement **Result<T>** pattern for error handling
- ✅ Use **FluentValidation** for input validation

### Multi-Tenancy (CRITICAL)

- ✅ **All business entities** must implement `ITenantEntity`
- ✅ **NEVER** accept `tenant_id` from client input
- ✅ **ALWAYS** use `ITenantService.GetCurrentTenantId()`
- ✅ Write **isolation tests** for tenant separation
- ⚠️ **Zero tolerance** for data leaks between tenants

### Testing

- ✅ Write **unit tests** for business logic (>80% coverage)
- ✅ Write **integration tests** for API endpoints
- ✅ Write **multi-tenancy isolation tests** (mandatory)
- ✅ Use **Testcontainers** for database integration tests
- ✅ Use **AAA pattern** (Arrange, Act, Assert)

### Commit Messages

Follow **Conventional Commits**:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style (formatting)
- `refactor`: Code refactoring
- `perf`: Performance improvement
- `test`: Adding tests
- `chore`: Maintenance
- `ci`: CI/CD changes

**Examples:**
```
feat(pos): add barcode scanner support
fix(auth): correct JWT expiration handling
docs(readme): update installation instructions
refactor(domain): simplify product entity
perf(query): optimize product search with indexing
test(integration): add multi-tenancy isolation tests
```

## Pull Request Process

1. **Fill out the PR template** completely
2. **Link related issues** (`Closes #123`)
3. **Ensure all CI checks pass**:
   - ✅ Build successful
   - ✅ All tests passing
   - ✅ Code coverage >70%
   - ✅ SonarQube quality gate (A rating)
   - ✅ Security scan clean
   - ✅ Multi-tenancy checks passing

4. **Request review** from code owners
5. **Address review comments** promptly
6. **Squash commits** if requested
7. **Wait for approval** (at least 1 approval required)

## Code Review Guidelines

### For Contributors

- Be open to feedback
- Respond to comments promptly
- Explain your decisions when asked
- Don't take criticism personally
- Update PR based on feedback

### For Reviewers

- Be constructive and respectful
- Explain why changes are needed
- Suggest alternatives when possible
- Approve when standards are met
- Focus on:
  - Architecture compliance
  - Multi-tenancy security
  - Code quality
  - Test coverage
  - Performance impact

## Branch Strategy

- **main** → Production (protected)
- **develop** → Staging (protected)
- **feature/** → New features
- **fix/** → Bug fixes
- **hotfix/** → Emergency production fixes

## Security

- **NEVER** commit secrets, passwords, or API keys
- Use **dotnet user-secrets** for local development
- Use **Azure Key Vault** for production secrets
- Report security vulnerabilities to **security@deventsoft.com**
- See [SECURITY.md](./SECURITY.md) for more details

## Documentation

- Update **README.md** if adding major features
- Update **API documentation** (Swagger annotations)
- Add **inline comments** for complex logic (WHY, not WHAT)
- Update **CHANGELOG.md** for notable changes
- Create **migration guides** for breaking changes

## Questions?

- Check [CLAUDE.md](./CLAUDE.md) for development guidelines
- Check [docs/](./docs/) for detailed documentation
- Open a **Discussion** on GitHub
- Contact maintainers: **support@deventsoft.com**

## License

By contributing, you agree that your contributions will be licensed under the project's license (see [LICENSE](./LICENSE)).

---

**Thank you for contributing to Corelio!** 🎉
