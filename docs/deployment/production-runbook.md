# Corelio Production Deployment Runbook

**Version:** 1.0  
**Last Updated:** 2026-04-15  
**Audience:** DevOps engineers, on-call engineers

---

## 1. Prerequisites

### Azure Resources (must be provisioned before first deploy — see TASK-10.4.1–10.4.7)

| Resource | Purpose | Expected Name |
|----------|---------|---------------|
| Azure Database for PostgreSQL Flexible Server | Application database | `corelio-pg-prod` |
| Azure Cache for Redis | Session/search cache | `corelio-redis-prod` |
| Azure Key Vault | CSD certificates, secrets | `corelio-kv-prod` |
| Azure Container Registry | Docker image storage | `corelioregistry.azurecr.io` |
| Azure Container Apps (WebAPI) | API hosting | `corelio-api-prod` |
| Azure Container Apps (BlazorApp) | UI hosting | `corelio-blazor-prod` |
| Application Insights | Telemetry & monitoring | `corelio-appinsights-prod` |

### GitHub Secrets (must be set in repo → Settings → Secrets and variables → Actions)

```
AZURE_CREDENTIALS              # Service principal JSON (az ad sp create-for-rbac)
AZURE_SUBSCRIPTION_ID
AZURE_RESOURCE_GROUP           # e.g. corelio-prod-rg
AZURE_CONTAINER_APP_NAME_API   # corelio-api-prod
AZURE_CONTAINER_APP_NAME_BLAZOR# corelio-blazor-prod
DOCKER_HUB_USERNAME            # dacanetdev
DOCKER_HUB_TOKEN
SONAR_TOKEN                    # SonarCloud analysis
SNYK_TOKEN                     # Snyk security scan
```

### Environment Variables (set on Container App via portal or CLI — no .env files)

**WebAPI Container App:**
```
ConnectionStrings__corelioDb=<postgres-connection-string>
ConnectionStrings__redis=<redis-connection-string>
JwtSettings__Secret=<jwt-secret-from-keyvault>
JwtSettings__Issuer=Corelio
JwtSettings__Audience=Corelio.Client
JwtSettings__ExpiryMinutes=60
APPLICATIONINSIGHTS_CONNECTION_STRING=<app-insights-connection-string>
ASPNETCORE_ENVIRONMENT=Production
```

**BlazorApp Container App:**
```
ApiSettings__BaseUrl=https://corelio-api-prod.<region>.azurecontainerapps.io
APPLICATIONINSIGHTS_CONNECTION_STRING=<app-insights-connection-string>
ASPNETCORE_ENVIRONMENT=Production
```

---

## 2. First-Time Production Deployment

### Step 1 — Build and push Docker images

```bash
# Build from repo root
docker build -f src/Presentation/Corelio.WebAPI/Dockerfile \
  -t corelioregistry.azurecr.io/corelio-api:latest \
  -t corelioregistry.azurecr.io/corelio-api:$(git rev-parse --short HEAD) .

docker build -f src/Presentation/Corelio.BlazorApp/Dockerfile \
  -t corelioregistry.azurecr.io/corelio-blazor:latest \
  -t corelioregistry.azurecr.io/corelio-blazor:$(git rev-parse --short HEAD) .

# Push to registry (requires az acr login first)
az acr login --name corelioregistry
docker push corelioregistry.azurecr.io/corelio-api:latest
docker push corelioregistry.azurecr.io/corelio-blazor:latest
```

### Step 2 — Apply database migrations

```bash
# Run migrations against production DB from dev machine or CI
ASPNETCORE_ENVIRONMENT=Production \
ConnectionStrings__corelioDb="<prod-connection-string>" \
dotnet ef database update \
  --project src/Infrastructure/Corelio.Infrastructure \
  --startup-project src/Presentation/Corelio.WebAPI
```

### Step 3 — Deploy Container Apps

```bash
az containerapp update \
  --name corelio-api-prod \
  --resource-group corelio-prod-rg \
  --image corelioregistry.azurecr.io/corelio-api:latest

az containerapp update \
  --name corelio-blazor-prod \
  --resource-group corelio-prod-rg \
  --image corelioregistry.azurecr.io/corelio-blazor:latest
```

### Step 4 — Verify deployment

```bash
# Run smoke tests (see docs/testing/smoke-test.sh)
BASE_URL=https://corelio-api-prod.<region>.azurecontainerapps.io \
bash docs/testing/smoke-test.sh
```

---

## 3. Routine Deployment (CI/CD — Automated)

All deployments to `main` are handled automatically by `.github/workflows/ci-cd.yml`:

1. Push/merge to `main`
2. CI runs: build → unit tests → integration tests → SonarQube → Snyk scan
3. Docker images built and pushed to Docker Hub (`dacanetdev/corelio-api`, `dacanetdev/corelio-blazor`)
4. `deploy-production` job runs database migrations, then updates Container Apps
5. Smoke tests run against `/health` endpoints
6. GitHub release created with automated release notes

**No manual steps required for routine releases.**

---

## 4. Rollback Procedure

### Option A — Revert to previous image tag (fastest)

```bash
# Find previous commit SHA
git log --oneline -5

# Roll back API to a specific image tag
az containerapp update \
  --name corelio-api-prod \
  --resource-group corelio-prod-rg \
  --image corelioregistry.azurecr.io/corelio-api:<previous-sha>

az containerapp update \
  --name corelio-blazor-prod \
  --resource-group corelio-prod-rg \
  --image corelioregistry.azurecr.io/corelio-blazor:<previous-sha>
```

### Option B — Revert migration (if schema change was applied)

```bash
# Target the migration before the problematic one
dotnet ef database update <PreviousMigrationName> \
  --project src/Infrastructure/Corelio.Infrastructure \
  --startup-project src/Presentation/Corelio.WebAPI
```

> ⚠️ Only revert migrations if they are non-destructive (no data loss). For destructive rollbacks, restore from backup.

### Option C — Restore from database backup

1. Open Azure Portal → PostgreSQL Flexible Server → Backups
2. Select the backup point-in-time before the incident
3. Restore to a new server instance
4. Update the `ConnectionStrings__corelioDb` secret to point to restored server
5. Redeploy Container Apps with `latest` image

---

## 5. Post-Deployment Verification

After every deployment run:

```bash
# Health checks
curl -f https://corelio-api-prod.<region>.azurecontainerapps.io/health
curl -f https://corelio-api-prod.<region>.azurecontainerapps.io/alive

# Full smoke test
BASE_URL=https://corelio-api-prod.<region>.azurecontainerapps.io \
bash docs/testing/smoke-test.sh
```

Expected: all checks return PASS. If any fail, initiate rollback (Section 4).

---

## 6. Monitoring & Alerts

| Dashboard | URL | Purpose |
|-----------|-----|---------|
| Application Insights | Azure Portal → corelio-appinsights-prod | Error rates, response times, live metrics |
| Container Apps | Azure Portal → Container Apps → Metrics | CPU, memory, replica count |
| PostgreSQL | Azure Portal → corelio-pg-prod → Monitoring | Connections, query performance |

**Key metrics to watch:**
- API error rate > 1% → investigate
- p95 response time > 500ms → investigate
- DB connection count > 80% of max → scale up

---

## 7. On-Call Escalation

| Severity | Definition | Response Time | Contact |
|----------|-----------|---------------|---------|
| P0 — Production down | All users affected, zero availability | 15 min | [fill in] |
| P1 — Partial outage | Major feature broken, workaround exists | 1 hour | [fill in] |
| P2 — Degraded | Performance issue, minor feature broken | Next business day | [fill in] |

**To report an incident:** [fill in — Slack channel, PagerDuty, etc.]
