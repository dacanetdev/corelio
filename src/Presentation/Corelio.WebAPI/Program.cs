using Corelio.Application;
using Corelio.Infrastructure;
using Corelio.Infrastructure.MultiTenancy;
using Corelio.ServiceDefaults;
using Corelio.WebAPI.Endpoints;
using Corelio.WebAPI.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (telemetry, health checks, service discovery)
builder.AddServiceDefaults();

// Add Application services (MediatR, FluentValidation, handlers)
builder.Services.AddApplicationServices();

// Add Infrastructure services (DbContext, interceptors, providers, repositories)
builder.AddInfrastructureServices();

// Add JWT authentication and authorization policies
builder.Services.AddCorelioAuthorization(builder.Configuration);

// Add OpenAPI and Scalar for API documentation
builder.Services.AddOpenApi();

var app = builder.Build();

// Map Aspire health check endpoints FIRST (before authentication middleware)
// This allows health checks to work without authentication/tenant context
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Corelio API";
        options.Theme = ScalarTheme.Purple;
        options.ShowSidebar = true;
    });
}

// Add authentication middleware (validates JWT tokens)
// Skip authentication for health check endpoints
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/health") &&
               !context.Request.Path.StartsWithSegments("/alive"),
    appBuilder =>
    {
        appBuilder.UseAuthentication();
        appBuilder.UseTenantResolution();
        appBuilder.UseAuthorization();
    });

// Map all API endpoints (authentication, tenants, etc.)
app.MapAllEndpoints();

app.Run();
