namespace BuildingBlocks.Resiliency.CircuitBreaker;

/// <summary>
/// Defines the configuration options for circuit breaker policies.
/// </summary>
public interface ICircuitBreakerPolicyOptions
{
    /// <summary>
    /// Gets or sets the number of retry attempts before the circuit breaker trips.
    /// </summary>
    /// <value>
    /// The number of times to retry a failed operation before opening the circuit.
    /// </value>
    int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the duration of the circuit break in seconds.
    /// </summary>
    /// <value>
    /// The duration to wait before trying the operation again after the circuit is opened.
    /// </value>
    int BreakDuration { get; set; }
}