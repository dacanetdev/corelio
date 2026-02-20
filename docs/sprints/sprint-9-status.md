# Sprint 9: CFDI Integration

**Goal:** SAT-compliant CFDI 4.0 invoice generation, digital signing, PAC stamping via Finkel, and complete invoice management UI — accountants can generate, view, and cancel electronic invoices entirely within Corelio.

**Duration:** TBD (~4 days estimated)
**Status:** 🔴 Not Started (0%)
**Started:** TBD
**Total Story Points:** 34 pts (US-9.1: 8, US-9.2: 8, US-9.3: 8, US-9.4: 10)
**Completed:** 0/41 tasks (0%)

> ⚠️ **Risk:** US-9.2 and US-9.3 carry the highest technical risk — Azure Key Vault and PAC sandbox access must be configured before work begins. Recommend spiking on Key Vault connectivity on Sprint 9 Day 1.

---

## User Story 9.1: CFDI Domain Model & Infrastructure
**As a developer, I want the CFDI invoice domain model, database schema, and repository in place so that all subsequent CFDI features build on a solid, SAT-compliant data foundation.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.1.1 | Create `Invoice` entity with all CFDI 4.0 fields (Serie, Folio, Uuid, StampDate, SatCertificateNumber, PacStampSignature, SatStampSignature, QrCodeData, XmlContent, SaleId, CustomerId) | `feature/US-9.1-cfdi-domain` | 🔴 | In `Domain/Entities/CFDI/` |
| TASK-9.1.2 | Create `InvoiceItem` entity — links `Invoice` + `Product`, with SAT codes, quantity, unit price, discount, taxes as JSONB | `feature/US-9.1-cfdi-domain` | 🔴 | |
| TASK-9.1.3 | Create `InvoiceStatus` enum (`Draft`, `Stamped`, `Cancelled`) and `InvoiceType` enum | `feature/US-9.1-cfdi-domain` | 🔴 | |
| TASK-9.1.4 | Update `TenantConfiguration` entity with CFDI fields: `CfdiCertificateId`, `CfdiKeyVaultUrl`, `CfdiCertificateExpiresAt`, `IssuerRfc`, `IssuerName`, `IssuerTaxRegime`, `IssuerPostalCode`, `DefaultCfdiSerie` | `feature/US-9.1-cfdi-domain` | 🔴 | |
| TASK-9.1.5 | Create EF Core configurations for `Invoice` (with `UNIQUE(tenant_id, serie, folio)` constraint) and `InvoiceItem` | `feature/US-9.1-cfdi-domain` | 🔴 | Follow `docs/planning/04-cfdi-integration-specification.md` |
| TASK-9.1.6 | Update `ApplicationDbContext` — add `DbSet<Invoice>`, `DbSet<InvoiceItem>`, add query filters to `ApplyTenantQueryFilters()` | `feature/US-9.1-cfdi-domain` | 🔴 | |
| TASK-9.1.7 | Create `IInvoiceRepository` interface and `InvoiceRepository` implementation | `feature/US-9.1-cfdi-domain` | 🔴 | |
| TASK-9.1.8 | Create migration `AddCfdiInvoiceSchema` and verify it applies without errors | `feature/US-9.1-cfdi-domain` | 🔴 | |
| TASK-9.1.9 | Register `IInvoiceRepository` in `DependencyInjection.cs` (both methods) | `feature/US-9.1-cfdi-domain` | 🔴 | |

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
**As an accountant, I want the system to generate a valid CFDI 4.0 XML document and sign it with the tenant's CSD certificate from Azure Key Vault so that invoices are ready for PAC stamping.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.2.1 | Add NuGet packages to `Corelio.Infrastructure`: `Azure.Identity`, `Azure.Security.KeyVault.Certificates`, `Azure.Security.KeyVault.Secrets` | `feature/US-9.2-cfdi-xml-signature` | 🔴 | |
| TASK-9.2.2 | Create `ICertificateService` interface in Application layer (`LoadCertificateAsync`, `UploadCertificateAsync`, `GetCertificateMetadataAsync`) | `feature/US-9.2-cfdi-xml-signature` | 🔴 | |
| TASK-9.2.3 | Implement `AzureKeyVaultCertificateService` using `DefaultAzureCredential` (Managed Identity — no credentials in code) | `feature/US-9.2-cfdi-xml-signature` | 🔴 | Key Vault secret name: `csd-tenant-{tenantId:N}` |
| TASK-9.2.4 | Create `CFDIXMLGenerator` in `Infrastructure/CFDI/` — generates CFDI 4.0 XML using `System.Xml.Linq` | `feature/US-9.2-cfdi-xml-signature` | 🔴 | SAT namespace: `http://www.sat.gob.mx/cfd/4` |
| TASK-9.2.5 | Embed `cfdv40.xsd` SAT schema as resource and add XSD validation step in generator | `feature/US-9.2-cfdi-xml-signature` | 🔴 | |
| TASK-9.2.6 | Create `GenerateInvoiceCommand` + handler — creates `Invoice` draft from `SaleId`, calculates folio (`MAX(folio)+1` per tenant/serie), maps sale items with SAT codes | `feature/US-9.2-cfdi-xml-signature` | 🔴 | |
| TASK-9.2.7 | Add certificate upload endpoint `POST /api/v1/tenants/cfdi/certificate` (multipart form with `.pfx` file) | `feature/US-9.2-cfdi-xml-signature` | 🔴 | |
| TASK-9.2.8 | Add `Azure:KeyVault:Url` to `appsettings.json`; configure via user secrets for local dev | `feature/US-9.2-cfdi-xml-signature` | 🔴 | NEVER hardcode Key Vault URL |
| TASK-9.2.9 | Register `ICertificateService` → `AzureKeyVaultCertificateService` in `DependencyInjection.cs` | `feature/US-9.2-cfdi-xml-signature` | 🔴 | |
| TASK-9.2.10 | Unit tests for `CFDIXMLGenerator` — 3+ test scenarios verifying XML node structure and attribute values (standard sale, with discount, cash payment) | `feature/US-9.2-cfdi-xml-signature` | 🔴 | |

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
**As an accountant, I want the system to send CFDI XML to the PAC (Finkel) for stamping and store the UUID and fiscal seal so that invoices are legally valid per SAT.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.3.1 | Add `Polly.Extensions.Http` NuGet to `Corelio.Infrastructure` | `feature/US-9.3-pac-integration` | 🔴 | Retry policy |
| TASK-9.3.2 | Create `IPACProvider` interface in Application layer (`StampAsync`, `CancelAsync`) | `feature/US-9.3-pac-integration` | 🔴 | |
| TASK-9.3.3 | Implement `FinkelPACProvider` — connects to Finkel sandbox in development; retry policy (100ms, 400ms, 1600ms exponential backoff) | `feature/US-9.3-pac-integration` | 🔴 | Config: `Finkel:ApiUrl`, `Finkel:ApiKey` via user secrets |
| TASK-9.3.4 | Create `ICFDIService` interface in Application layer | `feature/US-9.3-pac-integration` | 🔴 | |
| TASK-9.3.5 | Implement `CFDIService` — orchestrates: GenerateDraft → LoadCert → GenerateXML → Sign → Stamp → Persist | `feature/US-9.3-pac-integration` | 🔴 | `using var certificate = ...` for disposal |
| TASK-9.3.6 | Create `StampInvoiceCommand` + handler — changes `Invoice.Status` to `Stamped`, persists UUID, seals, QrCodeData | `feature/US-9.3-pac-integration` | 🔴 | |
| TASK-9.3.7 | Create `CancelInvoiceCommand` + handler — validates 72-hour cancellation window, reason codes 01-04 | `feature/US-9.3-pac-integration` | 🔴 | |
| TASK-9.3.8 | Implement PDF generation for stamped invoice with QR code (extend `SaleReceiptService` or create `InvoicePdfService`) | `feature/US-9.3-pac-integration` | 🔴 | Reuse QuestPDF from Sprint 8 |
| TASK-9.3.9 | Register `IPACProvider`, `ICFDIService` in `DependencyInjection.cs` | `feature/US-9.3-pac-integration` | 🔴 | |
| TASK-9.3.10 | Add user secrets: `Finkel:ApiUrl`, `Finkel:ApiKey` | `feature/US-9.3-pac-integration` | 🔴 | |
| TASK-9.3.11 | Integration test: stamp a test invoice using Finkel sandbox with RFC `XAXX010101000` and assert UUID is returned | `feature/US-9.3-pac-integration` | 🔴 | Requires sandbox access |

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

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-9.4.1 | Create `CfdiEndpoints.cs` with 8 invoice endpoints: POST generate, GET list, GET detail, POST stamp, POST cancel, GET xml, GET pdf, POST send-email | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.2 | Add CFDI settings endpoints: `GET /api/v1/tenants/cfdi/settings`, `PUT /api/v1/tenants/cfdi/settings`, `POST /api/v1/tenants/cfdi/certificate` | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.3 | Register `CfdiEndpoints` in `EndpointExtensions.cs` | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.4 | Create `ICfdiHttpService` + `CfdiHttpService` in BlazorApp | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.5 | Create `InvoiceList.razor` — paginated table at `/facturas` with: Folio, Serie, UUID (partial), Customer RFC, Total, Status chips (borrador/timbrado/cancelado), Date | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.6 | Create `GenerateInvoiceForm.razor` — sale selector, customer RFC verification, CFDI use dropdown (SAT catalog G01-G03/etc.), submit | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.7 | Create `InvoiceDetail.razor` — stamped invoice view, download XML/PDF buttons, send email button, cancel with reason dropdown (01-04) | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | Cancel button only visible if stamped + within 72h |
| TASK-9.4.8 | Create `CfdiSettings.razor` at `/settings/cfdi` — certificate upload (drag-and-drop `.pfx`), expiry date display, issuer data form (RFC, name, tax regime, postal code, default serie) | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.9 | Add "Facturas" and "Configuración CFDI" links to `NavMenu.razor` | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.10 | Add ~50 es-MX localization keys (`Invoice`, `Stamp`, `CancellationReason`, `TaxRegime`, `CfdiUse`, `FiscalFolio`, SAT reason codes, etc.) | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |
| TASK-9.4.11 | Add CFDI permissions to seed data: `cfdi.generate`, `cfdi.cancel`, `settings.cfdi` | `feature/US-9.4-cfdi-invoice-ui` | 🔴 | |

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
| US-9.1: CFDI Domain Model & Infrastructure | P0 Critical | 8 | 🔴 Not Started |
| US-9.2: CFDI XML Generation & Digital Signature | P0 Critical | 8 | 🔴 Not Started |
| US-9.3: PAC Integration & Invoice Stamping | P0 Critical | 8 | 🔴 Not Started |
| US-9.4: CFDI Invoice UI | P0 Critical | 10 | 🔴 Not Started |
| **Total** | | **34** | |

**Recommended execution order:** US-9.1 → US-9.2 → US-9.3 → US-9.4 (strict — each depends on prior)
**Pre-sprint spike required:** Verify Azure Key Vault and Finkel sandbox connectivity before committing to sprint
