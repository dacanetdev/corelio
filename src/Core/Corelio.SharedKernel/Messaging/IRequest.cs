namespace Corelio.SharedKernel.Messaging;

/// <summary>
/// Marker interface for a request that returns a response.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the request.</typeparam>
public interface IRequest<out TResponse>
{
}
