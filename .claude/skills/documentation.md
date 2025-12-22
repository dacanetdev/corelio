---
name: documentation-standards
description: Ensures comprehensive and consistent code documentation
---

# Instructions

## XML Documentation
1. All public classes, methods, and properties MUST have XML documentation
2. Use /// summary tags for all public members
3. Use /// param tags for all method parameters
4. Use /// returns tags for all method return values
5. Use /// exception tags for all thrown exceptions
6. Include code examples in /// example tags for complex APIs
7. Use /// remarks for additional context or warnings
8. Document all public APIs thoroughly for future maintainability

## Code Comments
1. Use // comments sparingly - only when code intent isn't obvious
2. Explain WHY, not WHAT (code should be self-explanatory)
3. Add TODO comments with owner and date: // TODO(username, 2025-01-15): Description
4. Use WARNING comments for critical security or performance concerns
5. Remove commented-out code before committing
6. Update comments when updating code (stale comments are worse than none)

## README Files
1. Root README.md: Project overview, quick start, key features
2. Each major module should have its own README.md
3. Include setup instructions, prerequisites, and dependencies
4. Document environment variables and configuration
5. Include troubleshooting section for common issues
6. Update README when adding major features

## API Documentation
1. Use Swagger/OpenAPI for REST API documentation
2. Include request/response examples
3. Document all error codes and their meanings
4. Include authentication/authorization requirements
5. Document rate limiting and pagination
6. Keep Swagger UI accessible in development (/swagger)

## Architecture Documentation
1. Maintain architecture decision records (ADRs) in docs/adr/
2. Document major architectural decisions with rationale
3. Include diagrams (C4, sequence, ERD) in docs/diagrams/
4. Update documentation when architecture changes
5. Document deployment architecture and infrastructure

## Domain Documentation
1. Document domain concepts and ubiquitous language
2. Create glossary of domain terms (especially CFDI/Mexican tax terms)
3. Document business rules and constraints
4. Include examples of domain scenarios

## Testing Documentation
1. Document testing strategy and approach
2. Include examples of how to write tests
3. Document test data setup and teardown
4. Document performance test scenarios
5. Document how to run tests locally and in CI/CD

## Onboarding Documentation
1. Developer onboarding guide (CLAUDE.md already exists)
2. Environment setup instructions
3. Code style guide and conventions
4. Git workflow and branching strategy
5. PR review checklist

## Inline Documentation Best Practices
- Use clear, concise variable and method names (self-documenting code)
- Prefer descriptive names over comments
- Use constants for magic numbers with meaningful names
- Group related code with regions only when absolutely necessary
- Keep methods short and focused (Single Responsibility Principle)

## Documentation Tools
1. Use Mermaid for diagrams in Markdown
2. Use DocFX or similar for API documentation generation
3. Keep documentation close to code (in same repository)
4. Use Markdown for all documentation (not Word docs)
5. Version documentation with code (same commits)

## Documentation Review
1. Review documentation during code reviews
2. Ensure documentation is accurate and up-to-date
3. Check for broken links in documentation
4. Verify code examples actually work
5. Ensure terminology is consistent across documentation
