# Corelio - Comprehensive Project Specification
## Multi-Tenant SaaS ERP for Mexican SMEs

**Document Version:** 1.0
**Date:** 2025-12-21
**Project Status:** Planning Complete - Ready for Implementation

---

## Executive Summary

**Corelio** is a cloud-native, multi-tenant SaaS ERP system designed specifically for Mexican small and medium-sized enterprises (SMEs), with initial focus on hardware stores (ferreterías). The system provides unified Point of Sale (POS), inventory management, customer relationship management, and complete CFDI 4.0 tax compliance.

**Market Opportunity:**
- 4.2 million SMEs in México
- 98% lack modern ERP systems
- CFDI 4.0 compliance mandatory since 2022
- Existing solutions too expensive ($200-500/month) or too complex

**Competitive Advantage:**
- **México-Specific:** Built-in CFDI 4.0 compliance, not an afterthought
- **Affordable:** Target $50-100/month (60% cheaper than competitors)
- **Modern Stack:** .NET 10 + Aspire (competitors use legacy tech)
- **Fast:** Sub-5-second checkout (competitors average 10-15 seconds)
- **Cloud-Native:** Automatic updates, backups, scaling

---

## Business Goals

### Primary Objectives
1. **Revenue:** $500K ARR by end of Year 1 (500 paying tenants)
2. **Market Share:** 2% of hardware store market (20,000 stores) within 3 years
3. **Customer Satisfaction:** NPS score >50 within 6 months
4. **Retention:** <5% monthly churn rate
5. **Performance:** 99.9% uptime SLA

### Success Metrics
- Time-to-value: <1 hour from signup to first sale
- Support tickets: <10 per 100 active tenants/month
- Feature adoption: >70% of users using POS within first week
- CFDI compliance: 100% of invoices stamped successfully
- Payment collection: >95% on-time payment rate

---

## Target Users

### 1. Owner/Manager (Primary Persona)
**Profile:**
- Age: 35-60
- Tech-savvy: Medium
- Business size: 2-10 employees
- Annual revenue: $100K-$2M MXN
- Pain points: Paper invoices, no inventory visibility, tax compliance burden

**Needs:**
- Real-time business dashboards
- Financial reports for tax filing
- Customer payment tracking
- CFDI generation and management

### 2. Cashier (Secondary Persona)
**Profile:**
- Age: 18-45
- Tech-savvy: Low-Medium
- Works 6-8 hour shifts
- Pain points: Slow checkout, pricing errors, lack of product info

**Needs:**
- Fast, intuitive POS interface
- Barcode scanner support
- Simple customer lookup
- Clear error messages

### 3. Inventory Manager
**Profile:**
- Age: 25-50
- Tech-savvy: Medium-High
- Pain points: Stock discrepancies, manual counts, no low-stock alerts

**Needs:**
- Real-time inventory levels
- Multi-warehouse management
- Stock movement history
- Automated reorder alerts

### 4. Accountant
**Profile:**
- Age: 30-60
- Tech-savvy: Medium-High
- Pain points: Manual CFDI generation, reconciliation errors, tax filing complexity

**Needs:**
- Bulk CFDI generation
- Invoice cancellation workflow
- Financial reports (P&L, Balance Sheet)
- SAT-compliant XML exports

---

## Functional Requirements

### Module 1: Authentication & Multi-Tenancy

**FR-AUTH-001:** System shall support tenant registration with unique subdomain
**FR-AUTH-002:** System shall validate RFC format per SAT standards
**FR-AUTH-003:** System shall enforce strong password policy (min 8 chars, special char, number)
**FR-AUTH-004:** System shall support email verification
**FR-AUTH-005:** System shall implement JWT-based authentication with refresh tokens
**FR-AUTH-006:** System shall enforce role-based access control (Owner, Cashier, Manager, Accountant)
**FR-AUTH-007:** System shall prevent cross-tenant data access (100% isolation)
**FR-AUTH-008:** System shall support multi-factor authentication (2FA) for Owner role
**FR-AUTH-009:** System shall log all authentication attempts (success/failure)
**FR-AUTH-010:** System shall support password reset via email

### Module 2: Point of Sale (POS)

**FR-POS-001:** System shall complete checkout in <3 seconds (95th percentile)
**FR-POS-002:** System shall support barcode scanner input (EAN13, UPC, CODE128)
**FR-POS-003:** System shall support keyboard shortcuts (F2=search, F12=pay, ESC=cancel)
**FR-POS-004:** System shall calculate IVA (16%) automatically per item
**FR-POS-005:** System shall support multiple payment methods (cash, card, transfer, mixed)
**FR-POS-006:** System shall calculate change for cash payments
**FR-POS-007:** System shall generate thermal receipt (58mm/80mm)
**FR-POS-008:** System shall generate PDF receipt for email
**FR-POS-009:** System shall reserve inventory during checkout (prevent overselling)
**FR-POS-010:** System shall support discounts (percentage or fixed amount)
**FR-POS-011:** System shall allow quick customer creation during checkout
**FR-POS-012:** System shall support sale cancellation with reason code
**FR-POS-013:** System shall track cashier performance (sales per hour, average ticket)
**FR-POS-014:** System shall support offline mode (cache + sync when online) - Phase 2
**FR-POS-015:** System shall support split payments (partial cash, partial card)

### Module 3: Inventory Management

**FR-INV-001:** System shall track inventory levels per product per warehouse
**FR-INV-002:** System shall support multi-warehouse operations
**FR-INV-003:** System shall prevent negative inventory (configurable)
**FR-INV-004:** System shall track inventory movements (purchase, sale, adjustment, transfer)
**FR-INV-005:** System shall support stock adjustments with reason codes
**FR-INV-006:** System shall calculate average cost using weighted average method
**FR-INV-007:** System shall alert when stock below minimum level
**FR-INV-008:** System shall support barcode generation for products
**FR-INV-009:** System shall support product variants (size, color, etc.)
**FR-INV-010:** System shall track inventory by lot/batch (Phase 2)
**FR-INV-011:** System shall support physical inventory counts
**FR-INV-012:** System shall generate inventory valuation reports
**FR-INV-013:** System shall support inter-warehouse transfers
**FR-INV-014:** System shall reserve stock for pending quotes

### Module 4: Product Management

**FR-PROD-001:** System shall support unlimited products per tenant
**FR-PROD-002:** System shall enforce unique SKU per tenant
**FR-PROD-003:** System shall support product categories (hierarchical, 5 levels max)
**FR-PROD-004:** System shall support SAT product codes (ClaveProdServ) for CFDI
**FR-PROD-005:** System shall support SAT unit codes (ClaveUnidad) for CFDI
**FR-PROD-006:** System shall support multiple pricing tiers (retail, wholesale, distributor)
**FR-PROD-007:** System shall calculate profit margin automatically
**FR-PROD-008:** System shall support product images (up to 5 per product)
**FR-PROD-009:** System shall support product search (name, SKU, barcode, description)
**FR-PROD-010:** System shall support bulk product import (CSV/Excel)
**FR-PROD-011:** System shall track product cost history
**FR-PROD-012:** System shall support product bundles (Phase 2)

### Module 5: Customer Management

**FR-CUST-001:** System shall validate RFC format per SAT standards
**FR-CUST-002:** System shall validate CURP format for individuals
**FR-CUST-003:** System shall support customer credit limits
**FR-CUST-004:** System shall track customer balances (accounts receivable)
**FR-CUST-005:** System shall support multiple addresses per customer (billing, shipping)
**FR-CUST-006:** System shall store CFDI preferences (use code, payment method, email)
**FR-CUST-007:** System shall track customer purchase history
**FR-CUST-008:** System shall support customer categories (retail, wholesale, VIP)
**FR-CUST-009:** System shall support customer search (name, RFC, phone, email)
**FR-CUST-010:** System shall prevent duplicate RFC entries per tenant
**FR-CUST-011:** System shall support customer notes and tags

### Module 6: CFDI Compliance

**FR-CFDI-001:** System shall generate CFDI 4.0 compliant XML
**FR-CFDI-002:** System shall validate all SAT requirements before stamping
**FR-CFDI-003:** System shall integrate with PAC provider (Finkel/Divertia)
**FR-CFDI-004:** System shall store stamped XML with UUID
**FR-CFDI-005:** System shall generate PDF invoice with QR code
**FR-CFDI-006:** System shall email invoice to customer automatically
**FR-CFDI-007:** System shall support invoice cancellation with reason code
**FR-CFDI-008:** System shall track cancellation status (pending, accepted, rejected)
**FR-CFDI-009:** System shall support invoice series (A, B, C, etc.)
**FR-CFDI-010:** System shall auto-increment folio numbers
**FR-CFDI-011:** System shall store CSD certificates securely (Azure Key Vault)
**FR-CFDI-012:** System shall alert 30 days before certificate expiration
**FR-CFDI-013:** System shall support credit notes (Egreso)
**FR-CFDI-014:** System shall support payment complements (Pago) - Phase 2
**FR-CFDI-015:** System shall retry failed PAC requests (3 attempts with backoff)

### Module 7: Reporting & Analytics

**FR-RPT-001:** System shall generate daily sales summary
**FR-RPT-002:** System shall generate inventory movement reports
**FR-RPT-003:** System shall generate profit/loss statement
**FR-RPT-004:** System shall generate customer aging report (accounts receivable)
**FR-RPT-005:** System shall generate best-selling products report
**FR-RPT-006:** System shall generate low-stock alerts report
**FR-RPT-007:** System shall export reports to PDF/Excel
**FR-RPT-008:** System shall support date range filtering
**FR-RPT-009:** System shall support real-time dashboard with KPIs
**FR-RPT-010:** System shall track user activity (audit logs)

---

## Non-Functional Requirements

### Performance

**NFR-PERF-001:** Product search shall return results in <500ms (95th percentile)
**NFR-PERF-002:** Add to cart shall complete in <200ms (95th percentile)
**NFR-PERF-003:** Complete sale shall finish in <3 seconds (95th percentile)
**NFR-PERF-004:** CFDI stamping shall complete in <5 seconds (95th percentile)
**NFR-PERF-005:** Dashboard shall load in <2 seconds (95th percentile)
**NFR-PERF-006:** System shall support 100 concurrent users per tenant
**NFR-PERF-007:** System shall handle 1000 sales per day per tenant
**NFR-PERF-008:** Database queries shall use proper indexes (execution time <100ms)

### Scalability

**NFR-SCALE-001:** System shall support 100-500 tenants per server instance
**NFR-SCALE-002:** System shall support horizontal scaling (load-balanced API)
**NFR-SCALE-003:** System shall use connection pooling for database
**NFR-SCALE-004:** System shall use Redis for distributed caching
**NFR-SCALE-005:** System shall use CDN for static assets

### Security

**NFR-SEC-001:** System shall enforce HTTPS (TLS 1.3) for all communication
**NFR-SEC-002:** System shall encrypt passwords using bcrypt (cost factor 12)
**NFR-SEC-003:** System shall use Azure Key Vault for certificate storage
**NFR-SEC-004:** System shall implement rate limiting (100 req/min per IP)
**NFR-SEC-005:** System shall log all security events (failed logins, permission denials)
**NFR-SEC-006:** System shall comply with GDPR/LFPDPPP (Mexican data protection)
**NFR-SEC-007:** System shall implement SQL injection prevention
**NFR-SEC-008:** System shall implement XSS prevention
**NFR-SEC-009:** System shall implement CSRF protection
**NFR-SEC-010:** System shall enforce multi-tenant data isolation (100% - zero leaks)

### Availability

**NFR-AVAIL-001:** System shall achieve 99.9% uptime SLA (8.76 hours downtime/year max)
**NFR-AVAIL-002:** System shall perform automated daily backups (retained 30 days)
**NFR-AVAIL-003:** System shall support point-in-time recovery (7 days)
**NFR-AVAIL-004:** System shall have disaster recovery plan (RTO <4 hours, RPO <1 hour)
**NFR-AVAIL-005:** System shall monitor health checks every 30 seconds

### Usability

**NFR-USA-001:** POS interface shall be usable with keyboard only (no mouse required)
**NFR-USA-002:** System shall support Spanish language interface
**NFR-USA-003:** System shall provide contextual help and tooltips
**NFR-USA-004:** System shall display clear error messages with resolution steps
**NFR-USA-005:** System shall require <1 hour training for cashiers
**NFR-USA-006:** System shall be responsive (mobile-friendly for tablets)

### Maintainability

**NFR-MAINT-001:** System shall achieve >70% code coverage with unit tests
**NFR-MAINT-002:** System shall use SonarQube for code quality (A rating minimum)
**NFR-MAINT-003:** System shall follow Clean Architecture principles
**NFR-MAINT-004:** System shall use dependency injection for all services
**NFR-MAINT-005:** System shall generate API documentation automatically (Swagger)
**NFR-MAINT-006:** System shall use structured logging (Serilog + OpenTelemetry)

---

## Technical Constraints

**TC-001:** Must use .NET 10 + C# 14 (specified by client)
**TC-002:** Must use .NET Aspire for orchestration (specified by client)
**TC-003:** Must use PostgreSQL 16 as primary database
**TC-004:** Must use Blazor Server for UI (not WASM)
**TC-005:** Must comply with CFDI 4.0 specification (no older versions)
**TC-006:** Must integrate with authorized PAC providers only
**TC-007:** Must run on Azure (preferred) or DigitalOcean
**TC-008:** Must support Windows and Linux deployment

---

## Integration Requirements

**INT-001:** PAC Provider API (Finkel or Divertia) for CFDI stamping
**INT-002:** SendGrid or similar for email delivery
**INT-003:** Azure Key Vault for certificate management
**INT-004:** Stripe or Conekta for subscription payments (Phase 2)
**INT-005:** SAT web service for RFC validation (optional)
**INT-006:** Thermal printer (ESC/POS protocol)
**INT-007:** Barcode scanner (USB HID mode)

---

## Regulatory Compliance

**REG-001:** CFDI 4.0 compliance per SAT regulations
**REG-002:** Mexican tax law (Código Fiscal de la Federación)
**REG-003:** LFPDPPP (Ley Federal de Protección de Datos Personales)
**REG-004:** GDPR (for potential EU customers)
**REG-005:** PCI-DSS Level 4 (if storing card data - Phase 2)

---

## Assumptions and Dependencies

### Assumptions
1. Tenants have stable internet connection (min 5 Mbps)
2. Tenants have modern web browser (Chrome, Edge, Firefox last 2 versions)
3. Tenants have valid SAT CSD certificates
4. PAC providers maintain 99.5% uptime
5. Azure/DigitalOcean infrastructure available
6. Development team has .NET 10 and Aspire expertise

### Dependencies
1. .NET 10 SDK availability
2. Azure Key Vault service
3. PostgreSQL 16 availability
4. Docker for local development
5. PAC provider API stability
6. SAT catalog availability (product codes, unit codes, etc.)

---

## Out of Scope (Phase 1 MVP)

The following features are **NOT** included in the 12-week MVP:

1. E-commerce integration
2. Mobile apps (iOS/Android)
3. Advanced analytics and BI
4. Payroll management
5. Purchase order management
6. Manufacturing/production modules
7. Multi-currency support
8. Loyalty program
9. Offline POS mode
10. Integration with accounting software (ContPAQ, QuickBooks)
11. API for third-party integrations
12. White-label/reseller program
13. Advanced permissions (custom roles)
14. Workflow automation
15. Multi-language support (English, etc.)

These will be considered for Phase 2 and beyond based on customer feedback.

---

## Risks and Mitigation

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| PAC service downtime | Medium | High | Implement retry logic, queue failed requests, backup PAC provider |
| Multi-tenant data leakage | Low | Critical | Extensive testing, security audits, query filter validation |
| .NET 10 production bugs | Medium | Medium | Thorough testing, fallback to .NET 9 if needed (user rejected this) |
| CFDI regulation changes | Medium | High | Monitor SAT announcements, flexible architecture for updates |
| Database performance issues | Low | Medium | Load testing, query optimization, proper indexing |
| Key developer unavailable | Medium | High | Knowledge documentation, pair programming, code reviews |
| Azure/DO service outage | Low | High | Multi-region deployment (Phase 2), daily backups |
| Certificate expiration | Medium | Medium | Automated monitoring, 30-day advance alerts |

---

## Glossary

**CFDI:** Comprobante Fiscal Digital por Internet (Digital Tax Receipt)
**SAT:** Servicio de Administración Tributaria (Tax Administration Service)
**PAC:** Proveedor Autorizado de Certificación (Authorized Certification Provider)
**CSD:** Certificado de Sello Digital (Digital Seal Certificate)
**RFC:** Registro Federal de Contribuyentes (Federal Taxpayer Registry)
**CURP:** Clave Única de Registro de Población (Unique Population Registry Code)
**IVA:** Impuesto al Valor Agregado (Value Added Tax - 16%)
**UUID:** Universally Unique Identifier (Folio Fiscal)
**POS:** Point of Sale
**ERP:** Enterprise Resource Planning
**SME:** Small and Medium-sized Enterprise
**SLA:** Service Level Agreement
**RTO:** Recovery Time Objective
**RPO:** Recovery Point Objective

---

**Document Approval:**

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Product Owner | [TBD] | _________ | _____ |
| Tech Lead | [TBD] | _________ | _____ |
| Stakeholder | [TBD] | _________ | _____ |

---
**Last Updated:** 2025-12-21
**Next Review:** After Phase 1 Completion
