using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Resiliency.CircuitBreaker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Resiliency.Extensions;

/// <summary>
/// Provides extension methods for configuring HttpClient with custom policies.
/// </summary>
public static partial class HttpClientBuilderExtensions
{
    /// <summary>
    /// Adds a circuit breaker policy handler to the HttpClient builder.
    /// </summary>
    /// <param name="httpClientBuilder">The HttpClient builder to add the policy handler to.</param>
    /// <returns>The configured HttpClient builder.</returns>
    public static IHttpClientBuilder AddCircuitBreakerHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler(
            (sp, _) =>
            {
                var options = sp.GetRequiredService<IConfiguration>()
                    .BindOptions<PolicyOptions>(nameof(PolicyOptions));

                options.NotBeNull();

                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var circuitBreakerLogger = loggerFactory
                    .CreateLogger("PollyHttpCircuitBreakerPoliciesLogger");

                return HttpCircuitBreakerPolicies.GetHttpCircuitBreakerPolicy(circuitBreakerLogger, options);
            }
        );
    }
}