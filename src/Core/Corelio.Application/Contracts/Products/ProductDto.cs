using Corelio.Domain.Enums;

namespace Corelio.Application.Contracts.Products;

/// <summary>
/// Full product DTO for detail views.
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    // Product Identification
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public BarcodeType BarcodeType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    // Categorization
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Brand { get; set; }
    public string? Manufacturer { get; set; }
    public string? ModelNumber { get; set; }

    // Pricing
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? Msrp { get; set; }

    // Tax
    public decimal TaxRate { get; set; }
    public bool IsTaxExempt { get; set; }

    // Inventory Management
    public bool TrackInventory { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public decimal? ReorderPoint { get; set; }
    public decimal? ReorderQuantity { get; set; }

    // Physical Properties
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? VolumeCm3 { get; set; }

    // CFDI / SAT Compliance
    public string? SatProductCode { get; set; }
    public string? SatUnitCode { get; set; }
    public string? SatHazardousMaterial { get; set; }

    // Images and Media
    public string? PrimaryImageUrl { get; set; }
    public string? ImagesJson { get; set; }

    // Product Type
    public bool IsService { get; set; }
    public bool IsBundle { get; set; }
    public bool IsVariantParent { get; set; }

    // Status
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }

    // SEO (future feature - not yet supported in commands)
    public string? Slug { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    // Calculated Properties
    public decimal ProfitMargin { get; set; }
    public decimal MarkupPercentage { get; set; }

    // Audit Properties
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
