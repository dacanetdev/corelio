using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the TenantConfiguration entity.
/// </summary>
public class TenantConfigurationEntityConfiguration : IEntityTypeConfiguration<TenantConfiguration>
{
    public void Configure(EntityTypeBuilder<TenantConfiguration> builder)
    {
        builder.ToTable("tenant_configurations");

        // Primary key
        builder.HasKey(tc => tc.Id);
        builder.Property(tc => tc.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(tc => tc.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // CFDI Settings
        builder.Property(tc => tc.CfdiPacProvider)
            .HasColumnName("cfdi_pac_provider")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(tc => tc.CfdiPacApiUrl)
            .HasColumnName("cfdi_pac_api_url")
            .HasMaxLength(500);

        builder.Property(tc => tc.CfdiPacApiKey)
            .HasColumnName("cfdi_pac_api_key")
            .HasMaxLength(500);

        builder.Property(tc => tc.CfdiPacTestMode)
            .HasColumnName("cfdi_pac_test_mode")
            .HasDefaultValue(true);

        builder.Property(tc => tc.CfdiCertificatePath)
            .HasColumnName("cfdi_certificate_path")
            .HasMaxLength(500);

        builder.Property(tc => tc.CfdiKeyPath)
            .HasColumnName("cfdi_key_path")
            .HasMaxLength(500);

        builder.Property(tc => tc.CfdiCertificatePassword)
            .HasColumnName("cfdi_certificate_password")
            .HasMaxLength(500);

        builder.Property(tc => tc.CfdiCertificateExpiresAt)
            .HasColumnName("cfdi_certificate_expires_at");

        builder.Property(tc => tc.CfdiSeries)
            .HasColumnName("cfdi_series")
            .HasMaxLength(10)
            .HasDefaultValue("A");

        builder.Property(tc => tc.CfdiNextFolio)
            .HasColumnName("cfdi_next_folio")
            .HasDefaultValue(1);

        // Business Settings
        builder.Property(tc => tc.DefaultWarehouseId)
            .HasColumnName("default_warehouse_id");

        builder.Property(tc => tc.DefaultTaxRate)
            .HasColumnName("default_tax_rate")
            .HasPrecision(5, 4)
            .HasDefaultValue(0.1600m);

        builder.Property(tc => tc.Currency)
            .HasColumnName("currency")
            .HasMaxLength(3)
            .HasDefaultValue("MXN");

        builder.Property(tc => tc.Timezone)
            .HasColumnName("timezone")
            .HasMaxLength(50)
            .HasDefaultValue("America/Mexico_City");

        builder.Property(tc => tc.BusinessHoursStart)
            .HasColumnName("business_hours_start");

        builder.Property(tc => tc.BusinessHoursEnd)
            .HasColumnName("business_hours_end");

        // POS Settings
        builder.Property(tc => tc.PosAutoPrintReceipt)
            .HasColumnName("pos_auto_print_receipt")
            .HasDefaultValue(false);

        builder.Property(tc => tc.PosRequireCustomer)
            .HasColumnName("pos_require_customer")
            .HasDefaultValue(false);

        builder.Property(tc => tc.PosDefaultPaymentMethod)
            .HasColumnName("pos_default_payment_method")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PaymentMethod.Cash);

        builder.Property(tc => tc.PosEnableBarcodeScanner)
            .HasColumnName("pos_enable_barcode_scanner")
            .HasDefaultValue(true);

        builder.Property(tc => tc.PosThermalPrinterName)
            .HasColumnName("pos_thermal_printer_name")
            .HasMaxLength(100);

        builder.Property(tc => tc.PosReceiptFooter)
            .HasColumnName("pos_receipt_footer")
            .HasMaxLength(500);

        // Pricing Settings
        builder.Property(tc => tc.AllowNegativeInventory)
            .HasColumnName("allow_negative_inventory")
            .HasDefaultValue(false);

        builder.Property(tc => tc.RequireProductCost)
            .HasColumnName("require_product_cost")
            .HasDefaultValue(true);

        builder.Property(tc => tc.AutoCalculateMargin)
            .HasColumnName("auto_calculate_margin")
            .HasDefaultValue(true);

        // Feature Flags
        builder.Property(tc => tc.FeatureMultiWarehouse)
            .HasColumnName("feature_multi_warehouse")
            .HasDefaultValue(false);

        builder.Property(tc => tc.FeatureEcommerce)
            .HasColumnName("feature_ecommerce")
            .HasDefaultValue(false);

        builder.Property(tc => tc.FeatureLoyaltyProgram)
            .HasColumnName("feature_loyalty_program")
            .HasDefaultValue(false);

        builder.Property(tc => tc.FeaturePurchaseOrders)
            .HasColumnName("feature_purchase_orders")
            .HasDefaultValue(false);

        // Notification Settings
        builder.Property(tc => tc.EmailNotificationsEnabled)
            .HasColumnName("email_notifications_enabled")
            .HasDefaultValue(true);

        builder.Property(tc => tc.SmsNotificationsEnabled)
            .HasColumnName("sms_notifications_enabled")
            .HasDefaultValue(false);

        builder.Property(tc => tc.LowStockNotificationThreshold)
            .HasColumnName("low_stock_notification_threshold")
            .HasPrecision(5, 2)
            .HasDefaultValue(20.00m);

        // Audit properties from AuditableEntity
        builder.Property(tc => tc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(tc => tc.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(tc => tc.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(tc => tc.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(tc => tc.TenantId)
            .IsUnique()
            .HasDatabaseName("ix_tenant_configurations_tenant_id");

        // Query filter for multi-tenancy will be applied in DbContext
    }
}
