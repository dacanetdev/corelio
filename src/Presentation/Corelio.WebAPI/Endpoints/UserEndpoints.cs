using Corelio.Application.Common.Models;
using Corelio.Application.Users.Commands.AssignRoles;
using Corelio.Application.Users.Commands.UpdateUser;
using Corelio.Application.Users.Common;
using Corelio.Application.Users.Queries.GetUserById;
using Corelio.Application.Users.Queries.GetUsers;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Tenant user management endpoints.
/// </summary>
public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
            .WithTags("Users")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get paged list of users")
            .WithDescription("Returns a paginated list of users in the current tenant. Supports free-text search on name and email. Optionally filter by `isActive` status. Includes role names for each user.")
            .Produces<PagedResult<UserListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("users.view");

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Returns full user profile including contact info, employment details, and assigned roles. Returns 404 if not found.")
            .Produces<UserDto>(200)
            .Produces(404)
            .RequireAuthorization("users.view");

        group.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update user profile")
            .WithDescription("Updates the user's personal and employment information. Does not change email or password. Returns 204 No Content on success.")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(400)
            .RequireAuthorization("users.update");

        group.MapPut("/{id:guid}/roles", AssignRoles)
            .WithName("AssignUserRoles")
            .WithSummary("Assign roles to a user")
            .WithDescription("Replaces all existing role assignments with the provided role codes. Pass an empty array to remove all roles. Role codes must be valid system or tenant-specific role names.")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(400)
            .RequireAuthorization("users.update");

        group.MapPut("/{id:guid}/deactivate", DeactivateUser)
            .WithName("DeactivateUser")
            .WithSummary("Deactivate a user")
            .WithDescription("Sets the user's IsActive flag to false, preventing login. Returns 204 No Content on success.")
            .Produces(204)
            .Produces(404)
            .RequireAuthorization("users.update");

        group.MapPut("/{id:guid}/activate", ActivateUser)
            .WithName("ActivateUser")
            .WithSummary("Activate a user")
            .WithDescription("Sets the user's IsActive flag to true, restoring login access. Returns 204 No Content on success.")
            .Produces(204)
            .Produces(404)
            .RequireAuthorization("users.update");

        return app;
    }

    private static async Task<IResult> GetUsers(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetUsersQuery(
            Page: pageNumber > 0 ? pageNumber : 1,
            Size: pageSize > 0 ? pageSize : 20,
            Search: search,
            IsActive: isActive);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetUserById(Guid id, IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Mobile,
            request.Position,
            request.EmployeeCode,
            request.HireDate,
            request.IsActive);

        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> AssignRoles(
        Guid id,
        [FromBody] AssignRolesRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new AssignRolesCommand(id, request.RoleCodes);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> DeactivateUser(Guid id, IMediator mediator, CancellationToken ct)
    {
        var user = await mediator.Send(new GetUserByIdQuery(id), ct);
        if (!user.IsSuccess)
        {
            return user.ToHttpResult();
        }

        var dto = user.Value!;
        var command = new UpdateUserCommand(
            id, dto.FirstName, dto.LastName, dto.Phone, dto.Mobile,
            dto.Position, dto.EmployeeCode, dto.HireDate, IsActive: false);

        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> ActivateUser(Guid id, IMediator mediator, CancellationToken ct)
    {
        var user = await mediator.Send(new GetUserByIdQuery(id), ct);
        if (!user.IsSuccess)
        {
            return user.ToHttpResult();
        }

        var dto = user.Value!;
        var command = new UpdateUserCommand(
            id, dto.FirstName, dto.LastName, dto.Phone, dto.Mobile,
            dto.Position, dto.EmployeeCode, dto.HireDate, IsActive: true);

        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }
}

/// <summary>Request body for updating a user profile.</summary>
public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? Phone,
    string? Mobile,
    string? Position,
    string? EmployeeCode,
    DateOnly? HireDate,
    bool IsActive);

/// <summary>Request body for assigning roles to a user.</summary>
public record AssignRolesRequest(string[] RoleCodes);
