# Corelio - CFDI 4.0 Integration Specification

## Document Information
- **Project:** Corelio Multi-Tenant SaaS ERP
- **CFDI Version:** 4.0
- **Country:** México
- **Compliance:** SAT (Servicio de Administración Tributaria)
- **Date:** 2025-12-20

---

## Table of Contents
1. [CFDI Overview](#cfdi-overview)
2. [SAT Requirements](#sat-requirements)
3. [Architecture](#architecture)
4. [Implementation Guide](#implementation-guide)
5. [XML Generation](#xml-generation)
6. [PAC Integration](#pac-integration)
7. [Testing](#testing)

---

## CFDI Overview

### What is CFDI?
**CFDI** (Comprobante Fiscal Digital por Internet) is Mexico's electronic invoicing system mandated by SAT for all commercial transactions.

### Why CFDI 4.0?
- **Mandatory since January 1, 2022**
- Replaces CFDI 3.3
- Stricter validation rules
- New required fields
- Enhanced tax compliance

### Key Concepts

| Term | Spanish | Description |
|------|---------|-------------|
| Issuer | Emisor | Business issuing the invoice (tenant) |
| Receiver | Receptor | Customer receiving the invoice |
| PAC | Proveedor Autorizado de Certificación | Authorized stamping provider |
| UUID | Folio Fiscal | Unique invoice identifier from SAT |
| Stamp | Timbre Fiscal Digital | Digital signature from SAT |
| CSD | Certificado de Sello Digital | Digital seal certificate |

---

## SAT Requirements

### Mandatory Components

#### 1. **Digital Certificate (CSD)**
- Issued by SAT to each RFC
- Consists of:
  - `.cer` file (public certificate)
  - `.key` file (private key)
  - Password for private key
- **Validity:** 4 years
- **Storage:** Encrypted in database or secure vault

#### 2. **XML Structure (CFDI 4.0)**
```xml
<?xml version="1.0" encoding="UTF-8"?>
<cfdi:Comprobante
    xmlns:cfdi="http://www.sat.gob.mx/cfd/4"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xsi:schemaLocation="http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"
    Version="4.0"
    Folio="123"
    Serie="A"
    Fecha="2025-12-20T15:30:00"
    FormaPago="01"
    MetodoPago="PUE"
    TipoDeComprobante="I"
    SubTotal="1000.00"
    Total="1160.00"
    Moneda="MXN"
    LugarExpedicion="06600">

    <cfdi:Emisor Rfc="FLO850101ABC" Nombre="Ferretería López S.A. de C.V." RegimenFiscal="601"/>

    <cfdi:Receptor
        Rfc="XAXX010101000"
        Nombre="PUBLICO EN GENERAL"
        DomicilioFiscalReceptor="06600"
        RegimenFiscalReceptor="616"
        UsoCFDI="G01"/>

    <cfdi:Conceptos>
        <cfdi:Concepto
            ClaveProdServ="25171500"
            ClaveUnidad="H87"
            Cantidad="1.00"
            Descripcion="Martillo de garra 16 oz"
            ValorUnitario="1000.00"
            Importe="1000.00"
            ObjetoImp="02">
            <cfdi:Impuestos>
                <cfdi:Traslados>
                    <cfdi:Traslado Base="1000.00" Impuesto="002" TipoFactor="Tasa" TasaOCuota="0.160000" Importe="160.00"/>
                </cfdi:Traslados>
            </cfdi:Impuestos>
        </cfdi:Concepto>
    </cfdi:Conceptos>

    <cfdi:Impuestos TotalImpuestosTrasladados="160.00">
        <cfdi:Traslados>
            <cfdi:Traslado Base="1000.00" Impuesto="002" TipoFactor="Tasa" TasaOCuota="0.160000" Importe="160.00"/>
        </cfdi:Traslados>
    </cfdi:Impuestos>
</cfdi:Comprobante>
```

#### 3. **SAT Catalogs**
Must use official codes from SAT:

| Catalog | Field | Example | Description |
|---------|-------|---------|-------------|
| c_ClaveProdServ | ClaveProdServ | 25171500 | Product/service code (8 digits) |
| c_ClaveUnidad | ClaveUnidad | H87 | Unit of measure code |
| c_FormaPago | FormaPago | 01, 03, 04 | Payment form (cash, transfer, card) |
| c_MetodoPago | MetodoPago | PUE, PPD | Payment method (single, installments) |
| c_UsoCFDI | UsoCFDI | G01, G02, G03 | CFDI usage purpose |
| c_RegimenFiscal | RegimenFiscal | 601, 612, 616 | Tax regime |

**Download:** https://www.sat.gob.mx/aplicacion/operacion/31274/catalogo-de-claves

---

## Architecture

### CFDI Generation Flow

```
┌─────────────────┐
│  POS Sale       │
│  Completed      │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  1. Generate CFDI Draft             │
│  - Validate customer RFC            │
│  - Set invoice series/folio         │
│  - Map payment methods              │
│  - Calculate taxes                  │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  2. Generate CFDI 4.0 XML           │
│  - Build XML per SAT schema         │
│  - Include all required fields      │
│  - Validate against XSD             │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  3. Sign with CSD                   │
│  - Load tenant certificate          │
│  - Generate original chain          │
│  - Create digital signature         │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  4. Send to PAC for Stamping        │
│  - POST to PAC API                  │
│  - Receive UUID + SAT signature     │
│  - Store stamped XML                │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  5. Generate PDF                    │
│  - Create visual representation     │
│  - Include QR code                  │
│  - Embed fiscal stamp               │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  6. Deliver to Customer             │
│  - Email XML + PDF                  │
│  - Store in customer portal         │
└─────────────────────────────────────┘
```

---

## Implementation Guide

### Step 1: Database Schema

```sql
CREATE TABLE cfdi_invoices (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    sale_id UUID REFERENCES sales(id),

    -- Invoice Numbers
    folio VARCHAR(50) NOT NULL,
    serie VARCHAR(10),
    uuid VARCHAR(36) UNIQUE, -- From SAT

    -- Status
    status VARCHAR(20) DEFAULT 'draft', -- draft, stamped, cancelled
    cfdi_version VARCHAR(5) DEFAULT '4.0',
    invoice_type VARCHAR(1) DEFAULT 'I', -- I=Ingreso, E=Egreso

    -- Issuer (Tenant)
    issuer_rfc VARCHAR(13) NOT NULL,
    issuer_name VARCHAR(300) NOT NULL,
    issuer_tax_regime VARCHAR(3) NOT NULL,

    -- Receiver (Customer)
    receiver_rfc VARCHAR(13) NOT NULL,
    receiver_name VARCHAR(300) NOT NULL,
    receiver_tax_regime VARCHAR(3),
    receiver_postal_code VARCHAR(10) NOT NULL,
    receiver_cfdi_use VARCHAR(3) NOT NULL,

    -- Amounts
    subtotal DECIMAL(15,2) NOT NULL,
    discount DECIMAL(15,2) DEFAULT 0,
    total DECIMAL(15,2) NOT NULL,

    -- Payment
    payment_form VARCHAR(3) NOT NULL, -- 01=cash, 03=transfer, 04=card
    payment_method VARCHAR(3) DEFAULT 'PUE', -- PUE=single, PPD=installments

    -- Stamp Data (from PAC)
    stamp_date TIMESTAMP,
    sat_certificate_number VARCHAR(20),
    pac_stamp_signature TEXT,
    sat_stamp_signature TEXT,
    qr_code_data TEXT,
    original_chain TEXT,

    -- Files
    xml_path VARCHAR(500),
    pdf_path VARCHAR(500),

    -- Cancellation
    cancellation_status VARCHAR(20),
    cancellation_date TIMESTAMP,
    cancellation_reason VARCHAR(2),

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uk_cfdi_tenant_serie_folio UNIQUE (tenant_id, serie, folio)
);

CREATE TABLE cfdi_invoice_items (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL REFERENCES tenants(id),
    invoice_id UUID NOT NULL REFERENCES cfdi_invoices(id),

    -- Item Details
    item_number INT NOT NULL,
    product_key VARCHAR(8) NOT NULL, -- SAT code
    unit_key VARCHAR(3) NOT NULL, -- SAT unit
    description VARCHAR(1000) NOT NULL,
    quantity DECIMAL(10,2) NOT NULL,
    unit_value DECIMAL(15,2) NOT NULL,
    amount DECIMAL(15,2) NOT NULL,
    discount DECIMAL(15,2) DEFAULT 0,

    -- Tax
    tax_object VARCHAR(2) DEFAULT '02', -- 01=no tax, 02=yes tax
    taxes JSONB, -- [{type: "002", rate: 0.16, amount: 160}]

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Step 2: Domain Entities

**File:** `src/Core/Corelio.Domain/Entities/CFDI/Invoice.cs`

```csharp
namespace Corelio.Domain.Entities.CFDI;

public class Invoice : AuditableEntity, ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? SaleId { get; set; }

    // Invoice Numbers
    public string Folio { get; set; } = string.Empty;
    public string? Serie { get; set; }
    public string? Uuid { get; set; } // From SAT

    // Status
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string CfdiVersion { get; set; } = "4.0";
    public string InvoiceType { get; set; } = "I";

    // Issuer (Tenant)
    public string IssuerRfc { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
    public string IssuerTaxRegime { get; set; } = string.Empty;

    // Receiver (Customer)
    public string ReceiverRfc { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverTaxRegime { get; set; } = string.Empty;
    public string ReceiverPostalCode { get; set; } = string.Empty;
    public string ReceiverCfdiUse { get; set; } = string.Empty;

    // Amounts
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }

    // Payment
    public string PaymentForm { get; set; } = string.Empty; // SAT code
    public string PaymentMethod { get; set; } = "PUE";

    // Stamp Data
    public DateTime? StampDate { get; set; }
    public string? SatCertificateNumber { get; set; }
    public string? PacStampSignature { get; set; }
    public string? SatStampSignature { get; set; }
    public string? QrCodeData { get; set; }
    public string? OriginalChain { get; set; }

    // Files
    public string? XmlPath { get; set; }
    public string? PdfPath { get; set; }

    // Navigation
    public virtual Sale? Sale { get; set; }
    public virtual ICollection<InvoiceItem> Items { get; set; } = [];
}

public enum InvoiceStatus
{
    Draft,
    Stamped,
    Cancelled
}
```

### Step 3: CFDI Service Interface

```csharp
namespace Corelio.Application.Common.Interfaces;

public interface ICFDIService
{
    /// <summary>
    /// Generate invoice draft from a completed sale
    /// </summary>
    Task<Result<Guid>> GenerateInvoiceAsync(Guid saleId, CFDIRequest request);

    /// <summary>
    /// Stamp invoice with PAC provider
    /// </summary>
    Task<Result<StampResult>> StampInvoiceAsync(Guid invoiceId);

    /// <summary>
    /// Cancel a stamped invoice
    /// </summary>
    Task<Result> CancelInvoiceAsync(Guid invoiceId, string cancellationReason);

    /// <summary>
    /// Get invoice XML
    /// </summary>
    Task<byte[]> GetInvoiceXMLAsync(Guid invoiceId);

    /// <summary>
    /// Get invoice PDF
    /// </summary>
    Task<byte[]> GetInvoicePDFAsync(Guid invoiceId);

    /// <summary>
    /// Send invoice via email
    /// </summary>
    Task<Result> SendInvoiceEmailAsync(Guid invoiceId, string emailAddress);
}

public record CFDIRequest(
    string ReceiverRfc,
    string ReceiverName,
    string ReceiverCfdiUse,
    string ReceiverPostalCode,
    string PaymentForm,
    string PaymentMethod = "PUE"
);

public record StampResult(
    string Uuid,
    DateTime StampDate,
    string SatCertificateNumber,
    string QrCodeData
);
```

### Step 4: XML Generation

**File:** `src/Infrastructure/Corelio.Infrastructure/CFDI/XMLGenerator.cs`

```csharp
using System.Xml.Linq;
using Corelio.Domain.Entities.CFDI;

namespace Corelio.Infrastructure.CFDI;

public class CFDIXMLGenerator
{
    private static readonly XNamespace CfdiNs = "http://www.sat.gob.mx/cfd/4";

    public string GenerateXML(Invoice invoice)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement(CfdiNs + "Comprobante",
                new XAttribute("Version", "4.0"),
                new XAttribute("Serie", invoice.Serie ?? ""),
                new XAttribute("Folio", invoice.Folio),
                new XAttribute("Fecha", invoice.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XAttribute("FormaPago", invoice.PaymentForm),
                new XAttribute("MetodoPago", invoice.PaymentMethod),
                new XAttribute("TipoDeComprobante", invoice.InvoiceType),
                new XAttribute("SubTotal", invoice.Subtotal.ToString("F2")),
                new XAttribute("Total", invoice.Total.ToString("F2")),
                new XAttribute("Moneda", "MXN"),
                new XAttribute("LugarExpedicion", GetTenantPostalCode()),

                // Emisor
                new XElement(CfdiNs + "Emisor",
                    new XAttribute("Rfc", invoice.IssuerRfc),
                    new XAttribute("Nombre", invoice.IssuerName),
                    new XAttribute("RegimenFiscal", invoice.IssuerTaxRegime)
                ),

                // Receptor
                new XElement(CfdiNs + "Receptor",
                    new XAttribute("Rfc", invoice.ReceiverRfc),
                    new XAttribute("Nombre", invoice.ReceiverName),
                    new XAttribute("DomicilioFiscalReceptor", invoice.ReceiverPostalCode),
                    new XAttribute("RegimenFiscalReceptor", invoice.ReceiverTaxRegime),
                    new XAttribute("UsoCFDI", invoice.ReceiverCfdiUse)
                ),

                // Conceptos
                new XElement(CfdiNs + "Conceptos",
                    invoice.Items.Select((item, index) =>
                        new XElement(CfdiNs + "Concepto",
                            new XAttribute("ClaveProdServ", item.ProductKey),
                            new XAttribute("ClaveUnidad", item.UnitKey),
                            new XAttribute("Cantidad", item.Quantity.ToString("F2")),
                            new XAttribute("Descripcion", item.Description),
                            new XAttribute("ValorUnitario", item.UnitValue.ToString("F2")),
                            new XAttribute("Importe", item.Amount.ToString("F2")),
                            new XAttribute("ObjetoImp", item.TaxObject),
                            GenerateItemTaxes(item)
                        )
                    )
                ),

                // Impuestos
                GenerateTotalTaxes(invoice)
            )
        );

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    private XElement GenerateItemTaxes(InvoiceItem item)
    {
        // Parse taxes from JSON
        var taxes = JsonSerializer.Deserialize<List<TaxInfo>>(item.Taxes ?? "[]");

        return new XElement(CfdiNs + "Impuestos",
            new XElement(CfdiNs + "Traslados",
                taxes.Select(tax =>
                    new XElement(CfdiNs + "Traslado",
                        new XAttribute("Base", item.Amount.ToString("F2")),
                        new XAttribute("Impuesto", tax.Type), // 002 = IVA
                        new XAttribute("TipoFactor", "Tasa"),
                        new XAttribute("TasaOCuota", tax.Rate.ToString("F6")),
                        new XAttribute("Importe", tax.Amount.ToString("F2"))
                    )
                )
            )
        );
    }
}
```

---

## PAC Integration

### Finkel PAC Provider

**File:** `src/Infrastructure/Corelio.Infrastructure/CFDI/FinkelPACProvider.cs`

```csharp
public class FinkelPACProvider(HttpClient httpClient, IConfiguration config) : IPACProvider
{
    private readonly string _apiUrl = config["Finkel:ApiUrl"] ?? throw new InvalidOperationException();
    private readonly string _apiKey = config["Finkel:ApiKey"] ?? throw new InvalidOperationException();

    public async Task<StampResult> StampAsync(string xml, X509Certificate2 certificate)
    {
        var request = new
        {
            xml = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml)),
            certificado = Convert.ToBase64String(certificate.RawData)
        };

        var response = await httpClient.PostAsJsonAsync($"{_apiUrl}/stamp", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new PACException($"Stamping failed: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<FinkelStampResponse>();

        return new StampResult(
            result.Uuid,
            result.StampDate,
            result.SatCertificateNumber,
            result.QrCode
        );
    }

    public async Task<CancelResult> CancelAsync(string uuid, string cancellationReason)
    {
        var request = new
        {
            uuid,
            motivo = cancellationReason // 01, 02, 03, 04
        };

        var response = await httpClient.PostAsJsonAsync($"{_apiUrl}/cancel", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new PACException($"Cancellation failed: {error}");
        }

        return new CancelResult(true);
    }
}
```

---

## Testing

### Test Environment

```csharp
// appsettings.Development.json
{
  "Finkel": {
    "ApiUrl": "https://sandbox.finkel.com.mx/api",
    "ApiKey": "test_api_key",
    "TestMode": true
  }
}
```

### Test RFC
- **RFC:** XAXX010101000 (Público en General)
- **Test Certificate:** Provided by SAT for testing

### Integration Test

```csharp
[Fact]
public async Task GenerateAndStampInvoice_Success()
{
    // Arrange
    var sale = await CreateTestSale();
    var cfdiRequest = new CFDIRequest(
        ReceiverRfc: "XAXX010101000",
        ReceiverName: "PUBLICO EN GENERAL",
        ReceiverCfdiUse: "G01",
        ReceiverPostalCode: "06600",
        PaymentForm: "01"
    );

    // Act - Generate
    var generateResult = await _cfdiService.GenerateInvoiceAsync(sale.Id, cfdiRequest);
    generateResult.IsSuccess.Should().BeTrue();

    // Act - Stamp
    var stampResult = await _cfdiService.StampInvoiceAsync(generateResult.Value);
    stampResult.IsSuccess.Should().BeTrue();

    // Assert
    stampResult.Value.Uuid.Should().NotBeNullOrEmpty();
    stampResult.Value.Uuid.Should().MatchRegex(@"^[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}$");
}
```

---

## Certificate Security Strategy

### The Problem with Database Storage

**CRITICAL:** Storing CSD certificates (`.cer` and `.key` files) and passwords in the database poses severe security risks:

1. **Data Breach Exposure** - If database is compromised, all certificates are exposed
2. **Backup Vulnerability** - Certificates included in all database backups
3. **Log Exposure** - Certificate data may appear in logs or error messages
4. **Compliance Issues** - Violates data protection best practices

### Recommended Solution: Azure Key Vault

**Azure Key Vault** provides enterprise-grade certificate management with:
- Hardware Security Module (HSM) protection
- Access control with Managed Identities
- Audit logging of all certificate access
- Automatic certificate rotation support
- Encryption at rest and in transit

### Architecture Overview

```
┌──────────────────────────────────────────────────────────────┐
│                   CFDI Signing Workflow                       │
├──────────────────────────────────────────────────────────────┤
│                                                                │
│  1. API receives invoice generation request                   │
│           ↓                                                    │
│  2. Load tenant's Key Vault certificate reference             │
│     (from tenant_configurations.cfdi_certificate_id)          │
│           ↓                                                    │
│  3. Authenticate with Azure using Managed Identity            │
│           ↓                                                    │
│  4. Retrieve certificate from Key Vault                       │
│     (certificate + private key in memory only)                │
│           ↓                                                    │
│  5. Sign XML with certificate                                 │
│           ↓                                                    │
│  6. Send signed XML to PAC for stamping                       │
│           ↓                                                    │
│  7. Dispose certificate from memory                           │
│                                                                │
└──────────────────────────────────────────────────────────────┘
```

### Implementation: Azure Key Vault Integration

#### 1. Update Database Schema

**Modified `tenant_configurations` table:**

```sql
ALTER TABLE tenant_configurations
  DROP COLUMN IF EXISTS cfdi_certificate_path,
  DROP COLUMN IF EXISTS cfdi_key_path,
  DROP COLUMN IF EXISTS cfdi_certificate_password;

-- Add Key Vault reference instead
ALTER TABLE tenant_configurations
  ADD COLUMN cfdi_certificate_id VARCHAR(200), -- Key Vault certificate ID/name
  ADD COLUMN cfdi_key_vault_url VARCHAR(500), -- Tenant-specific Key Vault URL (optional)
  ADD COLUMN cfdi_certificate_expires_at TIMESTAMP;

-- Example value: "csd-ferreteria-lopez-2025"
-- or full URI: "https://corelio-prod.vault.azure.net/certificates/csd-tenant-123/current"
```

#### 2. NuGet Packages Required

```bash
dotnet add package Azure.Identity
dotnet add package Azure.Security.KeyVault.Certificates
dotnet add package Azure.Security.KeyVault.Secrets
```

#### 3. Certificate Service Implementation

**File:** `src/Infrastructure/Corelio.Infrastructure/CFDI/AzureKeyVaultCertificateService.cs`

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace Corelio.Infrastructure.CFDI;

/// <summary>
/// Securely loads CSD certificates from Azure Key Vault
/// </summary>
public class AzureKeyVaultCertificateService(
    IConfiguration configuration,
    ILogger<AzureKeyVaultCertificateService> logger) : ICertificateService
{
    private readonly string _keyVaultUrl = configuration["Azure:KeyVault:Url"]
        ?? throw new InvalidOperationException("Key Vault URL not configured");

    public async Task<X509Certificate2> LoadCertificateAsync(
        Guid tenantId,
        string certificateId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Use Managed Identity for authentication (no credentials in code!)
            var credential = new DefaultAzureCredential();

            var certificateClient = new CertificateClient(
                new Uri(_keyVaultUrl),
                credential);

            // Retrieve certificate with private key
            var certificateResponse = await certificateClient.GetCertificateAsync(
                certificateId,
                cancellationToken);

            // Get the secret (contains private key)
            var secretClient = new SecretClient(
                new Uri(_keyVaultUrl),
                credential);

            var secretResponse = await secretClient.GetSecretAsync(
                certificateId,
                cancellationToken: cancellationToken);

            // Convert to X509Certificate2 with private key
            var certificateBytes = Convert.FromBase64String(secretResponse.Value.Value);
            var certificate = new X509Certificate2(
                certificateBytes,
                (string?)null, // No password needed - Key Vault handles encryption
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);

            logger.LogInformation(
                "Successfully loaded certificate {CertificateId} for tenant {TenantId} from Key Vault",
                certificateId, tenantId);

            return certificate;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to load certificate {CertificateId} for tenant {TenantId} from Key Vault",
                certificateId, tenantId);
            throw new CertificateLoadException(
                $"Failed to load certificate from Key Vault: {ex.Message}", ex);
        }
    }

    public async Task<CertificateMetadata> GetCertificateMetadataAsync(
        string certificateId,
        CancellationToken cancellationToken = default)
    {
        var credential = new DefaultAzureCredential();
        var certificateClient = new CertificateClient(new Uri(_keyVaultUrl), credential);

        var certificateResponse = await certificateClient.GetCertificateAsync(
            certificateId,
            cancellationToken);

        var cert = certificateResponse.Value;

        return new CertificateMetadata(
            cert.Name,
            cert.Properties.NotBefore?.UtcDateTime,
            cert.Properties.ExpiresOn?.UtcDateTime,
            cert.Properties.Enabled ?? false
        );
    }

    /// <summary>
    /// Upload a new CSD certificate to Key Vault for a tenant
    /// </summary>
    public async Task<string> UploadCertificateAsync(
        Guid tenantId,
        byte[] certificateBytes,
        string password,
        CancellationToken cancellationToken = default)
    {
        var credential = new DefaultAzureCredential();
        var certificateClient = new CertificateClient(new Uri(_keyVaultUrl), credential);

        // Generate unique certificate name
        var certificateName = $"csd-tenant-{tenantId:N}";

        // Import certificate with private key
        var importOptions = new ImportCertificateOptions(certificateName, certificateBytes)
        {
            Password = password, // Key Vault encrypts this automatically
            Policy = new CertificatePolicy
            {
                Exportable = false, // Prevent export for security
                KeyType = CertificateKeyType.Rsa,
                ReuseKey = false
            }
        };

        // Add metadata tags
        importOptions.Properties.Tags.Add("tenant_id", tenantId.ToString());
        importOptions.Properties.Tags.Add("certificate_type", "csd");
        importOptions.Properties.Tags.Add("uploaded_at", DateTime.UtcNow.ToString("O"));

        var importResult = await certificateClient.ImportCertificateAsync(
            importOptions,
            cancellationToken);

        logger.LogInformation(
            "Successfully uploaded certificate {CertificateName} for tenant {TenantId} to Key Vault",
            certificateName, tenantId);

        return certificateName;
    }
}

public interface ICertificateService
{
    Task<X509Certificate2> LoadCertificateAsync(Guid tenantId, string certificateId, CancellationToken cancellationToken = default);
    Task<CertificateMetadata> GetCertificateMetadataAsync(string certificateId, CancellationToken cancellationToken = default);
    Task<string> UploadCertificateAsync(Guid tenantId, byte[] certificateBytes, string password, CancellationToken cancellationToken = default);
}

public record CertificateMetadata(
    string Name,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    bool IsEnabled);

public class CertificateLoadException(string message, Exception? innerException = null)
    : Exception(message, innerException);
```

#### 4. CFDI Service Integration

**Updated `CFDIService` to use Key Vault:**

```csharp
public class CFDIService(
    ApplicationDbContext dbContext,
    ICertificateService certificateService, // Injected Key Vault service
    IPACProvider pacProvider,
    ITenantService tenantService) : ICFDIService
{
    public async Task<Result<Invoice>> StampInvoiceAsync(Guid invoiceId)
    {
        var invoice = await dbContext.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice == null)
            return Result<Invoice>.NotFound();

        // Get tenant configuration
        var tenant = await tenantService.GetCurrentTenantAsync();
        var config = await dbContext.TenantConfigurations
            .FirstOrDefaultAsync(c => c.TenantId == tenant.Id);

        if (string.IsNullOrEmpty(config?.CfdiCertificateId))
            return Result<Invoice>.Failure("No CSD certificate configured for this tenant");

        try
        {
            // Load certificate from Key Vault (secure!)
            using var certificate = await certificateService.LoadCertificateAsync(
                tenant.Id,
                config.CfdiCertificateId);

            // Generate XML
            var xml = GenerateXML(invoice);

            // Sign and stamp with PAC
            var stampResult = await pacProvider.StampAsync(xml, certificate);

            // Update invoice with stamp data
            invoice.Uuid = stampResult.Uuid;
            invoice.StampDate = stampResult.StampDate;
            invoice.SatCertificateNumber = stampResult.SatCertificateNumber;
            invoice.Status = InvoiceStatus.Stamped;

            await dbContext.SaveChangesAsync();

            return Result<Invoice>.Success(invoice);
        }
        catch (CertificateLoadException ex)
        {
            return Result<Invoice>.Failure($"Certificate error: {ex.Message}");
        }
        // Certificate is disposed here automatically (using statement)
    }
}
```

#### 5. Configuration (appsettings.json)

```json
{
  "Azure": {
    "KeyVault": {
      "Url": "https://corelio-prod.vault.azure.net/"
    }
  }
}
```

**Note:** No credentials in config - use **Managed Identity** in Azure App Service or AKS.

### Alternative: Local Development with User Secrets

For local development without Azure:

```bash
dotnet user-secrets set "Azure:KeyVault:Url" "https://corelio-dev.vault.azure.net/"
```

Use `DefaultAzureCredential` which automatically uses:
1. Environment variables (CI/CD)
2. Managed Identity (Azure production)
3. Visual Studio account (local dev)
4. Azure CLI account (local dev)

### Certificate Expiration Monitoring

**Implement automated monitoring:**

```csharp
public class CertificateExpirationMonitorService(
    ApplicationDbContext dbContext,
    ICertificateService certificateService,
    IEmailService emailService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckCertificateExpirationsAsync();

            // Run daily
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task CheckCertificateExpirationsAsync()
    {
        var configs = await dbContext.TenantConfigurations
            .Where(c => c.CfdiCertificateId != null)
            .ToListAsync();

        foreach (var config in configs)
        {
            var metadata = await certificateService.GetCertificateMetadataAsync(
                config.CfdiCertificateId!);

            if (metadata.ValidTo.HasValue)
            {
                var daysUntilExpiration = (metadata.ValidTo.Value - DateTime.UtcNow).Days;

                if (daysUntilExpiration <= 30)
                {
                    // Send alert email
                    await emailService.SendAsync(
                        config.Tenant.Email,
                        "URGENT: CFDI Certificate Expiring Soon",
                        $"Your CSD certificate will expire in {daysUntilExpiration} days. Please renew it in SAT portal.");
                }
            }
        }
    }
}
```

### Access Control with RBAC

**Configure Key Vault Access Policies:**

```bash
# Grant API Managed Identity access to certificates
az keyvault set-policy \
  --name corelio-prod \
  --object-id <managed-identity-object-id> \
  --certificate-permissions get list \
  --secret-permissions get list
```

**Least Privilege:** API only needs `get` and `list`, NOT `delete` or `purge`.

### Cost Estimate

**Azure Key Vault Pricing (2025):**
- Standard tier: $0.03 per 10,000 operations
- Certificate storage: Free for first 25 certificates
- Estimated cost: < $5/month for 100 tenants

### Alternative Solutions

If Azure is not an option:

1. **HashiCorp Vault** (on-premise or cloud)
   - Similar to Azure Key Vault
   - Self-hosted option available
   - API-driven certificate management

2. **AWS Secrets Manager** (if using AWS)
   - Automatic rotation
   - Fine-grained IAM policies

3. **Encrypted File Storage** (minimum viable)
   - Store encrypted certificates in blob storage (Azure Blob, S3)
   - Encryption keys in Key Vault or AWS KMS
   - **Not recommended** for production - use Key Vault instead

### Migration Path from Database Storage

If certificates are currently in database:

```csharp
public async Task MigrateCertificatesToKeyVault()
{
    var configs = await dbContext.TenantConfigurations
        .Where(c => c.CfdiCertificatePath != null)
        .ToListAsync();

    foreach (var config in configs)
    {
        // Read certificate from file system
        var certBytes = await File.ReadAllBytesAsync(config.CfdiCertificatePath!);
        var keyBytes = await File.ReadAllBytesAsync(config.CfdiKeyPath!);

        // Combine into PFX format
        var pfxBytes = CombineCertificateAndKey(certBytes, keyBytes, config.CfdiCertificatePassword);

        // Upload to Key Vault
        var certificateId = await _certificateService.UploadCertificateAsync(
            config.TenantId,
            pfxBytes,
            config.CfdiCertificatePassword!);

        // Update config
        config.CfdiCertificateId = certificateId;
        config.CfdiCertificatePath = null; // Remove old reference
        config.CfdiKeyPath = null;
        config.CfdiCertificatePassword = null;

        await dbContext.SaveChangesAsync();

        // Delete old files
        File.Delete(config.CfdiCertificatePath!);
        File.Delete(config.CfdiKeyPath!);
    }
}
```

### Security Best Practices Summary

1. **NEVER** store certificate passwords in database (even encrypted)
2. **ALWAYS** use Managed Identity for authentication (no API keys)
3. **LIMIT** Key Vault access to minimum required permissions
4. **MONITOR** certificate access logs for anomalies
5. **ROTATE** certificates before expiration (SAT requires every 4 years)
6. **AUDIT** who uploads/accesses certificates
7. **TEST** certificate loading in staging environment first

---

## Production Checklist

- [ ] Valid CSD certificate uploaded
- [ ] Certificate expiration monitoring
- [ ] PAC provider credentials configured
- [ ] SAT catalogs up to date
- [ ] RFC validation enabled
- [ ] Error logging configured
- [ ] Retry logic for PAC failures
- [ ] Email delivery configured
- [ ] XML/PDF storage configured
- [ ] Invoice numbering sequence initialized
- [ ] Test invoices in PAC sandbox
- [ ] Production PAC credentials ready