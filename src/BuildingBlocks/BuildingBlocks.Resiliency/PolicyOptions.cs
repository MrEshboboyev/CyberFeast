using BuildingBlocks.Resiliency.CircuitBreaker;
using BuildingBlocks.Resiliency.Retry;
using BuildingBlocks.Resiliency.Timeout;

namespace BuildingBlocks.Resiliency;

/// <summary>
/// Defines the policy options for circuit breaker, retry, and timeout policies.
/// </summary>
public class PolicyOptions : ICircuitBreakerPolicyOptions, IRetryPolicyOptions, ITimeoutPolicyOptions
{
    /// <summary>
    /// Gets or sets the number of retry attempts.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the duration of the circuit break in seconds.
    /// </summary>
    public int BreakDuration { get; set; } = 30;

    /// <summary>
    /// Gets or sets the timeout duration in seconds.
    /// </summary>
    public int TimeOutDuration { get; set; } = 15;
}