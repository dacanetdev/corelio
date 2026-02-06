using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Interfaces.Authentication;
using Corelio.Application.Common.Interfaces.Email;
using Corelio.Domain.Common.Interfaces;
using Corelio.Domain.Repositories;
using Corelio.Infrastructure.Authentication;
using Corelio.Infrastructure.Email;
using Corelio.Infrastructure.MultiTenancy;
using Corelio.Infrastructure.Persistence;
using Corelio.Infrastructure.Persistence.Interceptors;
using Corelio.Infrastructure.Persistence.Repositories;
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

        // Register tenant theme service
        services.AddScoped<ITenantThemeService, TenantThemeService>();

        // Register current user provider
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        // Register authentication services
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Register email service (stub for MVP)
        services.AddScoped<IEmailService, StubEmailService>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITenantConfigurationRepository, TenantConfigurationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<ITenantPricingConfigurationRepository, TenantPricingConfigurationRepository>();
        services.AddScoped<IProductPricingRepository, ProductPricingRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

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

        // Register tenant theme service
        builder.Services.AddScoped<ITenantThemeService, TenantThemeService>();

        // Register current user provider
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        // Register authentication services
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
        builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        builder.Services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Register email service (stub for MVP)
        builder.Services.AddScoped<IEmailService, StubEmailService>();

        // Register repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITenantRepository, TenantRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<ITenantConfigurationRepository, TenantConfigurationRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        builder.Services.AddScoped<ITenantPricingConfigurationRepository, TenantPricingConfigurationRepository>();
        builder.Services.AddScoped<IProductPricingRepository, ProductPricingRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register interceptors (injected into ApplicationDbContext via constructor)
        builder.Services.AddScoped<TenantInterceptor>();
        builder.Services.AddScoped<AuditInterceptor>();

        // Add Redis distributed cache with Aspire integration (for tenant caching)
        builder.AddRedisClient("redis");
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            // Aspire will auto-configure the connection via service discovery
        });

        // Add DbContext without pooling
        // Cannot use AddNpgsqlDbContext because it uses pooling which conflicts with scoped ITenantProvider
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            // Get connection string from Aspire-enriched configuration
            var connectionString = builder.Configuration.GetConnectionString("corelioDb");

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

        return builder;
    }
}
