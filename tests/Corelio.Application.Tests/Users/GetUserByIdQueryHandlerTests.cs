using Corelio.Application.Common.Models;
using Corelio.Application.Users.Queries.GetUserById;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Users;

[Trait("Category", "Unit")]
public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsMappedDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = new Role { Id = Guid.NewGuid(), Name = "Cashier" };
        var user = new User
        {
            Id = userId,
            TenantId = Guid.NewGuid(),
            FirstName = "Luis",
            LastName = "Hernández",
            Email = "luis@test.com",
            Phone = "555-1234",
            Mobile = "555-5678",
            Position = "Cajero",
            EmployeeCode = "EMP001",
            HireDate = new DateOnly(2023, 1, 15),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        user.UserRoles.Add(new UserRole { UserId = userId, RoleId = role.Id, Role = role });

        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(userId);
        dto.FirstName.Should().Be("Luis");
        dto.LastName.Should().Be("Hernández");
        dto.FullName.Should().Be("Luis Hernández");
        dto.Email.Should().Be("luis@test.com");
        dto.Phone.Should().Be("555-1234");
        dto.Mobile.Should().Be("555-5678");
        dto.Position.Should().Be("Cajero");
        dto.EmployeeCode.Should().Be("EMP001");
        dto.HireDate.Should().Be(new DateOnly(2023, 1, 15));
        dto.IsActive.Should().BeTrue();
        dto.Roles.Should().ContainSingle().Which.Should().Be("Cashier");
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("User.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
