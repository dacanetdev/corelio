using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using FluentAssertions;

namespace Corelio.Domain.Tests.Entities;

[Trait("Category", "Unit")]
public class TenantTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var tenant = new Tenant();

        // Assert
        tenant.Id.Should().NotBeEmpty();
        tenant.SubscriptionPlan.Should().Be(SubscriptionPlan.Basic);
        tenant.MaxUsers.Should().Be(5);
        tenant.MaxProducts.Should().Be(1000);
        tenant.MaxSalesPerMonth.Should().Be(5000);
        tenant.IsActive.Should().BeTrue();
        tenant.IsTrial.Should().BeFalse();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = "Ferretería López",
            LegalName = "Ferretería López S.A. de C.V.",
            Rfc = "FLP901201ABC",
            Subdomain = "ferreteria-lopez",
            CustomDomain = "ferreteria-lopez.com",
            SubscriptionPlan = SubscriptionPlan.Premium,
            IsActive = true,
            IsTrial = true
        };

        // Assert
        tenant.Id.Should().Be(tenantId);
        tenant.Name.Should().Be("Ferretería López");
        tenant.LegalName.Should().Be("Ferretería López S.A. de C.V.");
        tenant.Rfc.Should().Be("FLP901201ABC");
        tenant.Subdomain.Should().Be("ferreteria-lopez");
        tenant.CustomDomain.Should().Be("ferreteria-lopez.com");
        tenant.SubscriptionPlan.Should().Be(SubscriptionPlan.Premium);
        tenant.IsTrial.Should().BeTrue();
    }

    [Fact]
    public void Users_ShouldBeEmptyByDefault()
    {
        // Act
        var tenant = new Tenant();

        // Assert
        tenant.Users.Should().NotBeNull();
        tenant.Users.Should().BeEmpty();
    }

    [Fact]
    public void Roles_ShouldBeEmptyByDefault()
    {
        // Act
        var tenant = new Tenant();

        // Assert
        tenant.Roles.Should().NotBeNull();
        tenant.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Configuration_ShouldBeNullByDefault()
    {
        // Act
        var tenant = new Tenant();

        // Assert
        tenant.Configuration.Should().BeNull();
    }
}
