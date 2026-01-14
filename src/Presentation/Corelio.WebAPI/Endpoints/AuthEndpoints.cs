using Corelio.Application.Authentication.Commands.ForgotPassword;
using Corelio.Application.Authentication.Commands.Login;
using Corelio.Application.Authentication.Commands.RefreshToken;
using Corelio.Application.Authentication.Commands.RegisterUser;
using Corelio.Application.Authentication.Commands.ResetPassword;
using Corelio.Application.Authentication.Commands.RevokeToken;
using Corelio.SharedKernel.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Authentication endpoints for login, registration, token management, and password reset.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps all authentication endpoints to the application.
    /// </summary>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Authenticate a user and issue JWT tokens")
            .AllowAnonymous();

        group.MapPost("/register", RegisterUser)
            .WithName("RegisterUser")
            .WithSummary("Register a new user within the current tenant")
            .RequireAuthorization("users.create");

        group.MapPost("/refresh", RefreshToken)
            .WithName("RefreshToken")
            .WithSummary("Refresh an access token using a refresh token")
            .AllowAnonymous();

        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithSummary("Revoke a refresh token (logout)")
            .RequireAuthorization();

        group.MapPost("/forgot-password", ForgotPassword)
            .WithName("ForgotPassword")
            .WithSummary("Request a password reset token")
            .AllowAnonymous();

        group.MapPost("/reset-password", ResetPassword)
            .WithName("ResetPassword")
            .WithSummary("Reset password using a reset token")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> Login(
        [FromBody] LoginCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
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

    private static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/v1/users/{result.Value}", new { userId = result.Value })
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

    private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
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

    private static async Task<IResult> Logout(
        [FromBody] RevokeTokenCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
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

    private static async Task<IResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        // Always return 200 OK for security (don't reveal if email exists)
        return Results.Ok(new { message = "If the email exists, a password reset link has been sent." });
    }

    private static async Task<IResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(new { message = "Password has been reset successfully." })
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
