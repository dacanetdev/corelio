---
name: cfdi-validation
description: Validates CFDI 4.0 XML structure and SAT compliance requirements
---

# CFDI 4.0 Validation

## Overview

CFDI (Comprobante Fiscal Digital por Internet) is Mexico's electronic invoicing system. Version 4.0 is mandatory as of 2022.

## Critical Requirements

### ✅ ALWAYS Required

1. **Valid RFC (Tax ID)**
   ```csharp
   // RFC patterns
   // Legal entity: AAA010101AAA (12 chars)
   // Individual: AAAA010101AAA (13 chars)

   public static class RfcValidator
   {
       private static readonly Regex LegalEntityPattern = new(@"^[A-ZÑ&]{3}\d{6}[A-Z0-9]{3}$");
       private static readonly Regex IndividualPattern = new(@"^[A-ZÑ&]{4}\d{6}[A-Z0-9]{3}$");

       public static bool IsValid(string rfc)
       {
           if (string.IsNullOrWhiteSpace(rfc)) return false;

           rfc = rfc.ToUpperInvariant().Trim();

           return rfc.Length == 12
               ? LegalEntityPattern.IsMatch(rfc)
               : rfc.Length == 13 && IndividualPattern.IsMatch(rfc);
       }
   }
   ```

2. **Valid SAT Catalogs**
   - Product/Service codes (c_ClaveProdServ)
   - Unit of measure (c_ClaveUnidad)
   - Tax type (c_Impuesto)
   - Payment method (c_FormaPago)
   - Payment type (c_MetodoPago)
   - Use of CFDI (c_UsoCFDI)
   - Tax regime (c_RegimenFiscal)

3. **Valid Certificate (CSD)**
   ```csharp
   public interface ICsdCertificateValidator
   {
       Task<bool> ValidateAsync(X509Certificate2 certificate);
       Task<bool> IsExpiredAsync(X509Certificate2 certificate);
       Task<bool> IsRevokedAsync(string certificateNumber);
   }

   // Certificate must:
   // - Be issued by SAT
   // - Not be expired
   // - Not be revoked
   // - Match tenant's RFC
   ```

## CFDI 4.0 XML Structure

### Comprobante (Root Element)
```xml
<cfdi:Comprobante
    xmlns:cfdi="http://www.sat.gob.mx/cfd/4"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xsi:schemaLocation="http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"
    Version="4.0"
    Serie="A"
    Folio="12345"
    Fecha="2025-01-15T10:30:00"
    Sello=""
    NoCertificado=""
    Certificado=""
    SubTotal="1000.00"
    Descuento="50.00"
    Total="1111.00"
    Moneda="MXN"
    TipoDeComprobante="I"
    Exportacion="01"
    MetodoPago="PUE"
    LugarExpedicion="06000">

    <!-- Emisor, Receptor, Conceptos, Impuestos -->
</cfdi:Comprobante>
```

### Required Attributes
```csharp
public class ComprobanteValidator : AbstractValidator<Comprobante>
{
    public ComprobanteValidator()
    {
        RuleFor(x => x.Version)
            .Equal("4.0").WithMessage("Version must be 4.0");

        RuleFor(x => x.Fecha)
            .NotEmpty().WithMessage("Date is required")
            .Must(BeWithin72Hours).WithMessage("Invoice date must be within last 72 hours");

        RuleFor(x => x.Moneda)
            .NotEmpty().WithMessage("Currency is required")
            .Must(BeValidCurrency).WithMessage("Invalid currency code");

        RuleFor(x => x.TipoDeComprobante)
            .NotEmpty().WithMessage("Document type is required")
            .Must(x => new[] { "I", "E", "T", "N", "P" }.Contains(x))
            .WithMessage("Invalid document type");

        RuleFor(x => x.Exportacion)
            .NotEmpty().WithMessage("Export indicator is required")
            .Must(x => new[] { "01", "02", "03", "04" }.Contains(x))
            .WithMessage("Invalid export indicator");

        RuleFor(x => x.MetodoPago)
            .Must(BeValidMetodoPago).WithMessage("Invalid payment method");

        RuleFor(x => x.LugarExpedicion)
            .NotEmpty().WithMessage("Place of issuance is required")
            .Matches(@"^\d{5}$").WithMessage("Must be a valid 5-digit postal code");
    }

    private bool BeWithin72Hours(DateTime fecha)
    {
        var now = DateTime.UtcNow;
        var hoursAgo72 = now.AddHours(-72);
        return fecha >= hoursAgo72 && fecha <= now;
    }
}
```

## Emisor (Issuer) Validation

```xml
<cfdi:Emisor
    Rfc="AAA010101AAA"
    Nombre="Mi Empresa SA de CV"
    RegimenFiscal="601"/>
```

```csharp
public class EmisorValidator : AbstractValidator<Emisor>
{
    public EmisorValidator(ISatCatalogService satService)
    {
        RuleFor(x => x.Rfc)
            .NotEmpty().WithMessage("RFC is required")
            .Must(RfcValidator.IsValid).WithMessage("Invalid RFC format")
            .Length(12, 13).WithMessage("RFC must be 12 or 13 characters");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(254).WithMessage("Name too long");

        RuleFor(x => x.RegimenFiscal)
            .NotEmpty().WithMessage("Tax regime is required")
            .MustAsync(async (regimen, cancellation) =>
                await satService.IsValidRegimenFiscalAsync(regimen))
            .WithMessage("Invalid tax regime code");
    }
}
```

## Receptor (Receiver) Validation

```xml
<cfdi:Receptor
    Rfc="XAXX010101000"
    Nombre="PUBLICO EN GENERAL"
    DomicilioFiscalReceptor="06000"
    RegimenFiscalReceptor="616"
    UsoCFDI="S01"/>
```

```csharp
public class ReceptorValidator : AbstractValidator<Receptor>
{
    public ReceptorValidator(ISatCatalogService satService)
    {
        RuleFor(x => x.Rfc)
            .NotEmpty().WithMessage("RFC is required")
            .Must(RfcValidator.IsValid).WithMessage("Invalid RFC format");

        // Special case: Public in general
        When(x => x.Rfc == "XAXX010101000", () =>
        {
            RuleFor(x => x.Nombre)
                .Equal("PUBLICO EN GENERAL")
                .WithMessage("For RFC XAXX010101000, name must be 'PUBLICO EN GENERAL'");
        });

        RuleFor(x => x.DomicilioFiscalReceptor)
            .NotEmpty().WithMessage("Fiscal address postal code is required")
            .Matches(@"^\d{5}$").WithMessage("Must be a valid 5-digit postal code");

        RuleFor(x => x.RegimenFiscalReceptor)
            .NotEmpty().WithMessage("Tax regime is required")
            .MustAsync(async (regimen, cancellation) =>
                await satService.IsValidRegimenFiscalAsync(regimen))
            .WithMessage("Invalid tax regime code");

        RuleFor(x => x.UsoCFDI)
            .NotEmpty().WithMessage("CFDI use is required")
            .MustAsync(async (uso, cancellation) =>
                await satService.IsValidUsoCfdiAsync(uso))
            .WithMessage("Invalid CFDI use code");
    }
}
```

## Conceptos (Line Items) Validation

```xml
<cfdi:Conceptos>
    <cfdi:Concepto
        ClaveProdServ="01010101"
        NoIdentificacion="PROD-001"
        Cantidad="2.00"
        ClaveUnidad="H87"
        Unidad="Pieza"
        Descripcion="Producto de ejemplo"
        ValorUnitario="500.00"
        Importe="1000.00"
        Descuento="50.00"
        ObjetoImp="02">

        <cfdi:Impuestos>
            <cfdi:Traslados>
                <cfdi:Traslado
                    Base="950.00"
                    Impuesto="002"
                    TipoFactor="Tasa"
                    TasaOCuota="0.160000"
                    Importe="152.00"/>
            </cfdi:Traslados>
        </cfdi:Impuestos>
    </cfdi:Concepto>
</cfdi:Conceptos>
```

```csharp
public class ConceptoValidator : AbstractValidator<Concepto>
{
    public ConceptoValidator(ISatCatalogService satService)
    {
        RuleFor(x => x.ClaveProdServ)
            .NotEmpty().WithMessage("Product/Service code is required")
            .MustAsync(async (code, cancellation) =>
                await satService.IsValidProductCodeAsync(code))
            .WithMessage("Invalid SAT product/service code");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("Quantity must be positive");

        RuleFor(x => x.ClaveUnidad)
            .NotEmpty().WithMessage("Unit code is required")
            .MustAsync(async (code, cancellation) =>
                await satService.IsValidUnitCodeAsync(code))
            .WithMessage("Invalid SAT unit code");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description too short (min 10 chars)");

        RuleFor(x => x.ValorUnitario)
            .GreaterThanOrEqualTo(0).WithMessage("Unit value must be non-negative")
            .PrecisionScale(18, 6, true).WithMessage("Invalid precision (max 6 decimals)");

        RuleFor(x => x.Importe)
            .GreaterThanOrEqualTo(0).WithMessage("Amount must be non-negative")
            .Must((concepto, importe) =>
                Math.Abs(importe - (concepto.Cantidad * concepto.ValorUnitario)) < 0.01m)
            .WithMessage("Amount must equal Quantity × UnitValue");

        RuleFor(x => x.ObjetoImp)
            .NotEmpty().WithMessage("Tax object is required")
            .Must(x => new[] { "01", "02", "03", "04" }.Contains(x))
            .WithMessage("Invalid tax object code");

        // If ObjetoImp = "02" (Subject to tax), taxes are required
        When(x => x.ObjetoImp == "02", () =>
        {
            RuleFor(x => x.Impuestos)
                .NotNull().WithMessage("Taxes are required when ObjetoImp is '02'");
        });
    }
}
```

## Impuestos (Taxes) Validation

```csharp
public class ImpuestosValidator : AbstractValidator<ImpuestosConcepto>
{
    public ImpuestosValidator()
    {
        // IVA (VAT) - Most common
        RuleForEach(x => x.Traslados)
            .ChildRules(traslado =>
            {
                traslado.RuleFor(x => x.Base)
                    .GreaterThanOrEqualTo(0).WithMessage("Tax base must be non-negative");

                traslado.RuleFor(x => x.Impuesto)
                    .NotEmpty().WithMessage("Tax type is required")
                    .Must(x => new[] { "001", "002", "003" }.Contains(x))
                    .WithMessage("Invalid tax type (001=ISR, 002=IVA, 003=IEPS)");

                traslado.RuleFor(x => x.TipoFactor)
                    .NotEmpty().WithMessage("Tax factor type is required")
                    .Must(x => new[] { "Tasa", "Cuota", "Exento" }.Contains(x))
                    .WithMessage("Invalid tax factor type");

                // Standard IVA rates in Mexico: 0%, 8%, 16%
                When(x => x.Impuesto == "002" && x.TipoFactor == "Tasa", () =>
                {
                    traslado.RuleFor(x => x.TasaOCuota)
                        .Must(x => new[] { 0.000000m, 0.080000m, 0.160000m }.Contains(x))
                        .WithMessage("IVA rate must be 0%, 8%, or 16%");
                });

                traslado.RuleFor(x => x.Importe)
                    .Must((impuesto, importe, context) =>
                    {
                        var parent = context.InstanceToValidate as ImpuestosConcepto;
                        var traslado = parent?.Traslados?.FirstOrDefault();
                        if (traslado == null) return true;

                        var expected = traslado.Base * traslado.TasaOCuota;
                        return Math.Abs(importe - expected) < 0.01m;
                    })
                    .WithMessage("Tax amount must equal Base × Rate");
            });

        // Retenciones (Withholdings) - Less common
        RuleForEach(x => x.Retenciones)
            .ChildRules(retencion =>
            {
                retencion.RuleFor(x => x.Base)
                    .GreaterThanOrEqualTo(0).WithMessage("Tax base must be non-negative");

                retencion.RuleFor(x => x.Impuesto)
                    .NotEmpty().WithMessage("Tax type is required");

                retencion.RuleFor(x => x.TipoFactor)
                    .Equal("Tasa").WithMessage("Withholding must use 'Tasa' factor");
            });
    }
}
```

## Totals Validation

```csharp
public class TotalsValidator : AbstractValidator<Comprobante>
{
    public TotalsValidator()
    {
        RuleFor(x => x)
            .Must(ValidateSubtotal)
            .WithMessage("Subtotal must equal sum of all concept amounts");

        RuleFor(x => x)
            .Must(ValidateTotal)
            .WithMessage("Total calculation is incorrect");
    }

    private bool ValidateSubtotal(Comprobante comprobante)
    {
        var expected = comprobante.Conceptos.Sum(c => c.Importe);
        return Math.Abs(comprobante.SubTotal - expected) < 0.01m;
    }

    private bool ValidateTotal(Comprobante comprobante)
    {
        // Total = Subtotal - Descuento + Impuestos Trasladados - Impuestos Retenidos
        var subtotal = comprobante.SubTotal;
        var descuento = comprobante.Descuento ?? 0;
        var trasladados = comprobante.Impuestos?.TotalImpuestosTrasladados ?? 0;
        var retenidos = comprobante.Impuestos?.TotalImpuestosRetenidos ?? 0;

        var expected = subtotal - descuento + trasladados - retenidos;
        return Math.Abs(comprobante.Total - expected) < 0.01m;
    }
}
```

## Digital Signature (Sello) Validation

```csharp
public interface ICfdiSignatureValidator
{
    /// <summary>
    /// Validates the digital signature of a CFDI
    /// </summary>
    Task<Result<bool>> ValidateSignatureAsync(string xmlContent, X509Certificate2 certificate);

    /// <summary>
    /// Signs a CFDI with the tenant's CSD certificate
    /// </summary>
    Task<Result<SignedCfdi>> SignCfdiAsync(string xmlContent, Guid tenantId);
}

public class CfdiSignatureValidator : ICfdiSignatureValidator
{
    public async Task<Result<bool>> ValidateSignatureAsync(
        string xmlContent,
        X509Certificate2 certificate)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var sello = xmlDoc.DocumentElement?.GetAttribute("Sello");
            if (string.IsNullOrWhiteSpace(sello))
                return Result<bool>.Failure("Missing digital signature (Sello)");

            // Extract original chain
            var cadenaOriginal = GetCadenaOriginal(xmlDoc);

            // Verify signature
            using var rsa = certificate.GetRSAPublicKey();
            var signatureBytes = Convert.FromBase64String(sello);
            var dataBytes = Encoding.UTF8.GetBytes(cadenaOriginal);

            var isValid = rsa.VerifyData(
                dataBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            return Result<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Signature validation failed: {ex.Message}");
        }
    }

    private string GetCadenaOriginal(XmlDocument xmlDoc)
    {
        // Apply XSLT transformation to get original chain
        // SAT provides official XSLT files for each CFDI version
        var xslt = LoadSatXslt("cadenaoriginal_4_0.xslt");

        var transform = new XslCompiledTransform();
        transform.Load(xslt);

        using var writer = new StringWriter();
        transform.Transform(xmlDoc, null, writer);

        return writer.ToString();
    }
}
```

## SAT Catalog Validation

```csharp
public interface ISatCatalogService
{
    Task<bool> IsValidProductCodeAsync(string code);
    Task<bool> IsValidUnitCodeAsync(string code);
    Task<bool> IsValidUsoCfdiAsync(string code);
    Task<bool> IsValidRegimenFiscalAsync(string code);
    Task<bool> IsValidPaymentMethodAsync(string code);
    Task<bool> IsValidPaymentFormAsync(string code);
}

// Example: Product/Service Code
public class SatProductCode
{
    public string Code { get; set; } // "01010101"
    public string Description { get; set; } // "Animales vivos"
    public bool IsActive { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

// Cached validation
public class SatCatalogService : ISatCatalogService
{
    private readonly IMemoryCache _cache;
    private readonly IApplicationDbContext _context;

    public async Task<bool> IsValidProductCodeAsync(string code)
    {
        var cacheKey = $"sat_product_{code}";

        if (_cache.TryGetValue<bool>(cacheKey, out var isValid))
            return isValid;

        var exists = await _context.SatProductCodes
            .AnyAsync(p => p.Code == code && p.IsActive);

        _cache.Set(cacheKey, exists, TimeSpan.FromHours(24));

        return exists;
    }
}
```

## Common Validation Errors

### 1. Invalid RFC Format
```csharp
// ❌ WRONG
"AAA-010101-AAA" // Contains hyphens
"aaa010101aaa"   // Lowercase
"AAA010101"      // Too short

// ✅ CORRECT
"AAA010101AAA"   // 12 chars, uppercase, no separators
"AAAA010101AAA"  // 13 chars for individuals
```

### 2. Incorrect Totals
```csharp
// ❌ WRONG
Subtotal = 1000.00
IVA (16%) = 160.00
Total = 1150.00 // Should be 1160.00!

// ✅ CORRECT
Subtotal = 1000.00
IVA (16%) = 160.00
Total = 1160.00
```

### 3. Invalid SAT Codes
```csharp
// ❌ WRONG
ClaveProdServ = "12345678" // Non-existent code

// ✅ CORRECT
ClaveProdServ = "01010101" // Valid SAT code from catalog
```

### 4. Date Out of Range
```csharp
// ❌ WRONG
Fecha = DateTime.Now.AddDays(-5) // More than 72 hours ago

// ✅ CORRECT
Fecha = DateTime.Now.AddMinutes(-10) // Within 72 hours
```

## Testing Strategy

### Unit Tests
```csharp
[Fact]
public void Rfc_Validator_Should_Accept_Valid_Legal_Entity_Rfc()
{
    // Arrange
    var rfc = "AAA010101AAA";

    // Act
    var isValid = RfcValidator.IsValid(rfc);

    // Assert
    isValid.Should().BeTrue();
}

[Fact]
public void Total_Should_Equal_Subtotal_Plus_Tax()
{
    // Arrange
    var comprobante = new Comprobante
    {
        SubTotal = 1000.00m,
        Impuestos = new Impuestos
        {
            TotalImpuestosTrasladados = 160.00m
        },
        Total = 1160.00m
    };

    // Act
    var validator = new TotalsValidator();
    var result = validator.Validate(comprobante);

    // Assert
    result.IsValid.Should().BeTrue();
}
```

### Integration Tests
```csharp
[Fact]
public async Task Should_Generate_Valid_CFDI_XML()
{
    // Arrange
    var request = new GenerateCfdiRequest
    {
        CustomerId = _customerId,
        Items = new[]
        {
            new CfdiItem
            {
                ProductCode = "01010101",
                Quantity = 2,
                UnitPrice = 500.00m
            }
        }
    };

    // Act
    var result = await _cfdiService.GenerateAsync(request);

    // Assert
    result.IsSuccess.Should().BeTrue();

    var xml = result.Value.XmlContent;
    xml.Should().Contain("Version=\"4.0\"");

    // Validate against SAT XSD
    var isValidXml = await _satValidator.ValidateSchemaAsync(xml);
    isValidXml.Should().BeTrue();
}
```

## Compliance Checklist

Before submitting CFDI to PAC:

- [ ] RFC format validated (issuer and receiver)
- [ ] All SAT catalog codes validated
- [ ] Date within 72 hours
- [ ] Subtotal calculation correct
- [ ] Tax calculations correct
- [ ] Total calculation correct
- [ ] CSD certificate valid and not expired
- [ ] Digital signature (Sello) present
- [ ] XML validates against SAT XSD schema
- [ ] Original chain (Cadena Original) generated correctly
- [ ] Certificate number matches XML

## Resources

- SAT Official Documentation: https://www.sat.gob.mx/consulta/factura_electronica/
- CFDI 4.0 XSD Schema: http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd
- SAT Catalogs: https://www.sat.gob.mx/consulta/catalogos_detalle

---

**Remember**: CFDI validation errors can result in rejected invoices and tax compliance issues!
