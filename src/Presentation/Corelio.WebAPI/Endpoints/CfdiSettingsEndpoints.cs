using Corelio.Application.CFDI.Commands.UpdateCfdiSettings;
using Corelio.Application.CFDI.Commands.UploadCfdiCertificate;
using Corelio.Application.CFDI.Queries.GetCfdiSettings;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// CFDI issuer settings and certificate management endpoints.
/// </summary>
public static class CfdiSettingsEndpoints
{
    public static IEndpointRouteBuilder MapCfdiSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tenants/cfdi")
            .WithTags("CFDI Settings")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/settings", GetSettings)
            .WithName("GetCfdiSettings")
            .WithSummary("Get current tenant CFDI issuer settings")
            .RequireAuthorization("settings.cfdi");

        group.MapPut("/settings", UpdateSettings)
            .WithName("UpdateCfdiSettings")
            .WithSummary("Update tenant CFDI issuer settings (RFC, name, tax regime, etc.)")
            .RequireAuthorization("settings.cfdi");

        group.MapPost("/certificate", UploadCertificate)
            .WithName("UploadCfdiCertificate")
            .WithSummary("Upload a CSD (.pfx) certificate for CFDI signing")
            .RequireAuthorization("settings.cfdi")
            .DisableAntiforgery();

        return app;
    }

    private static async Task<IResult> GetSettings(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCfdiSettingsQuery();
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateSettings(
        UpdateCfdiSettingsCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UploadCertificate(
        IFormFile file,
        [FromForm] string password,
        IMediator mediator,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("Certificate file is required.");
        }

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var certData = ms.ToArray();

        var command = new UploadCfdiCertificateCommand(certData, password);
        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }
}
