using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Users;

namespace Corelio.BlazorApp.Services.Users;

public interface IUserHttpService
{
    Task<Result<PagedResult<UserListModel>>> GetUsersAsync(
        int pageNumber = 1, int pageSize = 20, string? search = null, bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<Result<UserModel>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<bool>> UpdateUserAsync(Guid id, UserFormModel model, CancellationToken cancellationToken = default);

    Task<Result<bool>> AssignRolesAsync(Guid id, string[] roleCodes, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<bool>> ActivateUserAsync(Guid id, CancellationToken cancellationToken = default);
}
