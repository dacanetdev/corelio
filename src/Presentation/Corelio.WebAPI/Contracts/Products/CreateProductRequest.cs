using Corelio.Domain.Enums;

namespace Corelio.WebAPI.Contracts.Products;

/// <summary>
/// Request DTO for creating a new product.
/// </summary>
public class CreateProductRequest
{
    // Product Identification (Required)
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal SalePrice { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.PCS;

    // Product Identification (Optional)
    public string? Barcode { get; set; }
    public BarcodeType BarcodeType { get; set; } = BarcodeType.EAN13;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    // Categorization
    public Guid? CategoryId { get; set; }
    public string? Brand { get; set; }
    public string? Manufacturer { get; set; }
    public string? ModelNumber { get; set; }

    // Pricing
    public decimal CostPrice { get; set; } = 0.00m;
    public decimal? WholesalePrice { get; set; }
    public decimal? Msrp { get; set; }

    // Tax
    public decimal TaxRate { get; set; } = 0.16m;
    public bool IsTaxExempt { get; set; }

    // Inventory Management
    public bool TrackInventory { get; set; } = true;
    public decimal MinStockLevel { get; set; } = 0m;
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
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
}
