using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.Application.Products.Queries.GetProducts;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using MapsterMapper;
using Moq;

namespace Corelio.Application.Tests.Products.Queries;

[Trait("Category", "Unit")]
public class GetProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProductsQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetProductsQueryHandler(
            _productRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithDefaultParameters_ReturnsPagedResult()
    {
        // Arrange
        var products = CreateProductList(5);
        var productDtos = CreateProductListDtos(5);

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 5));
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(products))
            .Returns(productDtos);

        var query = new GetProductsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(5);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(20);
        result.Value.TotalCount.Should().Be(5);
    }

    [Fact]
    public async Task Handle_WithPagination_PassesCorrectParametersToRepository()
    {
        // Arrange
        var products = CreateProductList(10);
        var productDtos = CreateProductListDtos(10);

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                2, 10, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 25));
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(products))
            .Returns(productDtos);

        var query = new GetProductsQuery(PageNumber: 2, PageSize: 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(2);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(25);
        result.Value.TotalPages.Should().Be(3);
        _productRepositoryMock.Verify(x => x.GetPagedAsync(
            2, 10, null, null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_PassesSearchTermToRepository()
    {
        // Arrange
        var products = CreateProductList(2);
        var productDtos = CreateProductListDtos(2);

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, "hammer", null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 2));
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(products))
            .Returns(productDtos);

        var query = new GetProductsQuery(SearchTerm: "hammer");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        _productRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, "hammer", null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_PassesCategoryIdToRepository()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var products = CreateProductList(3);
        var productDtos = CreateProductListDtos(3);

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, categoryId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 3));
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(products))
            .Returns(productDtos);

        var query = new GetProductsQuery(CategoryId: categoryId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, null, categoryId, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithActiveFilter_PassesIsActiveToRepository()
    {
        // Arrange
        var products = CreateProductList(4);
        var productDtos = CreateProductListDtos(4);

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 4));
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(products))
            .Returns(productDtos);

        var query = new GetProductsQuery(IsActive: true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.GetPagedAsync(
            1, 20, null, null, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAllFilters_PassesAllParametersToRepository()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var products = CreateProductList(1);
        var productDtos = CreateProductListDtos(1);

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                3, 15, "tool", categoryId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 1));
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(products))
            .Returns(productDtos);

        var query = new GetProductsQuery(
            PageNumber: 3,
            PageSize: 15,
            SearchTerm: "tool",
            CategoryId: categoryId,
            IsActive: true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.GetPagedAsync(
            3, 15, "tool", categoryId, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoProducts_ReturnsEmptyPagedResult()
    {
        // Arrange
        var emptyProducts = new List<Product>();
        var emptyDtos = new List<ProductListDto>();

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((emptyProducts, 0));
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(emptyProducts))
            .Returns(emptyDtos);

        var query = new GetProductsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
        result.Value.TotalPages.Should().Be(0);
        result.Value.HasPreviousPage.Should().BeFalse();
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_CalculatesCorrectPaginationMetadata()
    {
        // Arrange
        var products = CreateProductList(10);
        var productDtos = CreateProductListDtos(10);

        _productRepositoryMock.Setup(x => x.GetPagedAsync(
                2, 10, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 35)); // Total 35 items
        _mapperMock.Setup(x => x.Map<List<ProductListDto>>(products))
            .Returns(productDtos);

        var query = new GetProductsQuery(PageNumber: 2, PageSize: 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(2);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(35);
        result.Value.TotalPages.Should().Be(4); // 35 / 10 = 3.5, ceiling = 4
        result.Value.HasPreviousPage.Should().BeTrue();
        result.Value.HasNextPage.Should().BeTrue();
    }

    private List<Product> CreateProductList(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Product
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = $"PROD-{i:D3}",
                Name = $"Product {i}",
                SalePrice = 99.99m + i,
                CostPrice = 50.00m,
                UnitOfMeasure = UnitOfMeasure.PCS,
                IsActive = true
            })
            .ToList();
    }

    private static List<ProductListDto> CreateProductListDtos(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new ProductListDto(
                Id: Guid.NewGuid(),
                Sku: $"PROD-{i:D3}",
                Name: $"Product {i}",
                SalePrice: 99.99m + i,
                CostPrice: 50.00m,
                UnitOfMeasure: UnitOfMeasure.PCS,
                CategoryName: null,
                Barcode: null,
                Brand: null,
                IsActive: true,
                IsFeatured: false,
                ProfitMargin: 49.99m,
                PrimaryImageUrl: null))
            .ToList();
    }
}
