using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Roles.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Roles.Queries.GetRoles;

public class GetRolesQueryHandler(
    IRoleRepository roleRepository,
    ITenantService tenantService) : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        var roles = await roleRepository.GetAllWithPermissionsAsync(tenantId, cancellationToken);

        var dtos = roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Description,
            r.IsSystemRole,
            r.IsDefault,
            r.RolePermissions
                .Select(rp => new PermissionSummaryDto(
                    rp.Permission.Code,
                    rp.Permission.Name,
                    rp.Permission.Module.ToString(),
                    rp.Permission.Category,
                    rp.Permission.IsDangerous))
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Code)
                .ToArray()
        )).ToList();

        return Result<List<RoleDto>>.Success(dtos);
    }
}
