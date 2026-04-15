#!/usr/bin/env bash
# smoke-test.sh — Post-deployment smoke tests for Corelio
#
# Usage:
#   bash docs/testing/smoke-test.sh
#   BASE_URL=https://corelio-api-prod.<region>.azurecontainerapps.io bash docs/testing/smoke-test.sh
#   ADMIN_EMAIL=admin@demo.com ADMIN_PASSWORD=Admin123! BASE_URL=http://localhost:7001 bash docs/testing/smoke-test.sh

set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost:7001}"
ADMIN_EMAIL="${ADMIN_EMAIL:-admin@ferreteria-demo.com}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-Admin123!}"

PASS=0
FAIL=0

green='\033[0;32m'
red='\033[0;31m'
reset='\033[0m'

check() {
    local name="$1"
    local result="$2"
    if [ "$result" = "0" ]; then
        echo -e "${green}PASS${reset}  $name"
        PASS=$((PASS + 1))
    else
        echo -e "${red}FAIL${reset}  $name"
        FAIL=$((FAIL + 1))
    fi
}

echo ""
echo "========================================="
echo "  Corelio Smoke Tests"
echo "  Target: $BASE_URL"
echo "========================================="
echo ""

# ── Health checks ─────────────────────────────────────────────────────────────

HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/health" 2>/dev/null || echo "000")
[ "$HTTP_STATUS" = "200" ] && RES=0 || RES=1
check "GET /health → 200" $RES

HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/alive" 2>/dev/null || echo "000")
[ "$HTTP_STATUS" = "200" ] && RES=0 || RES=1
check "GET /alive → 200" $RES

# ── Authentication ─────────────────────────────────────────────────────────────

LOGIN_RESPONSE=$(curl -s -o /tmp/corelio_login.json -w "%{http_code}" \
    -X POST "$BASE_URL/api/v1/auth/login" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$ADMIN_EMAIL\",\"password\":\"$ADMIN_PASSWORD\"}" 2>/dev/null || echo "000")

[ "$LOGIN_RESPONSE" = "200" ] && RES=0 || RES=1
check "POST /api/v1/auth/login → 200" $RES

# Extract token (requires jq; falls back gracefully)
TOKEN=""
if command -v jq &>/dev/null; then
    TOKEN=$(jq -r '.value.accessToken // .accessToken // empty' /tmp/corelio_login.json 2>/dev/null || true)
fi

if [ -z "$TOKEN" ]; then
    echo "  (jq not found or token field not matched — skipping authenticated tests)"
    TOKEN_AVAILABLE=0
else
    TOKEN_AVAILABLE=1
fi

# ── Authenticated endpoints ────────────────────────────────────────────────────

if [ "$TOKEN_AVAILABLE" = "1" ]; then
    HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" \
        "$BASE_URL/api/v1/products?page=1&pageSize=10" \
        -H "Authorization: Bearer $TOKEN" 2>/dev/null || echo "000")
    [ "$HTTP_STATUS" = "200" ] && RES=0 || RES=1
    check "GET /api/v1/products → 200" $RES

    HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" \
        "$BASE_URL/api/v1/pos/search?term=tornillo" \
        -H "Authorization: Bearer $TOKEN" 2>/dev/null || echo "000")
    [ "$HTTP_STATUS" = "200" ] && RES=0 || RES=1
    check "GET /api/v1/pos/search?term=tornillo → 200" $RES

    HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" \
        "$BASE_URL/api/v1/sales?page=1&pageSize=10" \
        -H "Authorization: Bearer $TOKEN" 2>/dev/null || echo "000")
    [ "$HTTP_STATUS" = "200" ] && RES=0 || RES=1
    check "GET /api/v1/sales → 200" $RES
else
    echo "  SKIP  GET /api/v1/products (no token)"
    echo "  SKIP  GET /api/v1/pos/search (no token)"
    echo "  SKIP  GET /api/v1/sales (no token)"
fi

# ── Summary ────────────────────────────────────────────────────────────────────

echo ""
echo "========================================="
echo "  Results: $PASS passed, $FAIL failed"
echo "========================================="
echo ""

rm -f /tmp/corelio_login.json

if [ "$FAIL" -gt 0 ]; then
    exit 1
fi
