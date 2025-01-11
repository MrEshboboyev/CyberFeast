using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace BuildingBlocks.Resiliency.Retry;

/// <summary>
/// Provides a base policy builder for handling HTTP errors.
/// </summary>
public static class HttpPolicyBuilders
{
    /// <summary>
    /// Gets the base policy builder for handling transient HTTP errors and bad requests.
    /// </summary>
    /// <returns>The policy builder.</returns>
    public static PolicyBuilder<HttpResponseMessage> GetBaseBuilder()
    {
        // Handle transient HTTP errors and HTTP 400 Bad Request responses.
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest);
    }
}