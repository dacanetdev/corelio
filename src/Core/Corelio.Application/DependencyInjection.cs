using Corelio.Application.Common.Behaviors;
using System.Reflection;
using Corelio.SharedKernel.Messaging;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Corelio.Application;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register custom Mediator
        services.AddScoped<IMediator, Mediator>();

        // Register all request handlers automatically
        RegisterHandlers(services, assembly);

        // Register all FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(assembly);

        // Register the validation pipeline behavior
        services.AddScoped(typeof(IRequestPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register Mapster for object mapping
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(assembly);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        // Find all types that implement IRequestHandler<,>
        var handlerInterfaceType = typeof(IRequestHandler<,>);

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                ImplementationType = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == handlerInterfaceType)
                    .ToList()
            })
            .Where(t => t.Interfaces.Count > 0)
            .ToList();

        // Register each handler
        foreach (var handlerType in handlerTypes)
        {
            foreach (var interfaceType in handlerType.Interfaces)
            {
                services.AddScoped(interfaceType, handlerType.ImplementationType);
            }
        }
    }
}

