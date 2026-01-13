using Corelio.Application.Tenants.Commands.RegisterTenant;
using Corelio.SharedKernel.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Tenant management endpoints for registration and configuration.
/// </summary>
public static class TenantEndpoints
{
    /// <summary>
    /// Maps all tenant endpoints to the application.
    /// </summary>
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tenants")
            .WithTags("Tenants");

        group.MapPost("/register", RegisterTenant)
            .WithName("RegisterTenant")
            .WithSummary("Register a new tenant with an owner account")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> RegisterTenant(
        [FromBody] RegisterTenantCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/v1/tenants/{result.Value}", new { tenantId = result.Value })
            : Results.Problem(
                statusCode: result.Error!.Type switch
                {
                    Application.Common.Models.ErrorType.NotFound => StatusCodes.Status404NotFound,
                    Application.Common.Models.ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    Application.Common.Models.ErrorType.Validation => StatusCodes.Status400BadRequest,
                    Application.Common.Models.ErrorType.Conflict => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                },
                title: result.Error.Code,
                detail: result.Error.Message);
    }
}
