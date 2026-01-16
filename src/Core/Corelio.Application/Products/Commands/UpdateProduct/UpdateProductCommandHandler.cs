using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Handler for the UpdateProductCommand that updates an existing product.
/// </summary>
public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IProductCategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get existing product
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return Result<bool>.Failure(
                new Error("Product.NotFound", $"Product with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        // Step 2: Check if SKU already exists (excluding current product)
        var skuExists = await productRepository.SkuExistsAsync(request.Sku, request.Id, cancellationToken);
        if (skuExists)
        {
            return Result<bool>.Failure(
                new Error("Product.SkuExists", $"A product with SKU '{request.Sku}' already exists.", ErrorType.Conflict));
        }

        // Step 3: Check if barcode already exists (if provided, excluding current product)
        if (!string.IsNullOrWhiteSpace(request.Barcode))
        {
            var barcodeExists = await productRepository.BarcodeExistsAsync(request.Barcode, request.Id, cancellationToken);
            if (barcodeExists)
            {
                return Result<bool>.Failure(
                    new Error("Product.BarcodeExists", $"A product with barcode '{request.Barcode}' already exists.", ErrorType.Conflict));
            }
        }

        // Step 4: Validate category exists (if provided)
        if (request.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                return Result<bool>.Failure(
                    new Error("Product.InvalidCategory", $"Category with ID '{request.CategoryId}' not found.", ErrorType.Validation));
            }
        }

        // Step 5: Update product properties
        product.Sku = request.Sku;
        product.Name = request.Name;
        product.SalePrice = request.SalePrice;
        product.UnitOfMeasure = request.UnitOfMeasure;
        product.CategoryId = request.CategoryId;
        product.Barcode = request.Barcode;
        product.BarcodeType = request.BarcodeType;
        product.Description = request.Description;
        product.ShortDescription = request.ShortDescription;
        product.Brand = request.Brand;
        product.Manufacturer = request.Manufacturer;
        product.ModelNumber = request.ModelNumber;
        product.CostPrice = request.CostPrice;
        product.WholesalePrice = request.WholesalePrice;
        product.Msrp = request.Msrp;
        product.TaxRate = request.TaxRate;
        product.IsTaxExempt = request.IsTaxExempt;
        product.TrackInventory = request.TrackInventory;
        product.MinStockLevel = request.MinStockLevel;
        product.MaxStockLevel = request.MaxStockLevel;
        product.ReorderPoint = request.ReorderPoint;
        product.ReorderQuantity = request.ReorderQuantity;
        product.WeightKg = request.WeightKg;
        product.LengthCm = request.LengthCm;
        product.WidthCm = request.WidthCm;
        product.HeightCm = request.HeightCm;
        product.VolumeCm3 = request.VolumeCm3;
        product.SatProductCode = request.SatProductCode;
        product.SatUnitCode = request.SatUnitCode;
        product.SatHazardousMaterial = request.SatHazardousMaterial;
        product.PrimaryImageUrl = request.PrimaryImageUrl;
        product.ImagesJson = request.ImagesJson;
        product.IsService = request.IsService;
        product.IsBundle = request.IsBundle;
        product.IsVariantParent = request.IsVariantParent;
        product.IsActive = request.IsActive;
        product.IsFeatured = request.IsFeatured;

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
