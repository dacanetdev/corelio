using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Products.Commands.CreateProduct;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Products.Commands;

[Trait("Category", "Unit")]
public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductCategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly CreateProductCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<IProductCategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccessWithProductId()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _productRepositoryMock.Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _productRepositoryMock.Verify(x => x.Add(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSkuExists_ReturnsConflictError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync("EXISTING-SKU", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = CreateValidCommand() with { Sku = "EXISTING-SKU" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.SkuExists");
        result.Error.Type.Should().Be(ErrorType.Conflict);
        _productRepositoryMock.Verify(x => x.Add(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBarcodeExists_ReturnsConflictError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _productRepositoryMock.Setup(x => x.BarcodeExistsAsync("7501234567890", null, It.IsAny<CancellationToken>()))
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
    public async Task Handle_WithNullBarcode_DoesNotCheckBarcodeExistence()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
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

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsValidationError()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
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
    public async Task Handle_WithValidCategory_CreatesProductWithCategoryId()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategory { Id = categoryId, Name = "Tools", TenantId = _tenantId };

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        Product? capturedProduct = null;
        _productRepositoryMock.Setup(x => x.Add(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p);

        var command = CreateValidCommand() with { CategoryId = categoryId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedProduct.Should().NotBeNull();
        capturedProduct!.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task Handle_SetsCorrectTenantIdOnProduct()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Product? capturedProduct = null;
        _productRepositoryMock.Setup(x => x.Add(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedProduct.Should().NotBeNull();
        capturedProduct!.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task Handle_MapsAllPropertiesCorrectly()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _productRepositoryMock.Setup(x => x.SkuExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Product? capturedProduct = null;
        _productRepositoryMock.Setup(x => x.Add(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p);

        var command = new CreateProductCommand(
            Sku: "TEST-001",
            Name: "Test Product",
            SalePrice: 199.99m,
            UnitOfMeasure: UnitOfMeasure.PCS,
            CategoryId: null,
            Barcode: "7501234567890",
            BarcodeType: BarcodeType.EAN13,
            Description: "Test description",
            ShortDescription: "Short desc",
            Brand: "TestBrand",
            Manufacturer: "TestManufacturer",
            ModelNumber: "MODEL-123",
            CostPrice: 100.00m,
            WholesalePrice: 150.00m,
            Msrp: 220.00m,
            TaxRate: 0.16m,
            IsTaxExempt: false,
            TrackInventory: true,
            MinStockLevel: 5,
            MaxStockLevel: 100,
            ReorderPoint: 10,
            ReorderQuantity: 20,
            WeightKg: 0.5m,
            LengthCm: 10m,
            WidthCm: 5m,
            HeightCm: 3m,
            VolumeCm3: 150m,
            SatProductCode: "12345678",
            SatUnitCode: "H87",
            SatHazardousMaterial: null,
            PrimaryImageUrl: "https://example.com/image.jpg",
            ImagesJson: null,
            IsService: false,
            IsBundle: false,
            IsVariantParent: false,
            IsActive: true,
            IsFeatured: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Sku.Should().Be("TEST-001");
        capturedProduct.Name.Should().Be("Test Product");
        capturedProduct.SalePrice.Should().Be(199.99m);
        capturedProduct.CostPrice.Should().Be(100.00m);
        capturedProduct.UnitOfMeasure.Should().Be(UnitOfMeasure.PCS);
        capturedProduct.Brand.Should().Be("TestBrand");
        capturedProduct.Manufacturer.Should().Be("TestManufacturer");
        capturedProduct.TaxRate.Should().Be(0.16m);
        capturedProduct.TrackInventory.Should().BeTrue();
        capturedProduct.IsActive.Should().BeTrue();
        capturedProduct.IsFeatured.Should().BeTrue();
        capturedProduct.SatProductCode.Should().Be("12345678");
        capturedProduct.SatUnitCode.Should().Be("H87");
    }

    private static CreateProductCommand CreateValidCommand()
    {
        return new CreateProductCommand(
            Sku: "PROD-001",
            Name: "Test Product",
            SalePrice: 99.99m,
            UnitOfMeasure: UnitOfMeasure.PCS);
    }
}
