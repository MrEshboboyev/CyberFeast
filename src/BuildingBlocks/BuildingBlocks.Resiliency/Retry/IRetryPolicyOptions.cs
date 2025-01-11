namespace BuildingBlocks.Resiliency.Retry;

/// <summary>
/// Defines the configuration options for retry policies.
/// </summary>
public interface IRetryPolicyOptions
{
    /// <summary>
    /// Gets or sets the number of retry attempts allowed by the policy.
    /// </summary>
    /// <value>
    /// The number of times to retry a failed operation before letting it fail permanently.
    /// </value>
    int RetryCount { get; set; }
}