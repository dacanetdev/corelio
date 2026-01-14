using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Corelio.WebAPI.Extensions;

/// <summary>
/// Extension methods for configuring authentication and authorization.
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Adds JWT authentication and authorization policies to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddCorelioAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure JWT authentication
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Set to true in production
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.Zero // No tolerance for expiry
            };

            // Configure events for debugging
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    // Support token from query string for SignalR (future)
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        // Configure authorization policies
        services.AddAuthorizationBuilder()
            // User management policies
            .AddPolicy("users.view", policy => policy.RequireClaim("permissions", "users.view"))
            .AddPolicy("users.create", policy => policy.RequireClaim("permissions", "users.create"))
            .AddPolicy("users.update", policy => policy.RequireClaim("permissions", "users.update"))
            .AddPolicy("users.delete", policy => policy.RequireClaim("permissions", "users.delete"))

            // Product management policies
            .AddPolicy("products.view", policy => policy.RequireClaim("permissions", "products.view"))
            .AddPolicy("products.create", policy => policy.RequireClaim("permissions", "products.create"))
            .AddPolicy("products.update", policy => policy.RequireClaim("permissions", "products.update"))
            .AddPolicy("products.delete", policy => policy.RequireClaim("permissions", "products.delete"))

            // Sales policies
            .AddPolicy("sales.view", policy => policy.RequireClaim("permissions", "sales.view"))
            .AddPolicy("sales.create", policy => policy.RequireClaim("permissions", "sales.create"))
            .AddPolicy("sales.update", policy => policy.RequireClaim("permissions", "sales.update"))
            .AddPolicy("sales.delete", policy => policy.RequireClaim("permissions", "sales.delete"))

            // Customer policies
            .AddPolicy("customers.view", policy => policy.RequireClaim("permissions", "customers.view"))
            .AddPolicy("customers.create", policy => policy.RequireClaim("permissions", "customers.create"))
            .AddPolicy("customers.update", policy => policy.RequireClaim("permissions", "customers.update"))
            .AddPolicy("customers.delete", policy => policy.RequireClaim("permissions", "customers.delete"))

            // Inventory policies
            .AddPolicy("inventory.view", policy => policy.RequireClaim("permissions", "inventory.view"))
            .AddPolicy("inventory.adjust", policy => policy.RequireClaim("permissions", "inventory.adjust"))

            // Reports policies
            .AddPolicy("reports.view", policy => policy.RequireClaim("permissions", "reports.view"))
            .AddPolicy("reports.export", policy => policy.RequireClaim("permissions", "reports.export"))

            // Settings policies
            .AddPolicy("settings.view", policy => policy.RequireClaim("permissions", "settings.view"))
            .AddPolicy("settings.update", policy => policy.RequireClaim("permissions", "settings.update"))

            // Role management policies
            .AddPolicy("roles.view", policy => policy.RequireClaim("permissions", "roles.view"))
            .AddPolicy("roles.create", policy => policy.RequireClaim("permissions", "roles.create"))
            .AddPolicy("roles.update", policy => policy.RequireClaim("permissions", "roles.update"))
            .AddPolicy("roles.delete", policy => policy.RequireClaim("permissions", "roles.delete"));

        return services;
    }
}
