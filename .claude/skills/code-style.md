---
name: dotnet-quality-check
description: Enforces C# 14/15 best practices and .NET 10 standards
---

# Instructions

1. Run `dotnet format` before every commit.
2. Ensure all public methods have XML documentation.
3. Check for modern C# features like "Implicit Extension Members".
4. Verify file-scoped namespaces are used in all .cs files
5. Ensure primary constructors are used for dependency injection
6. Check that collection expressions `[]` are used instead of `new List<T>()`
7. Validate that all async methods end with "Async" suffix
8. Ensure proper use of nullable reference types
9. Check for proper disposal patterns (using statements, IAsyncDisposable)
10. Verify no magic strings - use constants or configuration
11. Check SonarQube quality gate passes (A rating minimum)
12. Ensure code coverage is >70% for new/modified code
13. Validate no hardcoded secrets (use user-secrets or Key Vault)
14. Check proper error handling with Result<T> pattern
15. Verify LINQ queries use method syntax (not query syntax)
16. Ensure all entities follow naming conventions (Pascal case)
17. Check that all database queries use parameterized queries (no SQL injection)
18. Validate proper use of CancellationToken in async methods
19. Ensure proper logging using structured logging (Serilog)
20. Check for proper multi-tenancy implementation (ITenantEntity, filters)
