using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Commands.CreateProduct;

/// <summary>
/// Handler for the CreateProductCommand that creates a new product within the current tenant.
/// </summary>
public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IProductCategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get current tenant ID (NEVER trust client input for tenant!)
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        // Step 2: Check if SKU already exists
        var skuExists = await productRepository.SkuExistsAsync(request.Sku, null, cancellationToken);
        if (skuExists)
        {
            return Result<Guid>.Failure(
                new Error("Product.SkuExists", $"A product with SKU '{request.Sku}' already exists.", ErrorType.Conflict));
        }

        // Step 3: Check if barcode already exists (if provided)
        if (!string.IsNullOrWhiteSpace(request.Barcode))
        {
            var barcodeExists = await productRepository.BarcodeExistsAsync(request.Barcode, null, cancellationToken);
            if (barcodeExists)
            {
                return Result<Guid>.Failure(
                    new Error("Product.BarcodeExists", $"A product with barcode '{request.Barcode}' already exists.", ErrorType.Conflict));
            }
        }

        // Step 4: Validate category exists (if provided)
        if (request.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                return Result<Guid>.Failure(
                    new Error("Product.InvalidCategory", $"Category with ID '{request.CategoryId}' not found.", ErrorType.Validation));
            }
        }

        // Step 5: Create product entity
        var product = new Product
        {
            TenantId = tenantId.Value,
            Sku = request.Sku,
            Name = request.Name,
            SalePrice = request.SalePrice,
            UnitOfMeasure = request.UnitOfMeasure,
            CategoryId = request.CategoryId,
            Barcode = request.Barcode,
            BarcodeType = request.BarcodeType,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            Brand = request.Brand,
            Manufacturer = request.Manufacturer,
            ModelNumber = request.ModelNumber,
            CostPrice = request.CostPrice,
            WholesalePrice = request.WholesalePrice,
            Msrp = request.Msrp,
            TaxRate = request.TaxRate,
            IsTaxExempt = request.IsTaxExempt,
            TrackInventory = request.TrackInventory,
            MinStockLevel = request.MinStockLevel,
            MaxStockLevel = request.MaxStockLevel,
            ReorderPoint = request.ReorderPoint,
            ReorderQuantity = request.ReorderQuantity,
            WeightKg = request.WeightKg,
            LengthCm = request.LengthCm,
            WidthCm = request.WidthCm,
            HeightCm = request.HeightCm,
            VolumeCm3 = request.VolumeCm3,
            SatProductCode = request.SatProductCode,
            SatUnitCode = request.SatUnitCode,
            SatHazardousMaterial = request.SatHazardousMaterial,
            PrimaryImageUrl = request.PrimaryImageUrl,
            ImagesJson = request.ImagesJson,
            IsService = request.IsService,
            IsBundle = request.IsBundle,
            IsVariantParent = request.IsVariantParent,
            IsActive = request.IsActive,
            IsFeatured = request.IsFeatured
        };

        productRepository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(product.Id);
    }
}
