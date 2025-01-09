using System.Net.Http.Json;

namespace BuildingBlocks.Core.Web.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="HttpClient"/> class to simplify sending HTTP POST and PUT requests with JSON payloads and reading JSON responses.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a POST request with a JSON payload and reads the JSON response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request payload.</typeparam>
    /// <typeparam name="TResponse">The type of the response payload.</typeparam>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The URI of the request.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the response payload as the result.</returns>
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var responseMessage = await httpClient.PostAsJsonAsync(
            requestUri,
            request,
            cancellationToken: cancellationToken
        );

        var result = await responseMessage.Content
            .ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);

        return result;
    }

    /// <summary>
    /// Sends a PUT request with a JSON payload and reads the JSON response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request payload.</typeparam>
    /// <typeparam name="TResponse">The type of the response payload.</typeparam>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The URI of the request.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the response payload as the result.</returns>
    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var responseMessage = await httpClient.PutAsJsonAsync(
            requestUri,
            request,
            cancellationToken: cancellationToken
        );

        var result = await responseMessage.Content
            .ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);

        return result;
    }
}