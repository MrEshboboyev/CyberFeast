using BuildingBlocks.Resiliency.Retry;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace BuildingBlocks.Resiliency.CircuitBreaker;

/// <summary>
/// Provides circuit breaker policies for handling HTTP requests.
/// </summary>
public static class HttpCircuitBreakerPolicies
{
    /// <summary>
    /// Gets the HTTP circuit breaker policy based on the provided options and logger.
    /// </summary>
    /// <param name="logger">The logger to use for logging circuit breaker events.</param>
    /// <param name="circuitBreakerPolicyConfig">The circuit breaker policy configuration options.</param>
    /// <returns>The HTTP circuit breaker policy.</returns>
    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy(
        ILogger logger,
        ICircuitBreakerPolicyOptions circuitBreakerPolicyConfig
    )
    {
        return HttpPolicyBuilders
            .GetBaseBuilder()
            .CircuitBreakerAsync(
                circuitBreakerPolicyConfig.RetryCount + 1,
                TimeSpan.FromSeconds(circuitBreakerPolicyConfig.BreakDuration),
                (result, breakDuration) =>
                {
                    OnHttpBreak(result, breakDuration, circuitBreakerPolicyConfig.RetryCount, logger);
                },
                () => { OnHttpReset(logger); }
            );
    }

    /// <summary>
    /// Logs an event when the circuit breaker is triggered and the circuit is opened.
    /// </summary>
    /// <param name="result">The result of the HTTP request.</param>
    /// <param name="breakDuration">The duration of the circuit break.</param>
    /// <param name="retryCount">The number of retries before the circuit broke.</param>
    /// <param name="logger">The logger to use for logging.</param>
    private static void OnHttpBreak(
        DelegateResult<HttpResponseMessage> result,
        TimeSpan breakDuration,
        int retryCount,
        ILogger logger
    )
    {
        logger.LogWarning(
            "Service shutdown during {BreakDuration} after {DefaultRetryCount} failed retries",
            breakDuration,
            retryCount
        );
        throw new BrokenCircuitException("Service inoperative. Please try again later");
    }

    /// <summary>
    /// Logs an event when the circuit breaker is reset and the circuit is closed.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    public static void OnHttpReset(ILogger logger)
    {
        logger.LogInformation("Service restarted");
    }
}