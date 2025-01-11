namespace BuildingBlocks.Resiliency.Timeout;

/// <summary>
/// Defines the configuration options for timeout policies.
/// </summary>
public interface ITimeoutPolicyOptions
{
    /// <summary>
    /// Gets or sets the timeout duration in milliseconds.
    /// </summary>
    /// <value>
    /// The maximum duration to allow an operation to run before timing out.
    /// </value>
    int TimeOutDuration { get; set; }
}