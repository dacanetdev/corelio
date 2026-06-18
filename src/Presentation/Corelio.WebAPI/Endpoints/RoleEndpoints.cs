using Corelio.Application.Common.Models;
using Corelio.Application.Roles.Common;
using Corelio.Application.Roles.Queries.GetRoles;
using Corelio.SharedKernel.Messaging;

namespace Corelio.WebAPI.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/roles")
            .WithTags("Roles")
            .RequireAuthorization();

        group.MapGet("/", GetRoles)
            .WithName("GetRoles")
            .WithSummary("List all roles with their assigned permissions")
            .Produces<List<RoleDto>>(200)
            .RequireAuthorization("users.view");

        return app;
    }

    private static async Task<IResult> GetRoles(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetRolesQuery(), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }
}
