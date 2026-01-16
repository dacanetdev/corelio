namespace Corelio.BlazorApp.Models.Products;

/// <summary>
/// Request model for updating an existing product.
/// </summary>
public class UpdateProductRequest
{
    public Guid Id { get; set; }

    // Product Identification
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string BarcodeType { get; set; } = "EAN13";
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    // Categorization
    public Guid? CategoryId { get; set; }
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
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public decimal? ReorderPoint { get; set; }
    public decimal? ReorderQuantity { get; set; }

    // Physical Properties
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }

    // CFDI / SAT Compliance
    public string? SatProductCode { get; set; }
    public string? SatUnitCode { get; set; }
    public string? SatHazardousMaterial { get; set; }

    // Images and Media
    public string? PrimaryImageUrl { get; set; }

    // Product Type
    public bool IsService { get; set; }
    public bool IsBundle { get; set; }

    // Status
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }

    // SEO
    public string? Slug { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}
