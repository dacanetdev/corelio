using Corelio.Application.Common.Models;
using Corelio.Application.Products.Commands.DeleteProduct;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Products.Commands;

[Trait("Category", "Unit")]
public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteProductCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    public DeleteProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new DeleteProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidProduct_ReturnsSuccessAndSoftDeletes()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        var command = new DeleteProductCommand(_productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        existingProduct.IsDeleted.Should().BeTrue();
        existingProduct.DeletedAt.Should().NotBeNull();
        existingProduct.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _productRepositoryMock.Verify(x => x.Update(existingProduct), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new DeleteProductCommand(_productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _productRepositoryMock.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductAlreadyDeleted_ReturnsValidationError()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        existingProduct.IsDeleted = true;
        existingProduct.DeletedAt = DateTime.UtcNow.AddDays(-1);

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        var command = new DeleteProductCommand(_productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.AlreadyDeleted");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _productRepositoryMock.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SetsDeletedAtToCurrentTime()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        var beforeDelete = DateTime.UtcNow;

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        var command = new DeleteProductCommand(_productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var afterDelete = DateTime.UtcNow;

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingProduct.DeletedAt.Should().NotBeNull();
        existingProduct.DeletedAt!.Value.Should().BeOnOrAfter(beforeDelete);
        existingProduct.DeletedAt!.Value.Should().BeOnOrBefore(afterDelete);
    }

    [Fact]
    public async Task Handle_PreservesOtherProductProperties()
    {
        // Arrange
        var existingProduct = CreateExistingProduct();
        var originalSku = existingProduct.Sku;
        var originalName = existingProduct.Name;
        var originalPrice = existingProduct.SalePrice;

        _productRepositoryMock.Setup(x => x.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        var command = new DeleteProductCommand(_productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingProduct.Sku.Should().Be(originalSku);
        existingProduct.Name.Should().Be(originalName);
        existingProduct.SalePrice.Should().Be(originalPrice);
        existingProduct.TenantId.Should().Be(_tenantId);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ReturnsNotFoundError()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.GetByIdAsync(Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new DeleteProductCommand(Guid.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Product.NotFound");
    }

    private Product CreateExistingProduct()
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
            IsDeleted = false,
            DeletedAt = null
        };
    }
}
