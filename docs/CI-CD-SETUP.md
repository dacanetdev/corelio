# CI/CD Setup Guide

Complete Continuous Integration and Continuous Deployment setup for Corelio project.

---

## Overview

The CI/CD pipeline is built with GitHub Actions and includes:
- ✅ Automated building and testing
- ✅ Code quality analysis (SonarQube)
- ✅ Security scanning (Snyk, Trivy)
- ✅ Code coverage reporting
- ✅ Docker image building
- ✅ Automated deployment to staging and production
- ✅ Pull request validation
- ✅ Dependency updates (Dependabot)
- ✅ Scheduled nightly builds and security audits

---

## Workflows

### 1. Main CI/CD Pipeline (`ci-cd.yml`)

**Triggered on:** Push to `main`/`develop`, Pull Requests

**Jobs:**
1. **Build and Test**
   - Builds the solution
   - Runs unit tests (Category=Unit)
   - Runs integration tests (Category=Integration)
   - Generates code coverage report
   - Validates >70% code coverage threshold

2. **Code Quality Analysis**
   - Runs SonarQube/SonarCloud analysis
   - Checks quality gate (A rating minimum)
   - Reports code smells, bugs, vulnerabilities

3. **Security Scan**
   - Snyk vulnerability scanning
   - Trivy file system scanning
   - Dependency vulnerability check
   - Uploads results to GitHub Security

4. **Code Format Check**
   - Validates code formatting with `dotnet format`
   - Ensures consistent C# 14 style

5. **Docker Build** (main/develop only)
   - Builds Docker images for API and Blazor
   - Pushes to Docker Hub with tags (SHA + latest)
   - Uses layer caching for faster builds

6. **Deploy to Staging** (develop branch)
   - Deploys to Azure Container Apps (staging)
   - Runs database migrations
   - Performs smoke tests

7. **Deploy to Production** (main branch)
   - Deploys to Azure Container Apps (production)
   - Runs database migrations
   - Performs smoke tests
   - Creates GitHub release

8. **Notify Team**
   - Sends Slack notification with deployment status

---

### 2. Pull Request Checks (`pr-checks.yml`)

**Triggered on:** Pull Request opened/updated

**Jobs:**
1. **PR Validation**
   - Validates PR title follows conventional commits format
   - Checks for breaking changes
   - Warns if too many files changed (>100)

2. **Build PR**
   - Builds the solution to ensure no compile errors

3. **Test PR**
   - Runs all tests
   - Comments test results on PR

4. **Multi-Tenancy Security Check**
   - Verifies new entities implement ITenantEntity
   - Checks for potential tenant isolation issues
   - Warns about IgnoreQueryFilters() usage

5. **Code Review Checklist**
   - Posts automated code review checklist comment
   - Covers architecture, security, code quality, testing

6. **PR Size Labeling**
   - Automatically labels PR by size (xs/s/m/l/xl)

---

### 3. Scheduled Tasks (`scheduled-tasks.yml`)

**Triggered on:** Daily at 2 AM UTC, Manual trigger

**Jobs:**
1. **Nightly Build**
   - Full build and test run (including slow tests)
   - Retains results for 30 days

2. **Performance Benchmarks**
   - Runs BenchmarkDotNet tests
   - Compares with baseline performance
   - Fails if >10% degradation

3. **Security Audit**
   - Full OWASP dependency check
   - Vulnerability scanning
   - Generates security report

4. **Code Metrics**
   - Calculates lines of code
   - File counts
   - Commit activity
   - Generates metrics report

5. **Database Backup Test**
   - Tests backup scripts

6. **License Compliance Check**
   - Scans all dependencies for license compliance
   - Generates license report

7. **Stale Issue/PR Check**
   - Marks issues/PRs stale after 60 days of inactivity
   - Closes after 7 days of being stale

8. **Notify Results**
   - Sends Slack notification with nightly results

---

### 4. Dependabot (`dependabot.yml`)

**Updates:** Weekly on Mondays at 9 AM

**Monitors:**
- ✅ NuGet packages (weekly)
- ✅ GitHub Actions (weekly)
- ✅ Docker base images (weekly)

**Configuration:**
- Opens up to 10 PRs for .NET dependencies
- Opens up to 5 PRs for GitHub Actions
- Ignores major version updates (manual review needed)
- Auto-assigns to maintainers
- Labels: `dependencies`, `dotnet`, `github-actions`, `docker`

---

## Required GitHub Secrets

Configure these secrets in GitHub Settings → Secrets and variables → Actions:

### SonarQube/SonarCloud
```
SONAR_TOKEN=your-sonarcloud-token
```

### Snyk
```
SNYK_TOKEN=your-snyk-token
```

### Docker Hub
```
DOCKER_USERNAME=dacanetdev
DOCKER_PASSWORD=your-docker-password-or-token
```

### Azure Credentials
```
AZURE_CREDENTIALS_STAGING='{
  "clientId": "...",
  "clientSecret": "...",
  "subscriptionId": "...",
  "tenantId": "..."
}'

AZURE_CREDENTIALS_PRODUCTION='{
  "clientId": "...",
  "clientSecret": "...",
  "subscriptionId": "...",
  "tenantId": "..."
}'
```

### Slack Notifications
```
SLACK_WEBHOOK_URL=https://hooks.slack.com/services/YOUR/WEBHOOK/URL
```

---

## Branch Strategy

### Main Branch (`main`)
- **Protected:** Requires PR and reviews
- **Triggers:** Full CI/CD pipeline → Production deployment
- **Auto-deploy:** Yes (to production after tests pass)

### Develop Branch (`develop`)
- **Protected:** Requires PR and reviews
- **Triggers:** Full CI/CD pipeline → Staging deployment
- **Auto-deploy:** Yes (to staging after tests pass)

### Feature Branches (`feature/*`)
- **Naming:** `feature/description-of-feature`
- **Triggers:** Build and test only
- **Merge target:** `develop`

### Hotfix Branches (`hotfix/*`)
- **Naming:** `hotfix/description-of-fix`
- **Triggers:** Build and test only
- **Merge target:** `main` and `develop`

---

## Conventional Commits Format

All commit messages and PR titles must follow this format:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding or updating tests
- `chore`: Maintenance tasks
- `ci`: CI/CD changes
- `build`: Build system changes

### Examples
```
feat(pos): add barcode scanning support
fix(auth): correct JWT token expiration
docs(readme): update installation instructions
refactor(domain): simplify product entity
perf(query): optimize product search query
test(integration): add multi-tenancy isolation tests
chore(deps): update EF Core to 10.0.1
ci(workflow): add SonarQube analysis
```

### Breaking Changes
```
feat(api): redesign authentication flow

BREAKING CHANGE: Authentication endpoint changed from /api/auth/login to /api/v2/auth/login
```

---

## Quality Gates

### Build Quality Gate
Must pass for merge:
- ✅ All tests passing
- ✅ Code coverage >70%
- ✅ No build errors or warnings
- ✅ Code format check passes

### Code Quality Gate (SonarQube)
- ✅ Quality Gate: A rating
- ✅ No blocker or critical issues
- ✅ <3% code duplication
- ✅ Technical debt ratio <5%

### Security Gate
- ✅ No high or critical vulnerabilities
- ✅ No hardcoded secrets detected
- ✅ Dependencies up to date (no known vulnerabilities)

### Multi-Tenancy Gate
- ✅ All business entities implement ITenantEntity
- ✅ No tenant_id from client input
- ✅ Isolation tests passing

---

## Deployment Environments

### Staging
- **URL:** https://staging.corelio.app
- **API:** https://staging-api.corelio.app
- **Triggered by:** Push to `develop`
- **Azure:** Container Apps (staging resource group)
- **Database:** PostgreSQL (staging)
- **Purpose:** Testing and QA

### Production
- **URL:** https://corelio.app
- **API:** https://api.corelio.app
- **Triggered by:** Push to `main`
- **Azure:** Container Apps (production resource group)
- **Database:** PostgreSQL (production with replicas)
- **Purpose:** Live customer environment

---

## Manual Workflow Triggers

Some workflows can be triggered manually:

### Trigger Nightly Build Manually
```bash
gh workflow run scheduled-tasks.yml
```

### Trigger Deployment Manually
```bash
# Deploy to staging
gh workflow run ci-cd.yml --ref develop

# Deploy to production
gh workflow run ci-cd.yml --ref main
```

---

## Monitoring and Observability

### GitHub Actions
- View workflow runs: https://github.com/dacanetdev/corelio/actions
- Download artifacts (test results, coverage reports)
- View logs for debugging

### SonarCloud
- Dashboard: https://sonarcloud.io/project/overview?id=corelio
- Quality gates, code smells, security hotspots

### Azure Container Apps
- Monitor via Azure Portal
- Application Insights for telemetry
- Aspire Dashboard: http://localhost:15888 (local dev)

### Slack Notifications
- Deployment status
- Nightly build results
- Security alerts

---

## Troubleshooting

### Build Fails with "Workload not found"
**Solution:** Ensure .NET Aspire workload is installed:
```bash
dotnet workload update
dotnet workload install aspire
```

### Tests Fail with Connection String Error
**Solution:** Check GitHub secrets are configured correctly

### SonarQube Analysis Fails
**Solution:** Verify SONAR_TOKEN secret is valid

### Docker Push Fails
**Solution:** Check DOCKER_USERNAME and DOCKER_PASSWORD secrets

### Deployment Fails
**Solution:** Check AZURE_CREDENTIALS secrets and resource group names

---

## Best Practices

1. **Always create PR:** Never push directly to main/develop
2. **Write tests:** Aim for >70% coverage
3. **Follow conventions:** Use conventional commit format
4. **Keep PRs small:** <500 lines of code changed
5. **Review checklist:** Use automated checklist in PR
6. **Monitor builds:** Check GitHub Actions dashboard regularly
7. **Fix broken builds:** Immediately fix failing CI/CD pipelines
8. **Update dependencies:** Review Dependabot PRs weekly
9. **Security first:** Never commit secrets, always use GitHub Secrets
10. **Document changes:** Update docs when changing CI/CD

---

## Local Testing

Test CI/CD workflows locally using [act](https://github.com/nektos/act):

```bash
# Install act
brew install act  # macOS
choco install act # Windows

# Run a specific workflow
act -j build-and-test

# Run PR checks
act pull_request

# List available jobs
act -l
```

---

## Performance Targets

CI/CD pipelines should meet these targets:

| Stage | Target Time | Acceptable |
|-------|-------------|------------|
| Build | <3 minutes | <5 minutes |
| Unit Tests | <2 minutes | <5 minutes |
| Integration Tests | <5 minutes | <10 minutes |
| Code Quality | <5 minutes | <10 minutes |
| Security Scan | <3 minutes | <7 minutes |
| Docker Build | <5 minutes | <10 minutes |
| Deployment | <5 minutes | <10 minutes |
| **Total Pipeline** | **<25 minutes** | **<45 minutes** |

---

## Future Improvements

- [ ] Add E2E tests with Playwright
- [ ] Implement blue-green deployments
- [ ] Add canary deployments for production
- [ ] Implement automated rollback on errors
- [ ] Add performance regression testing
- [ ] Implement chaos engineering tests
- [ ] Add mobile app CI/CD (Phase 2)
- [ ] Implement GitOps with ArgoCD
- [ ] Add infrastructure as code (Terraform/Bicep)
- [ ] Implement automated security patching

---

**Last Updated:** 2025-12-22
**Maintained By:** DevOps Team
