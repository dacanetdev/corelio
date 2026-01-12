using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Corelio.Domain.Entities;
using Corelio.Infrastructure.MultiTenancy;
using Corelio.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace Corelio.Infrastructure.Tests.MultiTenancy;

[Trait("Category", "Unit")]
public class TenantServiceTests
{
    private readonly Mock<TenantProvider> _tenantProviderMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<ApplicationDbContext> _dbContextMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<ILogger<TenantService>> _loggerMock;
    private readonly TenantService _sut;
    private readonly HttpContext _httpContext;

    public TenantServiceTests()
    {
        _tenantProviderMock = new Mock<TenantProvider>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _dbContextMock = new Mock<ApplicationDbContext>();
        _cacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<TenantService>>();

        _httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContext);

        _sut = new TenantService(
            _tenantProviderMock.Object,
            _httpContextAccessorMock.Object,
            _dbContextMock.Object,
            _cacheMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void GetCurrentTenantId_WhenTenantContextExists_ReturnsCachedTenantId()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(true);
        _tenantProviderMock.Setup(x => x.TenantId).Returns(expectedTenantId);

        // Act
        var result = _sut.GetCurrentTenantId();

        // Assert
        result.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GetCurrentTenantId_WhenNoHttpContext_ReturnsNull()
    {
        // Arrange
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(false);

        // Act
        var result = _sut.GetCurrentTenantId();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentTenantId_WhenJwtClaimExists_ReturnsTenantIdFromClaim()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(false);

        var claims = new[]
        {
            new Claim("tenant_id", expectedTenantId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _httpContext.User = principal;

        // Act
        var result = _sut.GetCurrentTenantId();

        // Assert
        result.Should().Be(expectedTenantId);
        _tenantProviderMock.Verify(x => x.SetTenant(expectedTenantId), Times.Once);
    }

    [Fact]
    public void GetCurrentTenantId_WhenHeaderExists_ReturnsTenantIdFromHeader()
    {
        // Arrange
        var expectedTenantId = Guid.NewGuid();
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(false);
        _httpContext.Request.Headers["X-Tenant-ID"] = expectedTenantId.ToString();

        // Act
        var result = _sut.GetCurrentTenantId();

        // Assert
        result.Should().Be(expectedTenantId);
        _tenantProviderMock.Verify(x => x.SetTenant(expectedTenantId), Times.Once);
    }

    [Fact]
    public void GetCurrentTenantId_WhenNoTenantSource_ReturnsNull()
    {
        // Arrange
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(false);

        // Act
        var result = _sut.GetCurrentTenantId();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SetCurrentTenantId_WithValidTenantId_SetsTenantInProvider()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        _sut.SetCurrentTenantId(tenantId);

        // Assert
        _tenantProviderMock.Verify(x => x.SetTenant(tenantId), Times.Once);
    }

    [Fact]
    public void SetCurrentTenantId_WithNull_ClearsTenantContext()
    {
        // Act
        _sut.SetCurrentTenantId(null);

        // Assert
        _tenantProviderMock.Verify(x => x.ClearTenant(), Times.Once);
    }

    [Fact]
    public async Task TenantExistsAsync_WhenTenantInCache_ReturnsTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var cachedData = Encoding.UTF8.GetBytes("{}");
        _cacheMock.Setup(x => x.GetAsync($"tenant:{tenantId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        // Act
        var result = await _sut.TenantExistsAsync(tenantId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetTenantBySubdomainAsync_WithNull_ReturnsNull()
    {
        // Act
        var result = await _sut.GetTenantBySubdomainAsync(null!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTenantBySubdomainAsync_WhenInCache_ReturnsCachedTenant()
    {
        // Arrange
        var subdomain = "test-subdomain";
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Subdomain = subdomain,
            Name = "Test Business",
            LegalName = "Test Business SA de CV",
            Rfc = "TST123456ABC"
        };
        var cachedData = JsonSerializer.Serialize(tenant);
        var cachedBytes = Encoding.UTF8.GetBytes(cachedData);

        _cacheMock.Setup(x => x.GetAsync($"tenant:subdomain:{subdomain}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedBytes);

        // Act
        var result = await _sut.GetTenantBySubdomainAsync(subdomain);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tenant.Id);
        result.Subdomain.Should().Be(subdomain);
    }

    [Fact]
    public async Task BypassTenantFilterAsync_ExecutesOperationWithoutTenantContext()
    {
        // Arrange
        var originalTenantId = Guid.NewGuid();
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(true);
        _tenantProviderMock.Setup(x => x.TenantId).Returns(originalTenantId);

        var operationExecuted = false;
        Func<Task<bool>> operation = async () =>
        {
            await Task.Delay(1);
            operationExecuted = true;
            return true;
        };

        // Act
        var result = await _sut.BypassTenantFilterAsync(operation);

        // Assert
        result.Should().BeTrue();
        operationExecuted.Should().BeTrue();
        _tenantProviderMock.Verify(x => x.ClearTenant(), Times.Once);
        _tenantProviderMock.Verify(x => x.SetTenant(originalTenantId), Times.Once);
    }

    [Fact]
    public async Task BypassTenantFilterAsync_RestoresTenantContextAfterException()
    {
        // Arrange
        var originalTenantId = Guid.NewGuid();
        _tenantProviderMock.Setup(x => x.HasTenantContext).Returns(true);
        _tenantProviderMock.Setup(x => x.TenantId).Returns(originalTenantId);

        Func<Task<bool>> operation = () => throw new InvalidOperationException("Test exception");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _sut.BypassTenantFilterAsync(operation));

        _tenantProviderMock.Verify(x => x.SetTenant(originalTenantId), Times.Once);
    }

    [Fact]
    public async Task InvalidateTenantCacheAsync_RemovesTenantFromCache()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        await _sut.InvalidateTenantCacheAsync(tenantId);

        // Assert
        _cacheMock.Verify(x => x.RemoveAsync($"tenant:{tenantId}", It.IsAny<CancellationToken>()), Times.Once);
    }
}
