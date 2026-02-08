using Corelio.Application.Common.Models;
using Corelio.Application.Products.Commands.UpdateProduct;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Products.Commands;

[Trait("Category", "Unit")]
public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductCategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateProductCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    public UpdateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<IProductCategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UpdateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _productRepositoryMock.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSkuExistsOnDifferentProduct_ReturnsConflictError()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync("EXISTING-SKU", _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = CreateValidCommand() with { Sku = "EXISTING-SKU" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.SkuExists");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_WhenBarcodeExistsOnDifferentProduct_ReturnsConflictError()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _productRepositoryMock.Setup(x => x.BarcodeExistsAsync("7501234567890", _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = CreateValidCommand() with { Barcode = "7501234567890" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.BarcodeExists");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsValidationError()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingProduct = CreateExistingProduct();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductCategory?)null);

        var command = CreateValidCommand() with { CategoryId = categoryId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.InvalidCategory");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_UpdatesAllProductProperties()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateProductCommand(
            Id: _productId,
            Sku: "UPDATED-SKU",
            Name: "Updated Product Name",
            SalePrice: 299.99m,
            UnitOfMeasure: UnitOfMeasure.KG,
            CategoryId: null,
            Barcode: "9999999999999",
            BarcodeType: BarcodeType.UPC,
            Description: "Updated description",
            ShortDescription: "Updated short",
            Brand: "UpdatedBrand",
            Manufacturer: "UpdatedManufacturer",
            ModelNumber: "UPDATED-MODEL",
            CostPrice: 150.00m,
            WholesalePrice: 200.00m,
            Msrp: 350.00m,
            TaxRate: 0.08m,
            IsTaxExempt: true,
            TrackInventory: false,
            MinStockLevel: 10,
            MaxStockLevel: 200,
            ReorderPoint: 20,
            ReorderQuantity: 50,
            WeightKg: 2.5m,
            LengthCm: 20m,
            WidthCm: 15m,
            HeightCm: 10m,
            VolumeCm3: 3000m,
            SatProductCode: "87654321",
            SatUnitCode: "KGM",
            SatHazardousMaterial: null,
            PrimaryImageUrl: "https://example.com/updated.jpg",
            ImagesJson: null,
            IsService: true,
            IsBundle: true,
            IsVariantParent: true,
            IsActive: false,
            IsFeatured: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingProduct.Sku.Should().Be("UPDATED-SKU");
        existingProduct.Name.Should().Be("Updated Product Name");
        existingProduct.SalePrice.Should().Be(299.99m);
        existingProduct.CostPrice.Should().Be(150.00m);
        existingProduct.UnitOfMeasure.Should().Be(UnitOfMeasure.KG);
        existingProduct.Brand.Should().Be("UpdatedBrand");
        existingProduct.IsTaxExempt.Should().BeTrue();
        existingProduct.TrackInventory.Should().BeFalse();
        existingProduct.IsActive.Should().BeFalse();
        existingProduct.IsService.Should().BeTrue();
        existingProduct.SatProductCode.Should().Be("87654321");
    }

    [Fact]
    public async Task Handle_WithSameSkuAsExistingProduct_AllowsUpdate()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        existingProduct.Sku = "EXISTING-SKU";

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync("EXISTING-SKU", _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Returns false because we exclude current product

        var command = CreateValidCommand() with { Sku = "EXISTING-SKU" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNullBarcode_DoesNotCheckBarcodeExistence()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), _productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = CreateValidCommand() with { Barcode = null };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _productRepositoryMock.Verify(
            x => x.BarcodeExistsAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private Product CreateExistingProduct()
    {
        return new Product
        {
            Id = _productId,
            TenantId = _tenantId,
            Sku = "ORIGINAL-SKU",
            Name = "Original Product",
            SalePrice = 99.99m,
            CostPrice = 50.00m,
            UnitOfMeasure = UnitOfMeasure.PCS,
            IsActive = true
        };
    }

    private UpdateProductCommand CreateValidCommand()
    {
        return new UpdateProductCommand(
            Id: _productId,
            Sku: "PROD-001",
            Name: "Updated Product",
            SalePrice: 149.99m,
            UnitOfMeasure: UnitOfMeasure.PCS);
    }
}
