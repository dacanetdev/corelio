using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a product in the catalog with CFDI compliance fields.
/// </summary>
public class Product : TenantAuditableEntity, ISoftDeletable
{
    // Product Identification
    /// <summary>
    /// Stock Keeping Unit - unique identifier per tenant.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Product barcode (EAN13, UPC, etc.).
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Type of barcode used.
    /// </summary>
    public BarcodeType BarcodeType { get; set; } = BarcodeType.EAN13;

    /// <summary>
    /// Product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Short description for listings.
    /// </summary>
    public string? ShortDescription { get; set; }

    // Categorization
    /// <summary>
    /// Product category ID.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Product brand.
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Product manufacturer.
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// Model number.
    /// </summary>
    public string? ModelNumber { get; set; }

    // Pricing (in tenant's currency)
    /// <summary>
    /// Cost price (purchase price).
    /// </summary>
    public decimal CostPrice { get; set; } = 0.00m;

    /// <summary>
    /// Sale price (retail price).
    /// </summary>
    public decimal SalePrice { get; set; }

    /// <summary>
    /// Wholesale price for bulk purchases.
    /// </summary>
    public decimal? WholesalePrice { get; set; }

    /// <summary>
    /// Manufacturer's Suggested Retail Price.
    /// </summary>
    public decimal? Msrp { get; set; }

    /// <summary>
    /// List price ("Precio Lista") before cascading discounts are applied.
    /// </summary>
    public decimal? ListPrice { get; set; }

    /// <summary>
    /// Net cost ("Costo Neto") after applying all cascading discounts to ListPrice.
    /// Calculated as: ListPrice × (1-D1/100) × (1-D2/100) × ... × (1-Dn/100)
    /// </summary>
    public decimal? NetCost { get; set; }

    /// <summary>
    /// Whether IVA (VAT) is enabled for this product.
    /// </summary>
    public bool IvaEnabled { get; set; } = false;

    // Tax
    /// <summary>
    /// Tax rate (default 0.16 for IVA 16% in Mexico).
    /// </summary>
    public decimal TaxRate { get; set; } = 0.16m;

    /// <summary>
    /// Whether the product is tax exempt.
    /// </summary>
    public bool IsTaxExempt { get; set; } = false;

    // Inventory Management
    /// <summary>
    /// Whether to track inventory for this product.
    /// </summary>
    public bool TrackInventory { get; set; } = true;

    /// <summary>
    /// Unit of measure (PCS, KG, M, L, etc.).
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.PCS;

    /// <summary>
    /// Minimum stock level (triggers low stock alert).
    /// </summary>
    public decimal MinStockLevel { get; set; } = 0;

    /// <summary>
    /// Maximum stock level.
    /// </summary>
    public decimal? MaxStockLevel { get; set; }

    /// <summary>
    /// Reorder point - when to reorder stock.
    /// </summary>
    public decimal? ReorderPoint { get; set; }

    /// <summary>
    /// Quantity to reorder when stock reaches reorder point.
    /// </summary>
    public decimal? ReorderQuantity { get; set; }

    // Physical Properties
    /// <summary>
    /// Weight in kilograms.
    /// </summary>
    public decimal? WeightKg { get; set; }

    /// <summary>
    /// Length in centimeters.
    /// </summary>
    public decimal? LengthCm { get; set; }

    /// <summary>
    /// Width in centimeters.
    /// </summary>
    public decimal? WidthCm { get; set; }

    /// <summary>
    /// Height in centimeters.
    /// </summary>
    public decimal? HeightCm { get; set; }

    /// <summary>
    /// Volume in cubic centimeters.
    /// </summary>
    public decimal? VolumeCm3 { get; set; }

    // CFDI / SAT Compliance (Mexico)
    /// <summary>
    /// SAT product/service code (8 digits).
    /// Required for CFDI invoicing.
    /// </summary>
    public string? SatProductCode { get; set; }

    /// <summary>
    /// SAT unit code (2-3 characters, e.g., H87 for pieces).
    /// Required for CFDI invoicing.
    /// </summary>
    public string? SatUnitCode { get; set; }

    /// <summary>
    /// SAT hazardous material code (4 characters).
    /// Optional, only for hazardous materials.
    /// </summary>
    public string? SatHazardousMaterial { get; set; }

    // Images and Media
    /// <summary>
    /// URL of the primary product image.
    /// </summary>
    public string? PrimaryImageUrl { get; set; }

    /// <summary>
    /// JSON array of additional image URLs.
    /// </summary>
    public string? ImagesJson { get; set; }

    // Product Type
    /// <summary>
    /// Whether this is a service (not a physical product).
    /// </summary>
    public bool IsService { get; set; } = false;

    /// <summary>
    /// Whether this is a bundle of other products.
    /// </summary>
    public bool IsBundle { get; set; } = false;

    /// <summary>
    /// Whether this is a parent product with variants.
    /// </summary>
    public bool IsVariantParent { get; set; } = false;

    // Status
    /// <summary>
    /// Whether the product is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the product is featured.
    /// </summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>
    /// Indicates whether the product has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// The date and time when the product was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// The ID of the user who deleted this product.
    /// </summary>
    public Guid? DeletedBy { get; set; }

    // SEO (for future e-commerce)
    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Meta title for SEO.
    /// </summary>
    public string? MetaTitle { get; set; }

    /// <summary>
    /// Meta description for SEO.
    /// </summary>
    public string? MetaDescription { get; set; }

    /// <summary>
    /// Meta keywords for SEO.
    /// </summary>
    public string? MetaKeywords { get; set; }

    // Navigation properties
    /// <summary>
    /// Product category.
    /// </summary>
    public ProductCategory? Category { get; set; }

    /// <summary>
    /// Product discount tiers (cascading discounts applied to ListPrice).
    /// </summary>
    public List<ProductDiscount> Discounts { get; set; } = [];

    /// <summary>
    /// Product margin/price tiers (margin percentages and calculated prices).
    /// </summary>
    public List<ProductMarginPrice> MarginPrices { get; set; } = [];

    // Calculated Properties
    /// <summary>
    /// Profit margin percentage.
    /// </summary>
    public decimal ProfitMargin => CostPrice > 0 ? ((SalePrice - CostPrice) / CostPrice) * 100 : 0;

    /// <summary>
    /// Markup percentage.
    /// </summary>
    public decimal MarkupPercentage => CostPrice > 0 ? ((SalePrice - CostPrice) / SalePrice) * 100 : 0;
}
