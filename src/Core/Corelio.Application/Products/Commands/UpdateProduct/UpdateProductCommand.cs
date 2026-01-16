using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Command to update an existing product.
/// </summary>
public record UpdateProductCommand(
    Guid Id,
    string Sku,
    string Name,
    decimal SalePrice,
    UnitOfMeasure UnitOfMeasure,
    Guid? CategoryId = null,
    string? Barcode = null,
    BarcodeType BarcodeType = BarcodeType.EAN13,
    string? Description = null,
    string? ShortDescription = null,
    string? Brand = null,
    string? Manufacturer = null,
    string? ModelNumber = null,
    decimal CostPrice = 0.00m,
    decimal? WholesalePrice = null,
    decimal? Msrp = null,
    decimal TaxRate = 0.16m,
    bool IsTaxExempt = false,
    bool TrackInventory = true,
    decimal MinStockLevel = 0,
    decimal? MaxStockLevel = null,
    decimal? ReorderPoint = null,
    decimal? ReorderQuantity = null,
    decimal? WeightKg = null,
    decimal? LengthCm = null,
    decimal? WidthCm = null,
    decimal? HeightCm = null,
    decimal? VolumeCm3 = null,
    string? SatProductCode = null,
    string? SatUnitCode = null,
    string? SatHazardousMaterial = null,
    string? PrimaryImageUrl = null,
    string? ImagesJson = null,
    bool IsService = false,
    bool IsBundle = false,
    bool IsVariantParent = false,
    bool IsActive = true,
    bool IsFeatured = false) : IRequest<Result<bool>>;
