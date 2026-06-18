using Corelio.Application.Common.Models;
using Corelio.Application.Users.Queries.GetUsers;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Users;

[Trait("Category", "Unit")]
public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WithUsers_ReturnsPagedList()
    {
        // Arrange
        var users = new List<User>
        {
            CreateUser("Ana", "García"),
            CreateUser("Carlos", "López")
        };

        _userRepoMock.Setup(x => x.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 2));

        // Act
        var result = await _handler.Handle(new GetUsersQuery(1, 20, null, null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithNoUsers_ReturnsEmptyPagedResult()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<User>(), 0));

        // Act
        var result = await _handler.Handle(new GetUsersQuery(1, 20, null, null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_PassesSearchAndFilterToRepository()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetPagedAsync(2, 10, "ana", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<User>(), 0));

        // Act
        await _handler.Handle(new GetUsersQuery(2, 10, "ana", true), CancellationToken.None);

        // Assert
        _userRepoMock.Verify(x => x.GetPagedAsync(2, 10, "ana", true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MapsUserToListDto_IncludingRoles()
    {
        // Arrange
        var role = new Role { Id = Guid.NewGuid(), Name = "Administrator" };
        var user = CreateUser("María", "Ramos");
        user.Position = "Gerente";
        user.IsActive = true;
        user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id, Role = role });

        _userRepoMock.Setup(x => x.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(([user], 1));

        // Act
        var result = await _handler.Handle(new GetUsersQuery(1, 20, null, null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!.Items[0];
        dto.FullName.Should().Be("María Ramos");
        dto.Position.Should().Be("Gerente");
        dto.IsActive.Should().BeTrue();
        dto.Roles.Should().ContainSingle().Which.Should().Be("Administrator");
    }

    private static User CreateUser(string firstName, string lastName) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = $"{firstName.ToLower()}@test.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
}
