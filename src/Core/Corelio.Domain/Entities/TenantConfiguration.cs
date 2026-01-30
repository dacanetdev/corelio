using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Tenant-specific configuration settings.
/// </summary>
public class TenantConfiguration : AuditableEntity
{
    /// <summary>
    /// The tenant this configuration belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    // CFDI Settings (Mexican Electronic Invoicing)
    /// <summary>
    /// PAC provider for CFDI stamping.
    /// </summary>
    public PacProvider? CfdiPacProvider { get; set; }

    /// <summary>
    /// PAC API URL.
    /// </summary>
    public string? CfdiPacApiUrl { get; set; }

    /// <summary>
    /// PAC API key (encrypted).
    /// </summary>
    public string? CfdiPacApiKey { get; set; }

    /// <summary>
    /// Whether to use PAC test mode.
    /// </summary>
    public bool CfdiPacTestMode { get; set; } = true;

    /// <summary>
    /// Path to CSD certificate file.
    /// </summary>
    public string? CfdiCertificatePath { get; set; }

    /// <summary>
    /// Path to private key file.
    /// </summary>
    public string? CfdiKeyPath { get; set; }

    /// <summary>
    /// CSD certificate password (encrypted).
    /// </summary>
    public string? CfdiCertificatePassword { get; set; }

    /// <summary>
    /// When the CSD certificate expires.
    /// </summary>
    public DateTime? CfdiCertificateExpiresAt { get; set; }

    /// <summary>
    /// Invoice series (A, B, C, etc.).
    /// </summary>
    public string CfdiSeries { get; set; } = "A";

    /// <summary>
    /// Next folio number for invoices.
    /// </summary>
    public int CfdiNextFolio { get; set; } = 1;

    // Business Settings
    /// <summary>
    /// Default warehouse ID for new sales.
    /// </summary>
    public Guid? DefaultWarehouseId { get; set; }

    /// <summary>
    /// Default tax rate (IVA 16% = 0.1600).
    /// </summary>
    public decimal DefaultTaxRate { get; set; } = 0.1600m;

    /// <summary>
    /// Currency code (MXN, USD).
    /// </summary>
    public string Currency { get; set; } = "MXN";

    /// <summary>
    /// Timezone for this tenant.
    /// </summary>
    public string Timezone { get; set; } = "America/Mexico_City";

    /// <summary>
    /// Business hours start time.
    /// </summary>
    public TimeOnly? BusinessHoursStart { get; set; } = new TimeOnly(9, 0);

    /// <summary>
    /// Business hours end time.
    /// </summary>
    public TimeOnly? BusinessHoursEnd { get; set; } = new TimeOnly(18, 0);

    // POS Settings
    /// <summary>
    /// Whether to auto-print receipt after sale.
    /// </summary>
    public bool PosAutoPrintReceipt { get; set; } = false;

    /// <summary>
    /// Whether customer is required for sales.
    /// </summary>
    public bool PosRequireCustomer { get; set; } = false;

    /// <summary>
    /// Default payment method for POS.
    /// </summary>
    public PaymentMethod PosDefaultPaymentMethod { get; set; } = PaymentMethod.Cash;

    /// <summary>
    /// Whether barcode scanner is enabled.
    /// </summary>
    public bool PosEnableBarcodeScanner { get; set; } = true;

    /// <summary>
    /// Thermal printer name for receipts.
    /// </summary>
    public string? PosThermalPrinterName { get; set; }

    /// <summary>
    /// Custom footer text for receipts.
    /// </summary>
    public string? PosReceiptFooter { get; set; }

    // Pricing Settings
    /// <summary>
    /// Whether to allow negative inventory.
    /// </summary>
    public bool AllowNegativeInventory { get; set; } = false;

    /// <summary>
    /// Whether product cost is required.
    /// </summary>
    public bool RequireProductCost { get; set; } = true;

    /// <summary>
    /// Whether to auto-calculate margin.
    /// </summary>
    public bool AutoCalculateMargin { get; set; } = true;

    // Feature Flags
    /// <summary>
    /// Whether multi-warehouse feature is enabled.
    /// </summary>
    public bool FeatureMultiWarehouse { get; set; } = false;

    /// <summary>
    /// Whether e-commerce feature is enabled.
    /// </summary>
    public bool FeatureEcommerce { get; set; } = false;

    /// <summary>
    /// Whether loyalty program feature is enabled.
    /// </summary>
    public bool FeatureLoyaltyProgram { get; set; } = false;

    /// <summary>
    /// Whether purchase orders feature is enabled.
    /// </summary>
    public bool FeaturePurchaseOrders { get; set; } = false;

    // Branding Settings
    /// <summary>
    /// Whether the tenant uses a custom theme instead of the default.
    /// </summary>
    public bool UseCustomTheme { get; set; } = false;

    /// <summary>
    /// Custom primary color in hex format (e.g., #E74C3C).
    /// Only applies when UseCustomTheme is true.
    /// </summary>
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// URL to the tenant's logo image.
    /// </summary>
    public string? LogoUrl { get; set; }

    // Notification Settings
    /// <summary>
    /// Whether email notifications are enabled.
    /// </summary>
    public bool EmailNotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Whether SMS notifications are enabled.
    /// </summary>
    public bool SmsNotificationsEnabled { get; set; } = false;

    /// <summary>
    /// Low stock notification threshold percentage.
    /// </summary>
    public decimal LowStockNotificationThreshold { get; set; } = 20.00m;

    // Navigation property
    /// <summary>
    /// The tenant this configuration belongs to.
    /// </summary>
    public Tenant Tenant { get; set; } = null!;
}
