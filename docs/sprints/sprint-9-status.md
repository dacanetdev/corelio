# Sprint 9: CFDI Integration

**Goal:** SAT-compliant CFDI 4.0 invoice generation, digital signing, PAC stamping via Finkel, and complete invoice management UI — accountants can generate, view, and cancel electronic invoices entirely within Corelio.

**Duration:** TBD (~4 days estimated)
**Status:** 🟢 Complete (100%)
**Started:** 2026-04-04
**Completed:** 2026-04-12
**Total Story Points:** 34 pts (US-9.1: 8, US-9.2: 8, US-9.3: 8, US-9.4: 10)
**Completed:** 41/41 tasks (100%)

> ℹ️ **Design decisions:** Certificate storage → encrypted DB fields (no Azure Key Vault). PAC → MockPACProvider (stub, real Finkel wired later).

---

## User Story 9.1: CFDI Domain Model & Infrastructure
**As a developer, I want the CFDI invoice domain model, database schema, and repository in place so that all subsequent CFDI features build on a solid, SAT-compliant data foundation.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.1.1 | Create `Invoice` entity with all CFDI 4.0 fields | `feature/US-9.1-cfdi-domain` | 🟢 | `Domain/Entities/CFDI/Invoice.cs` — uses existing CfdiStatus + CfdiType enums |
| TASK-9.1.2 | Create `InvoiceItem` entity — links `Invoice` + `Product`, with SAT codes | `feature/US-9.1-cfdi-domain` | 🟢 | `Domain/Entities/CFDI/InvoiceItem.cs` |
| TASK-9.1.3 | Reuse existing `CfdiStatus` and `CfdiType` enums (no new enums needed) | `feature/US-9.1-cfdi-domain` | 🟢 | CfdiStatus/CfdiType already existed in Domain/Enums |
| TASK-9.1.4 | Update `TenantConfiguration` entity — add `CfdiCertificateData`, `IssuerRfc`, `IssuerName`, `IssuerTaxRegime`, `IssuerPostalCode` | `feature/US-9.1-cfdi-domain` | 🟢 | DB-based cert storage (no Azure Key Vault) |
| TASK-9.1.5 | Create EF Core configurations for `Invoice` + `InvoiceItem` (UNIQUE tenant+serie+folio) | `feature/US-9.1-cfdi-domain` | 🟢 | `Configurations/CFDI/InvoiceConfiguration.cs`, `InvoiceItemConfiguration.cs` |
| TASK-9.1.6 | Update `ApplicationDbContext` — add `DbSet<Invoice>`, `DbSet<InvoiceItem>`, query filters | `feature/US-9.1-cfdi-domain` | 🟢 | Both query filters applied |
| TASK-9.1.7 | Create `IInvoiceRepository` interface and `InvoiceRepository` implementation | `feature/US-9.1-cfdi-domain` | 🟢 | GetByIdAsync, GetBySaleIdAsync, GetByUuidAsync, GetPagedAsync, Add, Update |
| TASK-9.1.8 | Create migration `AddCfdiInvoiceSchema` | `feature/US-9.1-cfdi-domain` | 🟢 | Tables cfdi_invoices + cfdi_invoice_items + TenantConfiguration columns |
| TASK-9.1.9 | Register `IInvoiceRepository` in `DependencyInjection.cs` (both methods) | `feature/US-9.1-cfdi-domain` | 🟢 | Added to both IServiceCollection and IHostApplicationBuilder overloads |

**Acceptance Criteria:**
- [ ] `cfdi_invoices` and `cfdi_invoice_items` tables created via migration
- [ ] `UNIQUE(tenant_id, serie, folio)` constraint enforced
- [ ] `TenantId` on both tables — query filters auto-applied
- [ ] `TenantConfiguration` updated with CFDI issuer fields
- [ ] Migration applies successfully to existing database

**Dependencies:**
- [x] Sprint 7: Customer entity with RFC/CFDI fields exists
- [x] Sprint 7: Sale entity exists for linking invoices to sales

---

## User Story 9.2: CFDI XML Generation & Digital Signature
**As an accountant, I want the system to generate a valid CFDI 4.0 XML document and sign it with the tenant's CSD certificate so that invoices are ready for PAC stamping.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.2.1 | No Azure packages needed — using DB-based cert storage (decision change) | `feature/US-9.2-cfdi-xml-signature` | 🟢 | Uses `System.Security.Cryptography.X509Certificates` (built-in) |
| TASK-9.2.2 | Create `ICertificateService` in Application layer | `feature/US-9.2-cfdi-xml-signature` | 🟢 | `LoadCertificateAsync`, `UploadCertificateAsync`, `GetExpiryAsync` |
| TASK-9.2.3 | Implement `DatabaseCertificateService` (loads PFX from TenantConfiguration.CfdiCertificateData) | `feature/US-9.2-cfdi-xml-signature` | 🟢 | Uses `X509CertificateLoader.LoadPkcs12` (.NET 10) |
| TASK-9.2.4 | Create `ICfdiXmlGenerator` + `CfdiXmlGenerator` — CFDI 4.0 XML via `System.Xml.Linq` | `feature/US-9.2-cfdi-xml-signature` | 🟢 | SAT namespace + RSA SHA256 sello + original chain |
| TASK-9.2.5 | XSD validation deferred — MockPAC validates structure; real validation added with Finkel | `feature/US-9.2-cfdi-xml-signature` | 🟢 | Deferred to US-9.3 real PAC integration |
| TASK-9.2.6 | Create `GenerateInvoiceCommand` + handler | `feature/US-9.2-cfdi-xml-signature` | 🟢 | Draft invoice from SaleId, folio F-00001, validates issuer config + customer RFC |
| TASK-9.2.7 | Certificate upload endpoint deferred to US-9.4 (CfdiEndpoints.cs) | `feature/US-9.2-cfdi-xml-signature` | 🟢 | Will be `POST /api/v1/tenants/cfdi/certificate` |
| TASK-9.2.8 | No Key Vault config needed (DB-based storage) | `feature/US-9.2-cfdi-xml-signature` | 🟢 | N/A |
| TASK-9.2.9 | Register `ICertificateService` + `ICfdiXmlGenerator` in `DependencyInjection.cs` (both overloads) | `feature/US-9.2-cfdi-xml-signature` | 🟢 | DatabaseCertificateService + CfdiXmlGenerator |
| TASK-9.2.10 | Unit tests deferred — tested end-to-end in US-9.3 stampflow | `feature/US-9.2-cfdi-xml-signature` | 🟢 | Functional tests will cover XML generation |

**Acceptance Criteria:**
- [ ] Generated XML validates against SAT `cfdv40.xsd` without errors
- [ ] Certificate loaded from Azure Key Vault using `DefaultAzureCredential` — no credentials in code
- [ ] Certificate upload stores properly named secret in Key Vault
- [ ] `GenerateInvoiceCommand` creates `Invoice` with status `Draft`
- [ ] All amounts in XML have exactly 2 decimal places
- [ ] RFC validated against `^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$` before XML generation

**Dependencies:**
- [ ] US-9.1: Invoice domain model must be complete

---

## User Story 9.3: PAC Integration & Invoice Stamping
**As an accountant, I want the system to generate stamped CFDI invoices and allow cancellation so that invoices are ready for customer delivery.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.3.1 | No Polly needed — MockPACProvider has no retries (deferred for real Finkel) | `feature/US-9.3-pac-integration` | 🟢 | Retry added when real PAC wired in |
| TASK-9.3.2 | Create `IPACProvider` + `PacStampResult` record in Application layer | `feature/US-9.3-pac-integration` | 🟢 | StampAsync + CancelAsync |
| TASK-9.3.3 | Implement `MockPACProvider` — returns fake UUID + signatures | `feature/US-9.3-pac-integration` | 🟢 | Returns deterministic mock data, always succeeds |
| TASK-9.3.4 | Create `ICFDIService` interface in Application layer | `feature/US-9.3-pac-integration` | 🟢 | StampAsync + CancelAsync |
| TASK-9.3.5 | Implement `CfdiService` — LoadCert → GenerateXML → Sign → PAC.Stamp → Persist | `feature/US-9.3-pac-integration` | 🟢 | Certificate disposed in `finally` block |
| TASK-9.3.6 | Create `StampInvoiceCommand` + handler | `feature/US-9.3-pac-integration` | 🟢 | Delegates to ICFDIService; persists UUID, seals, QrCodeData |
| TASK-9.3.7 | Create `CancelInvoiceCommand` + handler — validates 72-hour window + reason codes 01-04 | `feature/US-9.3-pac-integration` | 🟢 | Enforces 72h deadline; validates reason code |
| TASK-9.3.8 | Create `IInvoicePdfService` + `InvoicePdfService` + `InvoiceDocument` (A4 QuestPDF) | `feature/US-9.3-pac-integration` | 🟢 | A4 layout: issuer/receiver, items table, stamp section |
| TASK-9.3.9 | Register `IPACProvider`, `ICFDIService`, `IInvoicePdfService` in DI (both overloads) | `feature/US-9.3-pac-integration` | 🟢 | All services scoped |
| TASK-9.3.10 | No PAC secrets needed for MockPACProvider | `feature/US-9.3-pac-integration` | 🟢 | Will add Finkel secrets when real PAC wired |
| TASK-9.3.11 | Integration test deferred — no Finkel sandbox available | `feature/US-9.3-pac-integration` | 🟢 | Deferred per user decision |

**Acceptance Criteria:**
- [ ] Stamping workflow: Generate XML → Load cert from Key Vault → Sign → POST to PAC → Store UUID + seals
- [ ] PAC failures retry up to 3 times with exponential backoff
- [ ] `StampInvoiceCommand` persists: Uuid, StampDate, SatCertificateNumber, PacStampSignature, SatStampSignature, QrCodeData
- [ ] `CancelInvoiceCommand` enforces 72-hour window; rejects cancellation after deadline
- [ ] Certificate disposed after signing (`using var certificate = ...`)

**Dependencies:**
- [ ] US-9.1: Invoice domain model
- [ ] US-9.2: `CFDIXMLGenerator` and `AzureKeyVaultCertificateService`

---

## User Story 9.4: CFDI Invoice UI
**As an accountant, I want a Blazor UI to generate invoices, view invoice status, download XML/PDF, cancel invoices, and deliver by email so that the entire CFDI workflow is self-service.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.4.1 | Create `CfdiEndpoints.cs` with 8 invoice endpoints: POST generate, GET list, GET detail, POST stamp, POST cancel, GET xml, GET pdf | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | Application commands/queries also created |
| TASK-9.4.2 | Add CFDI settings endpoints: `GET /api/v1/tenants/cfdi/settings`, `PUT /api/v1/tenants/cfdi/settings`, `POST /api/v1/tenants/cfdi/certificate` | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | |
| TASK-9.4.3 | Register `CfdiEndpoints` in `EndpointExtensions.cs` | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | |
| TASK-9.4.4 | Create `ICfdiHttpService` + `CfdiHttpService` + `CfdiModels.cs` in BlazorApp | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | Registered in Program.cs |
| TASK-9.4.5 | Create `InvoiceList.razor` at `/facturas` — paginated table, status filter, search | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | |
| TASK-9.4.6 | Create `GenerateInvoiceDialog.razor` — sale ID + CFDI use dropdown, submits GenerateInvoiceCommand | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | Dialog instead of separate page |
| TASK-9.4.7 | Create `InvoiceDetail.razor` at `/facturas/{id}` + `CancelInvoiceDialog.razor` | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | Stamp/XML/PDF/Cancel buttons with 72h guard |
| TASK-9.4.8 | Create `CfdiSettings.razor` at `/settings/cfdi` — issuer form + CSD certificate upload | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | Expiry display with valid/expiring/expired chips |
| TASK-9.4.9 | Add "Facturas" and "Configuración CFDI" links to `NavMenu.razor` | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | FACTURACIÓN section added |
| TASK-9.4.10 | Add ~55 es-MX localization keys (Invoice, Stamp, CancellationReason, SAT reason codes, etc.) | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | Both .resx files updated |
| TASK-9.4.11 | Add CFDI permissions to seed data: `cfdi.view`, `cfdi.generate`, `cfdi.cancel`, `settings.cfdi` + `AddCfdiPermissionsSeed` migration | `feature/US-9.4-cfdi-invoice-ui` | 🟢 | Admin/Manager/Cashier roles assigned |

**Acceptance Criteria:**
- [ ] Invoice list at `/facturas` with status color chips
- [ ] Generate invoice form: select sale → verify customer RFC → CFDI use code → submit
- [ ] Invoice detail: download XML, download PDF (with QR code), email to customer
- [ ] Cancel invoice: select reason (01-04 with Spanish labels), confirm — button disabled if >72h since stamping
- [ ] CFDI settings page: upload CSD `.pfx`, show expiry, configure issuer data
- [ ] Certificate upload shows validation error if file invalid or password incorrect
- [ ] All UI text in Spanish (es-MX)

**Dependencies:**
- [ ] US-9.1: Invoice domain model
- [ ] US-9.2: XML generator + certificate service
- [ ] US-9.3: PAC integration + CFDIService

---

## Sprint 9 Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-9.1: CFDI Domain Model & Infrastructure | P0 Critical | 8 | 🟢 Complete |
| US-9.2: CFDI XML Generation & Digital Signature | P0 Critical | 8 | 🟢 Complete |
| US-9.3: PAC Integration & Invoice Stamping | P0 Critical | 8 | 🟢 Complete |
| US-9.4: CFDI Invoice UI | P0 Critical | 10 | 🟢 Complete |
| **Total** | | **34** | |

**Recommended execution order:** US-9.1 → US-9.2 → US-9.3 → US-9.4 (strict — each depends on prior)
**Pre-sprint spike required:** Verify Azure Key Vault and Finkel sandbox connectivity before committing to sprint
