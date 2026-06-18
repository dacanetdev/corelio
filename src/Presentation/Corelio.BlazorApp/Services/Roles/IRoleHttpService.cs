using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Roles;

namespace Corelio.BlazorApp.Services.Roles;

public interface IRoleHttpService
{
    Task<Result<List<RoleModel>>> GetRolesAsync(CancellationToken cancellationToken = default);
}
