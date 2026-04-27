# Sprint 11: Production Infrastructure (Fly.io)

**Goal:** Deploy Corelio to Fly.io free tier as a replacement for unavailable Azure Container Apps, enabling UAT and production access without Azure dependencies.

**Duration:** ~1-2 days estimated
**Status:** 🟢 Complete
**Started:** 2026-04-27
**Total Story Points:** 8 pts

> **Prerequisites:** Sprint 10 complete | Docker Hub images published | Neon + Upstash accounts created

---

## User Story 11.1: Fly.io Deployment
**As the DevOps engineer, I want both containers deployed to Fly.io so that UAT and production access is available without Azure dependencies.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-11.1.1 | Create `fly.toml` for WebAPI (release_command --migrate, /health + /alive checks, 256 MB, mia region) | `feature/US-11.1-TASK-1-2-4-flyio-config` | 🟢 | PR #87 — `src/Presentation/Corelio.WebAPI/fly.toml` |
| TASK-11.1.2 | Create `fly.toml` for BlazorApp (min_machines_running=1 for SignalR, /health check, internal API URL) | `feature/US-11.1-TASK-1-2-4-flyio-config` | 🟢 | PR #87 — `src/Presentation/Corelio.BlazorApp/fly.toml` |
| TASK-11.1.3 | Fix Aspire Redis hard-wire in `DependencyInjection.cs` + add `--migrate` flag to WebAPI + add `/health` to BlazorApp | `feature/US-11.1-TASK-3-code-fixes` | 🟢 | PR #86 — conditional Redis (explicit → Aspire → memory), --migrate early-exit, GET /health |
| TASK-11.1.4 | `appsettings.Production.json` for BlazorApp | — | 🟢 | Dropped — `.gitignore` excludes `appsettings.*.json`; `ApiSettings__BaseUrl` set via env var in BlazorApp `fly.toml` instead |
| TASK-11.1.5 | Update CI/CD — replace Azure deploy jobs with `deploy-fly` job using flyctl | `feature/US-11.1-TASK-5-cicd-flyio` | 🟢 | PR #88 — removed `deploy-staging`/`deploy-production`, added `deploy-fly` |
| TASK-11.1.6 | Manual one-time setup: `fly apps create`, secrets, first deploy, `FLY_API_TOKEN` GitHub secret | — | 🔴 Pending | Requires developer to run CLI commands (see instructions below) |

**Acceptance Criteria:**
- [x] `fly.toml` created for both apps
- [x] Aspire Redis hard-wire fixed — WebAPI starts on Fly.io without Aspire
- [x] `--migrate` flag implemented — runs EF Core migrations as Fly.io release_command
- [x] `/health` endpoint added to BlazorApp
- [x] CI/CD deploys to Fly.io on merge to `main`
- [ ] First manual deploy completed (TASK-11.1.6 — requires developer CLI access)
- [ ] `FLY_API_TOKEN` added to GitHub Secrets

---

## Manual One-Time Setup (TASK-11.1.6)

Run these commands once before the first automated deploy can succeed:

```bash
# 1. Install flyctl and authenticate
curl -L https://fly.io/install.sh | sh
fly auth login

# 2. Create the apps
fly apps create corelio-api --org personal
fly apps create corelio-blazor --org personal

# 3. Set WebAPI secrets (get connection strings from Neon + Upstash dashboards)
fly secrets set --app corelio-api \
  ConnectionStrings__corelioDb="postgresql://user:pass@ep-xxx.neon.tech/coreliodb?sslmode=require" \
  ConnectionStrings__redis="rediss://default:password@xxx.upstash.io:6380" \
  JwtSettings__Secret="$(openssl rand -base64 32)" \
  JwtSettings__Issuer="Corelio" \
  JwtSettings__Audience="Corelio.Client" \
  JwtSettings__ExpiryMinutes="60"

# 4. First-time deploy (from repo root)
fly deploy --app corelio-api \
  --config src/Presentation/Corelio.WebAPI/fly.toml \
  --image dacanetdev/corelio-api:latest

fly deploy --app corelio-blazor \
  --config src/Presentation/Corelio.BlazorApp/fly.toml \
  --image dacanetdev/corelio-blazor:latest

# 5. Add FLY_API_TOKEN to GitHub Secrets
fly auth token
# → GitHub repo Settings > Secrets and variables > Actions > New secret → FLY_API_TOKEN
```

---

## Verification Checklist

```bash
# WebAPI health
curl -f https://corelio-api.fly.dev/health
curl -f https://corelio-api.fly.dev/alive

# BlazorApp health
curl -f https://corelio-blazor.fly.dev/health

# Login works
curl -X POST https://corelio-api.fly.dev/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@demo.com","password":"Demo1234!"}' | jq .

# Fly.io status
fly status --app corelio-api
fly status --app corelio-blazor

# Migration log
fly logs --app corelio-api
```

---

## Stack Summary

| Component | Provider | Tier |
|-----------|----------|------|
| WebAPI container | Fly.io | shared-cpu-1x, 256 MB (free) |
| BlazorApp container | Fly.io | shared-cpu-1x, 256 MB (free) |
| PostgreSQL 16 | Neon | 0.5 GB, serverless (free) |
| Redis cache | Upstash | 10K cmd/day, 256 MB (free) |
| Observability | Azure App Insights | 5 GB/mo (free) — optional |

## Sprint 11 Summary

| Story | SP | Status |
|-------|----|--------|
| US-11.1: Fly.io Deployment | 8 | 🟢 Complete (code done; manual deploy pending) |
| **Total** | **8** | |
