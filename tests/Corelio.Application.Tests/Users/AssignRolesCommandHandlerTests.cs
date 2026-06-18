using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Users.Commands.AssignRoles;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Users;

[Trait("Category", "Unit")]
public class AssignRolesCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRoleRepository> _roleRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly AssignRolesCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public AssignRolesCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _roleRepoMock = new Mock<IRoleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new AssignRolesCommandHandler(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRoles_ReplacesExistingRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);

        // User currently has 1 role
        var oldRole = new Role { Id = Guid.NewGuid(), Name = "Cashier" };
        user.UserRoles.Add(new UserRole { UserId = userId, RoleId = oldRole.Id, Role = oldRole });

        var adminRole = new Role { Id = Guid.NewGuid(), Name = "Administrator" };

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(x => x.GetByCodesAsync(
                It.Is<IEnumerable<string>>(codes => codes.Contains("Administrator")),
                _tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([adminRole]);

        // Act
        var result = await _handler.Handle(new AssignRolesCommand(userId, ["Administrator"]), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.UserRoles.Should().HaveCount(1);
        user.UserRoles.First().RoleId.Should().Be(adminRole.Id);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(new AssignRolesCommand(Guid.NewGuid(), ["Admin"]), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(new AssignRolesCommand(userId, ["Administrator"]), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("User.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenRoleCodeIsInvalid_ReturnsValidationError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Repo returns empty list — no roles found for the requested code
        _roleRepoMock.Setup(x => x.GetByCodesAsync(
                It.IsAny<IEnumerable<string>>(), _tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(
            new AssignRolesCommand(userId, ["NonExistentRole"]), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("User.InvalidRoles");
        result.Error.Type.Should().Be(ErrorType.Validation);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmptyRoles_ClearsAllExistingRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        var existingRole = new Role { Id = Guid.NewGuid(), Name = "Cashier" };
        user.UserRoles.Add(new UserRole { UserId = userId, RoleId = existingRole.Id, Role = existingRole });

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(x => x.GetByCodesAsync(
                It.IsAny<IEnumerable<string>>(), _tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new AssignRolesCommand(userId, []), CancellationToken.None);

        // Assert — empty list → 0 codes requested → 0 found, counts match → success
        result.IsSuccess.Should().BeTrue();
        user.UserRoles.Should().BeEmpty();
    }

    private static User CreateUser(Guid id) =>
        new()
        {
            Id = id,
            TenantId = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
}
