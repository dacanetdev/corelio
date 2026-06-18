using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Phone,
    string? Mobile,
    string? Position,
    string? EmployeeCode,
    DateOnly? HireDate,
    bool IsActive) : IRequest<Result<bool>>;
