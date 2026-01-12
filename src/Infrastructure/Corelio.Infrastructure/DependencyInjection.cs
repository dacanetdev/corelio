using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Common.Interfaces;
using Corelio.Infrastructure.MultiTenancy;
using Corelio.Infrastructure.Persistence;
using Corelio.Infrastructure.Persistence.Interceptors;
using Corelio.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corelio.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services to the dependency injection container.
    /// Interceptors are automatically added via ApplicationDbContext.OnConfiguring().
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register HTTP context accessor (required for tenant and user providers)
        services.AddHttpContextAccessor();

        // Register tenant provider as scoped (one per request)
        services.AddScoped<TenantProvider>();
        services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantProvider>());

        // Register tenant service
        services.AddScoped<ITenantService, TenantService>();

        // Register current user provider
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        // Register distributed cache (for tenant caching)
        // Note: For non-Aspire deployments, configure Redis connection string in appsettings.json
        var redisConnection = configuration.GetConnectionString("redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
            });
        }
        else
        {
            // Fallback to memory cache if Redis not configured
            services.AddDistributedMemoryCache();
        }

        // Register interceptors (injected into ApplicationDbContext via constructor)
        services.AddScoped<TenantInterceptor>();
        services.AddScoped<AuditInterceptor>();

        // Register DbContext with PostgreSQL
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("corelioDb");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });

#if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
        });

        return services;
    }

    /// <summary>
    /// Adds Infrastructure services with Aspire PostgreSQL integration.
    /// Interceptors are automatically added via ApplicationDbContext.OnConfiguring().
    /// </summary>
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // Register HTTP context accessor (required for tenant and user providers)
        builder.Services.AddHttpContextAccessor();

        // Register tenant provider as scoped (one per request)
        builder.Services.AddScoped<TenantProvider>();
        builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantProvider>());

        // Register tenant service
        builder.Services.AddScoped<ITenantService, TenantService>();

        // Register current user provider
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        // Register interceptors (injected into ApplicationDbContext via constructor)
        builder.Services.AddScoped<TenantInterceptor>();
        builder.Services.AddScoped<AuditInterceptor>();

        // Add Redis distributed cache with Aspire integration (for tenant caching)
        builder.AddRedisClient("redis");
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            // Aspire will auto-configure the connection via service discovery
        });

        // Add DbContext with Aspire PostgreSQL integration
        builder.AddNpgsqlDbContext<ApplicationDbContext>("corelioDb", configureDbContextOptions: options =>
        {
#if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
        });

        return builder;
    }
}
