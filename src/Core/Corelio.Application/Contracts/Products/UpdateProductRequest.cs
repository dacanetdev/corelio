using Corelio.Domain.Enums;

namespace Corelio.Application.Contracts.Products;

/// <summary>
/// Request DTO for updating a product.
/// </summary>
public class UpdateProductRequest
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal SalePrice { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.PCS;
    public Guid? CategoryId { get; set; }
    public string? Barcode { get; set; }
    public BarcodeType BarcodeType { get; set; } = BarcodeType.EAN13;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? Brand { get; set; }
    public string? Manufacturer { get; set; }
    public string? ModelNumber { get; set; }
    public decimal CostPrice { get; set; } = 0.00m;
    public decimal? WholesalePrice { get; set; }
    public decimal? Msrp { get; set; }
    public decimal TaxRate { get; set; } = 0.16m;
    public bool IsTaxExempt { get; set; }
    public bool TrackInventory { get; set; } = true;
    public decimal MinStockLevel { get; set; } = 0;
    public decimal? MaxStockLevel { get; set; }
    public decimal? ReorderPoint { get; set; }
    public decimal? ReorderQuantity { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? VolumeCm3 { get; set; }
    public string? SatProductCode { get; set; }
    public string? SatUnitCode { get; set; }
    public string? SatHazardousMaterial { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public string? ImagesJson { get; set; }
    public bool IsService { get; set; }
    public bool IsBundle { get; set; }
    public bool IsVariantParent { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }

    // SEO (future feature - not yet supported in commands)
    public string? Slug { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}
