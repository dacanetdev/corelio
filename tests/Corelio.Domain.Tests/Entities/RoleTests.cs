using Corelio.Domain.Entities;
using FluentAssertions;

namespace Corelio.Domain.Tests.Entities;

public class RoleTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var role = new Role();

        // Assert
        role.Id.Should().NotBeEmpty();
        role.IsSystemRole.Should().BeFalse();
        role.IsDefault.Should().BeFalse();
        role.TenantId.Should().BeNull();
    }

    [Fact]
    public void UserRoles_ShouldBeEmptyByDefault()
    {
        // Act
        var role = new Role();

        // Assert
        role.UserRoles.Should().NotBeNull();
        role.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void RolePermissions_ShouldBeEmptyByDefault()
    {
        // Act
        var role = new Role();

        // Assert
        role.RolePermissions.Should().NotBeNull();
        role.RolePermissions.Should().BeEmpty();
    }

    [Fact]
    public void SystemRoles_ShouldHaveCorrectGuids()
    {
        // Assert
        SystemRoles.Owner.Should().Be(new Guid("00000000-0000-0000-0000-000000000001"));
        SystemRoles.Administrator.Should().Be(new Guid("00000000-0000-0000-0000-000000000002"));
        SystemRoles.Cashier.Should().Be(new Guid("00000000-0000-0000-0000-000000000003"));
        SystemRoles.InventoryManager.Should().Be(new Guid("00000000-0000-0000-0000-000000000004"));
        SystemRoles.Accountant.Should().Be(new Guid("00000000-0000-0000-0000-000000000005"));
        SystemRoles.Seller.Should().Be(new Guid("00000000-0000-0000-0000-000000000006"));
    }
}
