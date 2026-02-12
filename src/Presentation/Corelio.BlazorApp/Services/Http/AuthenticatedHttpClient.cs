using System.Net.Http.Headers;
using System.Net.Http.Json;
using Corelio.BlazorApp.Services.Authentication;

namespace Corelio.BlazorApp.Services.Http;

/// <summary>
/// Wrapper around HttpClient that automatically adds JWT authorization headers.
/// This approach works perfectly with Blazor Server's circuit scoping by using
/// constructor-injected TokenService (which is circuit-scoped).
/// </summary>
public class AuthenticatedHttpClient(HttpClient httpClient, ITokenService tokenService)
{
    /// <summary>
    /// Sends a GET request with automatic authorization header.
    /// </summary>
    public async Task<TResponse?> GetAsync<TResponse>(string requestUri, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        return await httpClient.GetFromJsonAsync<TResponse>(requestUri, cancellationToken);
    }

    /// <summary>
    /// Sends a GET request and returns the HttpResponseMessage (for manual handling).
    /// </summary>
    public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        return await httpClient.GetAsync(requestUri, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with automatic authorization header.
    /// </summary>
    public async Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(
        string requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        return await httpClient.PostAsJsonAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with automatic authorization header and deserializes response.
    /// </summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        var response = await httpClient.PostAsJsonAsync(requestUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request with automatic authorization header.
    /// </summary>
    public async Task<HttpResponseMessage> PutAsJsonAsync<TRequest>(
        string requestUri,
        TRequest content,
        CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        return await httpClient.PutAsJsonAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request with automatic authorization header.
    /// </summary>
    public async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        await AddAuthorizationHeaderAsync();
        return await httpClient.DeleteAsync(requestUri, cancellationToken);
    }

    /// <summary>
    /// Provides access to the underlying HttpClient for advanced scenarios.
    /// Note: Authorization header must be added manually if using this directly.
    /// </summary>
    public HttpClient Client => httpClient;

    /// <summary>
    /// Adds the JWT authorization header to the request.
    /// Uses the circuit-scoped TokenService injected via constructor.
    /// </summary>
    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await tokenService.GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            token = token.Replace("Bearer ", string.Empty).Trim();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            // Remove header if no token (user logged out)
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
