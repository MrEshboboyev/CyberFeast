namespace BuildingBlocks.Caching;

/// <summary>
/// Defines configuration options for Redis.
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Gets or sets the Redis host.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets the Redis port.
    /// </summary>
    public int Port { get; set; }
}