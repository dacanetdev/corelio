# Corelio POS Load Test — Results & Interpretation Guide

## Performance Targets

| Endpoint | Target (p95) | Rationale |
|----------|-------------|-----------|
| `GET /api/v1/pos/search` | < 300 ms | Cashier UX: search feels instant under 300 ms |
| `POST /api/v1/sales/{id}/complete` | < 3 s | Checkout UX: customer wait tolerance |

## How to Run

### Prerequisites
1. Install k6: https://k6.io/docs/getting-started/installation/
2. Start the Corelio Aspire stack: `dotnet run --project src/Aspire/Corelio.AppHost`
3. Obtain a JWT token for a test tenant user (via `POST /api/v1/auth/login`)
4. Pre-create a pending sale to use for the complete-sale scenario

### Commands

```bash
# Full load test (100 VUs, 4 minutes)
k6 run \
  --env K6_BASE_URL=http://localhost:5000 \
  --env K6_JWT_TOKEN=<your-jwt-token> \
  --env K6_SALE_ID=<pending-sale-uuid> \
  docs/testing/load-test-k6.js

# Quick smoke test (5 VUs, 30 seconds)
k6 run \
  --vus 5 --duration 30s \
  --env K6_BASE_URL=http://localhost:5000 \
  --env K6_JWT_TOKEN=<token> \
  docs/testing/load-test-k6.js
```

### Against staging environment

```bash
k6 run \
  --env K6_BASE_URL=https://api.corelio.com.mx \
  --env K6_JWT_TOKEN=<staging-token> \
  --env K6_SALE_ID=<staging-sale-uuid> \
  docs/testing/load-test-k6.js
```

---

## Expected Results (Post US-10.2 Optimizations)

| Metric | Expected Value |
|--------|---------------|
| `pos_search_duration` p50 | < 50 ms (cache hit) / < 150 ms (cache miss) |
| `pos_search_duration` p95 | < 300 ms |
| `pos_search_duration` p99 | < 500 ms |
| `sale_complete_duration` p50 | < 500 ms |
| `sale_complete_duration` p95 | < 3,000 ms |
| `pos_search_errors` rate | < 1% |
| `sale_complete_errors` rate | < 1% |
| `http_req_failed` rate | < 1% |

---

## Optimizations Applied (US-10.2)

These changes should be in place before running the load test:

| Optimization | Impact |
|--------------|--------|
| Redis cache for POS search (5-min TTL, per-tenant) | ~90% reduction in DB load for repeated searches |
| Cache invalidation on product create/update | Ensures fresh results after catalog changes |
| `AsNoTracking()` on all read-only queries | Eliminates EF change-tracking overhead on list endpoints |
| EF compiled query for `GetDefaultWarehouseAsync` | Reduces LINQ translation overhead on every POS search |
| `ix_sales_tenant_created_at` composite index | Faster date-range filters in sales history |
| `ix_inventory_items_warehouse_id` index | Faster warehouse-scoped inventory lookups |

---

## Interpreting Results

### Green (targets met)
- All thresholds pass in k6 output
- p95 search < 300 ms, p95 complete < 3 s
- Error rate < 1%

### Yellow (investigate)
- p95 search between 300–500 ms → check Redis connectivity; cache may not be warming
- p95 complete between 3–5 s → check DB connection pool; review `GetNextFolioNumberAsync` performance

### Red (targets not met)
- p95 search > 500 ms → Redis not running or cache keys colliding; check `pos:ver:{tenantId}` key
- p95 complete > 5 s → likely DB contention; check `ix_sales_tenant_folio` unique constraint; review sale creation under concurrency
- Error rate > 1% → check WebAPI logs for 5xx responses; review Aspire dashboard traces

---

## Baseline vs. Optimized Comparison

> Note: This table should be filled in after actually running the load test against both
> the pre-US-10.2 and post-US-10.2 builds.

| Metric | Baseline (pre-US-10.2) | Optimized (post-US-10.2) | Improvement |
|--------|------------------------|--------------------------|-------------|
| Search p50 | TBD | TBD | TBD |
| Search p95 | TBD | TBD | TBD |
| Complete p50 | TBD | TBD | TBD |
| Complete p95 | TBD | TBD | TBD |
| DB queries/search | 3 (products + warehouse + inventory) | 0 (cache hit) / 3 (miss) | ~90% cache hit rate |

---

## Notes

- The complete-sale scenario uses a single pre-created sale. For a more realistic test,
  extend the script to create a new sale per virtual user in the `setup()` function.
- Integration tests (`Corelio.Integration.Tests`) require Docker and test tenant isolation,
  not raw throughput. Run the k6 script against a running instance for throughput testing.
- To reset the Redis cache between test runs: `redis-cli FLUSHDB` (development only).
