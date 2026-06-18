using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<bool>.Failure(
                new Error("User.NotFound", $"User with ID '{request.UserId}' not found.", ErrorType.NotFound));
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.Mobile = request.Mobile;
        user.Position = request.Position;
        user.EmployeeCode = request.EmployeeCode;
        user.HireDate = request.HireDate;
        user.IsActive = request.IsActive;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
