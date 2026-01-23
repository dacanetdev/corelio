namespace Corelio.BlazorApp.Models.Products;

/// <summary>
/// Lightweight product DTO for list views and tables.
/// </summary>
public class ProductListDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal SalePrice { get; set; }
    public decimal CostPrice { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public string? Barcode { get; set; }
    public string? Brand { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public decimal ProfitMargin { get; set; }
    public string? PrimaryImageUrl { get; set; }
}
