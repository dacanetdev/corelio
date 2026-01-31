namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Extension methods for mapping all API endpoints.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps all API endpoints to the application.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for method chaining.</returns>
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapTenantEndpoints();
        app.MapTenantThemeEndpoints();

        return app;
    }
}
