using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Resiliency.Extensions;

/// <summary>
/// Provides extension methods for configuring HttpClient with custom policies.
/// </summary>
public static partial class HttpClientBuilderExtensions
{
    /// <summary>
    /// Adds custom policy handlers, including both retry and circuit breaker policies, to the HttpClient builder.
    /// </summary>
    /// <param name="httpClientBuilder">The HttpClient builder to add the policy handlers to.</param>
    /// <param name="builder">An optional custom builder function for additional configuration.</param>
    /// <returns>The configured HttpClient builder.</returns>
    public static IHttpClientBuilder AddCustomPolicyHandlers(
        this IHttpClientBuilder httpClientBuilder,
        Func<IHttpClientBuilder, IHttpClientBuilder>? builder = null
    )
    {
        var result = httpClientBuilder.AddRetryPolicyHandler().AddCircuitBreakerHandler();

        if (builder is not null)
            result = builder.Invoke(result);

        return result;
    }
}