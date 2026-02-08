using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.Application.Products.Queries.GetProductById;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using MapsterMapper;
using Moq;

namespace Corelio.Application.Tests.Products.Queries;

[Trait("Category", "Unit")]
public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProductByIdQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    public GetProductByIdQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetProductByIdQueryHandler(
            _productRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingProduct_ReturnsSuccessWithProductDto()
    {
        // Arrange
        var product = CreateProduct();
        var expectedDto = CreateProductDto();

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mapperMock.Setup(x => x.Map<ProductDto>(product))
            .Returns(expectedDto);

        var query = new GetProductByIdQuery(_productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().Be(expectedDto);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var query = new GetProductByIdQuery(_productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _mapperMock.Verify(x => x.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductIsDeleted_ReturnsNotFoundError()
    {
        // Arrange
        var product = CreateProduct();
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var query = new GetProductByIdQuery(_productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _mapperMock.Verify(x => x.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var product = CreateProduct();
        var expectedDto = CreateProductDto();

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mapperMock.Setup(x => x.Map<ProductDto>(product))
            .Returns(expectedDto);

        var query = new GetProductByIdQuery(_productId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _productRepositoryMock.Verify(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MapsProductToDto()
    {
        // Arrange
        var product = CreateProduct();
        var expectedDto = CreateProductDto();

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mapperMock.Setup(x => x.Map<ProductDto>(product))
            .Returns(expectedDto);

        var query = new GetProductByIdQuery(_productId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(x => x.Map<ProductDto>(product), Times.Once);
    }

    private Product CreateProduct()
    {
        return new Product
        {
            Id = _productId,
            TenantId = _tenantId,
            Sku = "TEST-SKU",
            Name = "Test Product",
            SalePrice = 99.99m,
            CostPrice = 50.00m,
            UnitOfMeasure = UnitOfMeasure.PCS,
            IsActive = true,
            IsDeleted = false
        };
    }

    private ProductDto CreateProductDto()
    {
        return new ProductDto(
            Id: _productId,
            Sku: "TEST-SKU",
            Name: "Test Product",
            SalePrice: 99.99m,
            CostPrice: 50.00m,
            UnitOfMeasure: UnitOfMeasure.PCS,
            CategoryId: null,
            CategoryName: null,
            Barcode: null,
            BarcodeType: BarcodeType.EAN13,
            Description: null,
            ShortDescription: null,
            Brand: null,
            Manufacturer: null,
            ModelNumber: null,
            WholesalePrice: null,
            Msrp: null,
            TaxRate: 0.16m,
            IsTaxExempt: false,
            TrackInventory: true,
            MinStockLevel: 0,
            MaxStockLevel: null,
            ReorderPoint: null,
            ReorderQuantity: null,
            WeightKg: null,
            LengthCm: null,
            WidthCm: null,
            HeightCm: null,
            VolumeCm3: null,
            SatProductCode: null,
            SatUnitCode: null,
            SatHazardousMaterial: null,
            PrimaryImageUrl: null,
            ImagesJson: null,
            IsService: false,
            IsBundle: false,
            IsVariantParent: false,
            IsActive: true,
            IsFeatured: false,
            ProfitMargin: 49.99m,
            MarkupPercentage: 99.98m,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow);
    }
}
