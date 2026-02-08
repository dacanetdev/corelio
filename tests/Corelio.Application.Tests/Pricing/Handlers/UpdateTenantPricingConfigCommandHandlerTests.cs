using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Commands.UpdateTenantPricingConfig;
using Corelio.Application.Pricing.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Pricing.Handlers;

[Trait("Category", "Unit")]
public class UpdateTenantPricingConfigCommandHandlerTests
{
    private readonly Mock<ITenantPricingConfigurationRepository> _configRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly UpdateTenantPricingConfigCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public UpdateTenantPricingConfigCommandHandlerTests()
    {
        _configRepositoryMock = new Mock<ITenantPricingConfigurationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new UpdateTenantPricingConfigCommandHandler(
            _configRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithNewConfig_CreatesAndReturnsConfig()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantPricingConfiguration?)null);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.DiscountTierCount.Should().Be(3);
        result.Value.MarginTierCount.Should().Be(3);
        result.Value.DefaultIvaEnabled.Should().BeTrue();
        result.Value.IvaPercentage.Should().Be(16.00m);
        result.Value.DiscountTiers.Should().HaveCount(3);
        result.Value.MarginTiers.Should().HaveCount(3);
        _configRepositoryMock.Verify(x => x.Add(It.IsAny<TenantPricingConfiguration>()), Times.Once);
        _configRepositoryMock.Verify(x => x.Update(It.IsAny<TenantPricingConfiguration>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingConfig_UpdatesAndReturnsConfig()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);

        var existingConfig = CreateDefaultConfig(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConfig);

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _configRepositoryMock.Verify(x => x.Update(It.IsAny<TenantPricingConfiguration>()), Times.Once);
        _configRepositoryMock.Verify(x => x.Add(It.IsAny<TenantPricingConfiguration>()), Times.Never);
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
        _configRepositoryMock.Verify(x => x.Add(It.IsAny<TenantPricingConfiguration>()), Times.Never);
        _configRepositoryMock.Verify(x => x.Update(It.IsAny<TenantPricingConfiguration>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SavesChanges()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _configRepositoryMock
            .Setup(x => x.GetWithTierDefinitionsAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantPricingConfiguration?)null);

        var command = CreateValidCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UpdateTenantPricingConfigCommand CreateValidCommand() => new(
        DiscountTierCount: 3,
        MarginTierCount: 3,
        DefaultIvaEnabled: true,
        IvaPercentage: 16.00m,
        DiscountTiers:
        [
            new DiscountTierDto(1, "Descuento 1", true),
            new DiscountTierDto(2, "Descuento 2", true),
            new DiscountTierDto(3, "Descuento 3", true)
        ],
        MarginTiers:
        [
            new MarginTierDto(1, "Publico", true),
            new MarginTierDto(2, "Mayoreo", true),
            new MarginTierDto(3, "Especial", true)
        ]);

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
