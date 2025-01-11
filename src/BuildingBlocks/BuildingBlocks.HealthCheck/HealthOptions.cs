namespace BuildingBlocks.HealthCheck;

/// <summary>
/// Defines configuration options for enabling or disabling health checks.
/// </summary>
public class HealthOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether health checks are enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
}