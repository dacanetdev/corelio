using Corelio.Application.Products.Commands.CreateProduct;
using Corelio.Application.Products.Commands.UpdateProduct;
using Corelio.WebAPI.Contracts.Products;

namespace Corelio.WebAPI.Extensions;

/// <summary>
/// Extension methods for mapping product request DTOs to commands.
/// </summary>
public static class ProductMappingExtensions
{
    /// <summary>
    /// Maps CreateProductRequest to CreateProductCommand.
    /// </summary>
    public static CreateProductCommand ToCommand(this CreateProductRequest request) =>
        new(
            Sku: request.Sku,
            Name: request.Name,
            SalePrice: request.SalePrice,
            UnitOfMeasure: request.UnitOfMeasure,
            CategoryId: request.CategoryId,
            Barcode: request.Barcode,
            BarcodeType: request.BarcodeType,
            Description: request.Description,
            ShortDescription: request.ShortDescription,
            Brand: request.Brand,
            Manufacturer: request.Manufacturer,
            ModelNumber: request.ModelNumber,
            CostPrice: request.CostPrice,
            WholesalePrice: request.WholesalePrice,
            Msrp: request.Msrp,
            TaxRate: request.TaxRate,
            IsTaxExempt: request.IsTaxExempt,
            TrackInventory: request.TrackInventory,
            MinStockLevel: request.MinStockLevel,
            MaxStockLevel: request.MaxStockLevel,
            ReorderPoint: request.ReorderPoint,
            ReorderQuantity: request.ReorderQuantity,
            WeightKg: request.WeightKg,
            LengthCm: request.LengthCm,
            WidthCm: request.WidthCm,
            HeightCm: request.HeightCm,
            SatProductCode: request.SatProductCode,
            SatUnitCode: request.SatUnitCode,
            SatHazardousMaterial: request.SatHazardousMaterial,
            PrimaryImageUrl: request.PrimaryImageUrl,
            IsService: request.IsService,
            IsBundle: request.IsBundle,
            IsActive: request.IsActive,
            IsFeatured: request.IsFeatured);

    /// <summary>
    /// Maps UpdateProductRequest to UpdateProductCommand.
    /// </summary>
    public static UpdateProductCommand ToCommand(this UpdateProductRequest request, Guid id) =>
        new(
            Id: id,
            Sku: request.Sku,
            Name: request.Name,
            SalePrice: request.SalePrice,
            UnitOfMeasure: request.UnitOfMeasure,
            CategoryId: request.CategoryId,
            Barcode: request.Barcode,
            BarcodeType: request.BarcodeType,
            Description: request.Description,
            ShortDescription: request.ShortDescription,
            Brand: request.Brand,
            Manufacturer: request.Manufacturer,
            ModelNumber: request.ModelNumber,
            CostPrice: request.CostPrice,
            WholesalePrice: request.WholesalePrice,
            Msrp: request.Msrp,
            TaxRate: request.TaxRate,
            IsTaxExempt: request.IsTaxExempt,
            TrackInventory: request.TrackInventory,
            MinStockLevel: request.MinStockLevel,
            MaxStockLevel: request.MaxStockLevel,
            ReorderPoint: request.ReorderPoint,
            ReorderQuantity: request.ReorderQuantity,
            WeightKg: request.WeightKg,
            LengthCm: request.LengthCm,
            WidthCm: request.WidthCm,
            HeightCm: request.HeightCm,
            VolumeCm3: request.VolumeCm3,
            SatProductCode: request.SatProductCode,
            SatUnitCode: request.SatUnitCode,
            SatHazardousMaterial: request.SatHazardousMaterial,
            PrimaryImageUrl: request.PrimaryImageUrl,
            ImagesJson: request.ImagesJson,
            IsService: request.IsService,
            IsBundle: request.IsBundle,
            IsVariantParent: request.IsVariantParent,
            IsActive: request.IsActive,
            IsFeatured: request.IsFeatured);
}
