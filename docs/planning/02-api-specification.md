# Corelio - REST API Specification

## Document Information
- **Project:** Corelio Multi-Tenant SaaS ERP
- **API Version:** v1
- **Base URL:** `https://api.corelio.com.mx/api/v1`
- **Protocol:** HTTPS Only
- **Authentication:** JWT Bearer Token
- **Date:** 2025-12-20

---

## Table of Contents
1. [API Overview](#api-overview)
2. [Authentication](#authentication)
3. [Error Handling](#error-handling)
4. [Pagination](#pagination)
5. [API Endpoints](#api-endpoints)
6. [Webhook Events](#webhook-events)

---

## API Overview

### Design Principles
- **RESTful:** Resource-oriented URLs, HTTP verbs
- **Stateless:** No session state on server
- **Versioned:** URL path versioning (`/api/v1`)
- **JSON:** All requests and responses in JSON
- **Multi-Tenant:** Automatic tenant isolation via JWT

### Request Headers
```http
Authorization: Bearer {jwt_token}
Content-Type: application/json
Accept: application/json
X-Tenant-Id: {tenant_id}  # Optional, extracted from JWT
X-Request-ID: {uuid}      # Optional, for request tracking
```

### Response Headers
```http
Content-Type: application/json
X-Request-ID: {uuid}
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1640995200
```

---

## Authentication

### 1. Login
**POST** `/api/v1/auth/login`

**Request:**
```json
{
  "email": "user@ferreteria.com",
  "password": "SecurePassword123!",
  "rememberMe": false
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "expiresIn": 3600,
    "tokenType": "Bearer",
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@ferreteria.com",
      "firstName": "Juan",
      "lastName": "Pérez",
      "tenantId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
      "tenantName": "Ferretería López",
      "roles": ["Cashier"],
      "permissions": ["sales.create", "sales.view", "products.view"]
    }
  }
}
```

### 2. Refresh Token
**POST** `/api/v1/auth/refresh`

**Request:**
```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "new_access_token_here",
    "refreshToken": "new_refresh_token_here",
    "expiresIn": 3600
  }
}
```

### 3. Logout
**POST** `/api/v1/auth/logout`

**Request:**
```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
}
```

**Response:** `204 No Content`

---

## Error Handling

### Standard Error Response
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The request contains invalid data",
    "details": [
      {
        "field": "email",
        "message": "Invalid email format"
      },
      {
        "field": "password",
        "message": "Password must be at least 8 characters"
      }
    ],
    "timestamp": "2025-12-20T15:30:00Z",
    "requestId": "550e8400-e29b-41d4-a716-446655440000"
  }
}
```

### HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful GET, PUT, PATCH requests |
| 201 | Created | Successful POST creating a resource |
| 204 | No Content | Successful DELETE or action with no response body |
| 400 | Bad Request | Invalid request data |
| 401 | Unauthorized | Missing or invalid authentication |
| 403 | Forbidden | Authenticated but lacks permission |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Resource conflict (duplicate SKU, etc.) |
| 422 | Unprocessable Entity | Valid JSON but business logic error |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server-side error |
| 503 | Service Unavailable | Temporary unavailability |

### Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `VALIDATION_ERROR` | 400 | Request validation failed |
| `UNAUTHORIZED` | 401 | Authentication required |
| `FORBIDDEN` | 403 | Permission denied |
| `NOT_FOUND` | 404 | Resource not found |
| `DUPLICATE_RESOURCE` | 409 | Resource already exists |
| `TENANT_NOT_FOUND` | 404 | Tenant doesn't exist |
| `TENANT_INACTIVE` | 403 | Tenant account disabled |
| `INSUFFICIENT_INVENTORY` | 422 | Not enough stock |
| `INVOICE_STAMPING_FAILED` | 422 | CFDI stamping error |
| `RATE_LIMIT_EXCEEDED` | 429 | Too many requests |
| `INTERNAL_ERROR` | 500 | Server error |

---

## Pagination

### Query Parameters
```http
GET /api/v1/products?page=1&pageSize=20&sortBy=name&sortOrder=asc
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number (1-indexed) |
| `pageSize` | int | 20 | Items per page (max 100) |
| `sortBy` | string | id | Field to sort by |
| `sortOrder` | string | asc | Sort direction: `asc` or `desc` |

### Paginated Response
```json
{
  "success": true,
  "data": {
    "items": [ ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 150,
      "totalPages": 8,
      "hasPreviousPage": false,
      "hasNextPage": true
    }
  }
}
```

---

## API Endpoints

### Tenants

#### Register New Tenant (Public)
**POST** `/api/v1/tenants/register`

**Request:**
```json
{
  "businessName": "Ferretería López",
  "legalName": "Ferretería López S.A. de C.V.",
  "rfc": "FLO850101ABC",
  "subdomain": "ferreteria-lopez",
  "ownerFirstName": "Juan",
  "ownerLastName": "López",
  "ownerEmail": "juan@ferreteria-lopez.com",
  "ownerPassword": "SecurePass123!",
  "phone": "+52-555-1234567",
  "subscriptionPlan": "basic"
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "tenantId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "subdomain": "ferreteria-lopez",
    "accessUrl": "https://ferreteria-lopez.corelio.com.mx",
    "ownerUserId": "550e8400-e29b-41d4-a716-446655440000",
    "trialEndsAt": "2026-01-20T00:00:00Z",
    "message": "Tenant registered successfully. Please check your email to confirm your account."
  }
}
```

#### Get Current Tenant
**GET** `/api/v1/tenants/current`

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "name": "Ferretería López",
    "legalName": "Ferretería López S.A. de C.V.",
    "rfc": "FLO850101ABC",
    "subdomain": "ferreteria-lopez",
    "customDomain": null,
    "subscriptionPlan": "basic",
    "subscriptionEndsAt": "2026-12-20T00:00:00Z",
    "maxUsers": 5,
    "currentUsers": 3,
    "maxProducts": 1000,
    "currentProducts": 245,
    "isActive": true,
    "isTrial": false,
    "createdAt": "2025-01-20T10:00:00Z"
  }
}
```

---

### Products

#### List Products
**GET** `/api/v1/products`

**Query Parameters:**
- `page` (int): Page number
- `pageSize` (int): Items per page
- `search` (string): Search by name, SKU, or barcode
- `categoryId` (uuid): Filter by category
- `isActive` (bool): Filter active/inactive
- `minPrice` (decimal): Minimum price
- `maxPrice` (decimal): Maximum price
- `sortBy` (string): Field to sort by
- `sortOrder` (string): `asc` or `desc`

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-4789-0123-456789abcdef",
        "sku": "MART-001",
        "barcode": "7501234567890",
        "name": "Martillo de Garra 16 oz",
        "description": "Martillo profesional con mango de fibra de vidrio",
        "categoryId": "cat-123",
        "categoryName": "Herramientas",
        "costPrice": 150.00,
        "salePrice": 250.00,
        "taxRate": 0.16,
        "unitOfMeasure": "PCS",
        "availableQuantity": 45,
        "minStockLevel": 10,
        "isActive": true,
        "primaryImageUrl": "https://cdn.corelio.com.mx/products/martillo-001.jpg",
        "createdAt": "2025-11-15T10:00:00Z",
        "updatedAt": "2025-12-01T14:30:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 245,
      "totalPages": 13,
      "hasPreviousPage": false,
      "hasNextPage": true
    }
  }
}
```

#### Create Product
**POST** `/api/v1/products`

**Request:**
```json
{
  "sku": "TALA-001",
  "barcode": "7501234567891",
  "name": "Taladro Eléctrico 1/2\"",
  "description": "Taladro de impacto 850W con chuck de 1/2\"",
  "shortDescription": "Taladro 850W profesional",
  "categoryId": "cat-123",
  "brand": "DeWalt",
  "costPrice": 1200.00,
  "salePrice": 1800.00,
  "wholesalePrice": 1500.00,
  "taxRate": 0.16,
  "trackInventory": true,
  "unitOfMeasure": "PCS",
  "minStockLevel": 5,
  "satProductCode": "25171500",
  "satUnitCode": "H87",
  "isActive": true
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "id": "new-product-id",
    "sku": "TALA-001",
    "name": "Taladro Eléctrico 1/2\"",
    "salePrice": 1800.00,
    "createdAt": "2025-12-20T15:30:00Z"
  }
}
```

#### Get Product by Barcode
**GET** `/api/v1/products/barcode/{barcode}`

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "a1b2c3d4-e5f6-4789-0123-456789abcdef",
    "sku": "MART-001",
    "barcode": "7501234567890",
    "name": "Martillo de Garra 16 oz",
    "salePrice": 250.00,
    "availableQuantity": 45,
    "taxRate": 0.16
  }
}
```

---

### Sales (POS)

#### Create Sale
**POST** `/api/v1/sales`

**Request:**
```json
{
  "saleType": "pos",
  "customerId": "customer-id-optional",
  "customerName": "Cliente General",
  "warehouseId": "warehouse-id",
  "items": [
    {
      "productId": "product-1",
      "quantity": 2,
      "unitPrice": 250.00,
      "discountPercent": 0,
      "taxRate": 0.16
    },
    {
      "productId": "product-2",
      "quantity": 1,
      "unitPrice": 1800.00,
      "discountPercent": 5,
      "taxRate": 0.16
    }
  ],
  "payments": [
    {
      "paymentMethod": "cash",
      "amount": 2390.00
    }
  ],
  "requiresInvoice": false,
  "notes": "Venta mostrador"
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "id": "sale-id",
    "saleNumber": "POS-2025-000123",
    "subtotal": 2050.00,
    "taxAmount": 328.00,
    "total": 2378.00,
    "paidAmount": 2390.00,
    "change": 12.00,
    "status": "completed",
    "completedAt": "2025-12-20T15:35:00Z",
    "receiptUrl": "https://api.corelio.com.mx/api/v1/sales/sale-id/receipt"
  }
}
```

---

### CFDI (Invoices)

#### Generate Invoice from Sale
**POST** `/api/v1/invoices`

**Request:**
```json
{
  "saleId": "sale-id",
  "serie": "A",
  "receiverRfc": "XAXX010101000",
  "receiverName": "PÚBLICO EN GENERAL",
  "receiverCfdiUse": "G01",
  "receiverPostalCode": "06600",
  "paymentForm": "01",
  "paymentMethod": "PUE"
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "id": "invoice-id",
    "folio": "123",
    "serie": "A",
    "status": "draft",
    "subtotal": 2050.00,
    "total": 2378.00,
    "receiverRfc": "XAXX010101000",
    "createdAt": "2025-12-20T15:40:00Z"
  }
}
```

#### Stamp Invoice (PAC)
**POST** `/api/v1/invoices/{id}/stamp`

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "invoice-id",
    "uuid": "12345678-1234-1234-1234-123456789012",
    "status": "stamped",
    "stampDate": "2025-12-20T15:41:00Z",
    "xmlUrl": "https://api.corelio.com.mx/api/v1/invoices/invoice-id/xml",
    "pdfUrl": "https://api.corelio.com.mx/api/v1/invoices/invoice-id/pdf",
    "qrCodeData": "https://verificacfdi.facturaelectronica.sat.gob.mx/..."
  }
}
```

---

## Rate Limiting

### Limits by Plan

| Plan | Requests/Hour | Burst |
|------|---------------|-------|
| Basic | 1,000 | 50 |
| Premium | 5,000 | 100 |
| Enterprise | 20,000 | 200 |

### Rate Limit Headers
```http
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 995
X-RateLimit-Reset: 1640995200
```

---

## Webhook Events

### Available Events
- `sale.created`
- `sale.completed`
- `sale.cancelled`
- `invoice.stamped`
- `invoice.cancelled`
- `product.created`
- `product.updated`
- `inventory.low_stock`

### Webhook Payload
```json
{
  "eventId": "event-uuid",
  "eventType": "sale.completed",
  "tenantId": "tenant-id",
  "timestamp": "2025-12-20T15:35:00Z",
  "data": {
    "saleId": "sale-id",
    "saleNumber": "POS-2025-000123",
    "total": 2378.00
  }
}
```