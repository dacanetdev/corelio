using Microsoft.Extensions.DependencyInjection;

namespace Corelio.SharedKernel.Messaging;

/// <summary>
/// Default implementation of the Mediator pattern with pipeline behavior support.
/// </summary>
public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Build the handler type: IRequestHandler<TRequest, TResponse>
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        // Resolve the handler from DI container
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"No handler registered for request type {requestType.Name}. " +
                $"Ensure the handler implements IRequestHandler<{requestType.Name}, {responseType.Name}> " +
                $"and is registered in the DI container.");

        // Get the Handle method
        var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))
            ?? throw new InvalidOperationException($"Handle method not found on handler type {handlerType.Name}.");

        // Build the pipeline with behaviors
        var behaviorType = typeof(IRequestPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = serviceProvider.GetServices(behaviorType).Reverse().ToArray();

        // Create the handler delegate
        Func<Task<TResponse>> handlerDelegate = async () =>
        {
            var result = handleMethod.Invoke(handler, [request, cancellationToken]);
            if (result is Task<TResponse> task)
            {
                return await task;
            }
            throw new InvalidOperationException(
                $"Handler for {requestType.Name} did not return Task<{responseType.Name}>.");
        };

        // Build the pipeline by wrapping each behavior
        foreach (var behavior in behaviors)
        {
            var currentDelegate = handlerDelegate;
            var behaviorHandleMethod = behaviorType.GetMethod(nameof(IRequestPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))
                ?? throw new InvalidOperationException($"Handle method not found on behavior type {behaviorType.Name}.");

            handlerDelegate = async () =>
            {
                var result = behaviorHandleMethod.Invoke(behavior, [request, currentDelegate, cancellationToken]);
                if (result is Task<TResponse> task)
                {
                    return await task;
                }
                throw new InvalidOperationException(
                    $"Behavior did not return Task<{responseType.Name}>.");
            };
        }

        // Execute the pipeline
        return await handlerDelegate();
    }
}
