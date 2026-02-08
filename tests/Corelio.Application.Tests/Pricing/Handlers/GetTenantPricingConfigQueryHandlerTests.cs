using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Queries.GetTenantPricingConfig;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Pricing.Handlers;

[Trait("Category", "Unit")]
public class GetTenantPricingConfigQueryHandlerTests
{
    private readonly Mock<ITenantPricingConfigurationRepository> _configRepositoryMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly GetTenantPricingConfigQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetTenantPricingConfigQueryHandlerTests()
    {
        _configRepositoryMock = new Mock<ITenantPricingConfigurationRepository>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new GetTenantPricingConfigQueryHandler(
            _configRepositoryMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidTenant_ReturnsConfig()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var config = CreateDefaultConfig(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var query = new GetTenantPricingConfigQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TenantId.Should().Be(_tenantId);
        result.Value.DiscountTierCount.Should().Be(3);
        result.Value.MarginTierCount.Should().Be(3);
        result.Value.DefaultIvaEnabled.Should().BeTrue();
        result.Value.IvaPercentage.Should().Be(16.00m);
        result.Value.DiscountTiers.Should().HaveCount(3);
        result.Value.MarginTiers.Should().HaveCount(3);
        result.Value.DiscountTiers[0].TierName.Should().Be("Descuento 1");
        result.Value.MarginTiers[0].TierName.Should().Be("Publico");
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var query = new GetTenantPricingConfigQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _configRepositoryMock.Verify(
            x => x.GetWithTierDefinitionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ReturnsNotFoundError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantPricingConfiguration?)null);

        var query = new GetTenantPricingConfigQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("PricingConfig.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    private static TenantPricingConfiguration CreateDefaultConfig(Guid tenantId) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        DiscountTierCount = 3,
        MarginTierCount = 3,
        DefaultIvaEnabled = true,
        IvaPercentage = 16.00m,
        DiscountTierDefinitions =
        [
            new() { TierNumber = 1, TierName = "Descuento 1", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 2, TierName = "Descuento 2", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 3, TierName = "Descuento 3", IsActive = true, TenantId = tenantId }
        ],
        MarginTierDefinitions =
        [
            new() { TierNumber = 1, TierName = "Publico", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 2, TierName = "Mayoreo", IsActive = true, TenantId = tenantId },
            new() { TierNumber = 3, TierName = "Especial", IsActive = true, TenantId = tenantId }
        ]
    };
}
