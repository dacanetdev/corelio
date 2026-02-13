using System.Text.Json;
using System.Text.Json.Serialization;
using Corelio.Application;
using Corelio.Infrastructure;
using Corelio.Infrastructure.MultiTenancy;
using Corelio.ServiceDefaults;
using Corelio.WebAPI.Endpoints;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Corelio.WebAPI.Middleware;
using Microsoft.EntityFrameworkCore;
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

// Configure JSON serialization options for better error messages
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Allow string-to-enum conversion (e.g., "EAN13" -> BarcodeType.EAN13)
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

    // Make JSON property names case-insensitive for more forgiving API
    options.SerializerOptions.PropertyNameCaseInsensitive = true;

    // Write indented JSON in development for readability
    if (builder.Environment.IsDevelopment())
    {
        options.SerializerOptions.WriteIndented = true;
    }
});

// Add OpenAPI and Scalar for API documentation
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply pending migrations in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<Corelio.Infrastructure.Persistence.ApplicationDbContext>();

    try
    {
        await dbContext.Database.MigrateAsync();
        app.Logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

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

// Global exception handler (must be early in pipeline to catch all exceptions)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

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
