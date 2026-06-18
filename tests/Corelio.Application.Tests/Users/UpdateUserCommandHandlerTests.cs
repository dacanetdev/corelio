using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Users.Commands.UpdateUser;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.Users;

[Trait("Category", "Unit")]
public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateUserCommandHandler(_userRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);

        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(CreateCommand(userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepoMock.Verify(x => x.Update(user), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(CreateCommand(userId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("User.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _userRepoMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIsActiveFalse_SetsUserInactive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId, isActive: true);

        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = CreateCommand(userId) with { IsActive = false };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_UpdatesAllFieldsOnEntity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);

        _userRepoMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new UpdateUserCommand(
            userId,
            "Nuevo", "Apellido",
            "800-123-4567", "800-765-4321",
            "Gerente General", "EMP999",
            new DateOnly(2022, 6, 1),
            false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.FirstName.Should().Be("Nuevo");
        user.LastName.Should().Be("Apellido");
        user.Phone.Should().Be("800-123-4567");
        user.Mobile.Should().Be("800-765-4321");
        user.Position.Should().Be("Gerente General");
        user.EmployeeCode.Should().Be("EMP999");
        user.HireDate.Should().Be(new DateOnly(2022, 6, 1));
        user.IsActive.Should().BeFalse();
    }

    private static User CreateUser(Guid id, bool isActive = true) =>
        new()
        {
            Id = id,
            TenantId = Guid.NewGuid(),
            FirstName = "Original",
            LastName = "Name",
            Email = "original@test.com",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };

    private static UpdateUserCommand CreateCommand(Guid userId) =>
        new(userId, "Updated", "Name", null, null, null, null, null, true);
}
