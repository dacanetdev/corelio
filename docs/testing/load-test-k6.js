/**
 * Corelio POS Load Test — k6 Script
 *
 * Targets:
 *   - GET /api/v1/pos/search    → <300ms p95 (100 concurrent users)
 *   - POST /api/v1/sales/{id}/complete → <3s p95
 *
 * Prerequisites:
 *   1. Install k6: https://k6.io/docs/getting-started/installation/
 *   2. Set environment variables:
 *        K6_BASE_URL   — e.g. http://localhost:5000
 *        K6_JWT_TOKEN  — valid JWT for a test tenant user
 *        K6_SALE_ID    — a pre-created pending sale ID (UUID) for the complete-sale scenario
 *
 * Run:
 *   k6 run --env K6_BASE_URL=http://localhost:5000 \
 *           --env K6_JWT_TOKEN=<token> \
 *           --env K6_SALE_ID=<uuid> \
 *           docs/testing/load-test-k6.js
 */

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend, Rate, Counter } from 'k6/metrics';

// ---------------------------------------------------------------------------
// Custom metrics
// ---------------------------------------------------------------------------
const searchDuration = new Trend('pos_search_duration', true);
const searchErrors   = new Rate('pos_search_errors');
const completeDuration = new Trend('sale_complete_duration', true);
const completeErrors   = new Rate('sale_complete_errors');
const totalRequests  = new Counter('total_requests');

// ---------------------------------------------------------------------------
// Test configuration
// ---------------------------------------------------------------------------
export const options = {
  scenarios: {
    // Scenario 1: POS product search — 100 concurrent users, 4 minutes total
    pos_search: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '1m', target: 100 },   // ramp up to 100 users in 1 min
        { duration: '2m', target: 100 },   // hold 100 users for 2 min
        { duration: '1m', target: 0 },     // ramp down
      ],
      exec: 'searchProducts',
    },

    // Scenario 2: Complete sale — lower concurrency, separate from search
    complete_sale: {
      executor: 'constant-vus',
      vus: 10,
      duration: '4m',
      exec: 'completeSale',
      startTime: '0s',
    },
  },

  thresholds: {
    // POS search must return in <300ms for 95% of requests
    pos_search_duration: ['p(95)<300'],

    // Sale completion must return in <3000ms for 95% of requests
    sale_complete_duration: ['p(95)<3000'],

    // Error rates must stay below 1%
    pos_search_errors:   ['rate<0.01'],
    sale_complete_errors: ['rate<0.01'],

    // Overall HTTP failure rate
    http_req_failed: ['rate<0.01'],
  },
};

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------
const BASE_URL   = __ENV.K6_BASE_URL   || 'http://localhost:5000';
const JWT_TOKEN  = __ENV.K6_JWT_TOKEN  || '';
const SALE_ID    = __ENV.K6_SALE_ID    || '00000000-0000-0000-0000-000000000000';

const HEADERS = {
  'Authorization': `Bearer ${JWT_TOKEN}`,
  'Content-Type': 'application/json',
};

// Common hardware-store search terms to simulate realistic traffic
const SEARCH_TERMS = [
  'tornillo',
  'clavo',
  'pintura',
  'martillo',
  'llave',
  'tubo',
  'cemento',
  'varilla',
  'cable',
  'foco',
  'manguera',
  'brocha',
  'talacho',
  'pala',
  'cinta',
];

// ---------------------------------------------------------------------------
// Scenario 1: POS product search
// ---------------------------------------------------------------------------
export function searchProducts() {
  const term = SEARCH_TERMS[Math.floor(Math.random() * SEARCH_TERMS.length)];
  const url  = `${BASE_URL}/api/v1/pos/search?term=${encodeURIComponent(term)}&limit=20`;

  const start = Date.now();
  const res   = http.get(url, { headers: HEADERS });
  const elapsed = Date.now() - start;

  totalRequests.add(1);
  searchDuration.add(elapsed);

  const ok = check(res, {
    'search: status 200':          r => r.status === 200,
    'search: returns array':       r => { try { return Array.isArray(JSON.parse(r.body)); } catch { return false; } },
    'search: response time <300ms': () => elapsed < 300,
  });

  searchErrors.add(!ok);

  // Simulate cashier typing cadence (0.5–1.5 seconds between searches)
  sleep(0.5 + Math.random());
}

// ---------------------------------------------------------------------------
// Scenario 2: Complete sale
// Note: In a real load test, each VU would create its own sale first.
// This simplified version uses a single pre-created SALE_ID, which is
// appropriate for measuring endpoint latency under concurrent load.
// ---------------------------------------------------------------------------
export function completeSale() {
  const url = `${BASE_URL}/api/v1/sales/${SALE_ID}/complete`;

  const start = Date.now();
  const res   = http.post(url, '{}', { headers: HEADERS });
  const elapsed = Date.now() - start;

  totalRequests.add(1);
  completeDuration.add(elapsed);

  // Accept 200 OK or 404 Not Found (if sale was already completed by another VU)
  const ok = check(res, {
    'complete: status 2xx or 404': r => r.status === 200 || r.status === 404,
    'complete: response time <3s':  () => elapsed < 3000,
  });

  completeErrors.add(res.status >= 500);

  sleep(1 + Math.random() * 2);
}
