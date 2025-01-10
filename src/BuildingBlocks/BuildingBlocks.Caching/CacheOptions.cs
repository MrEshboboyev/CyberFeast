using BuildingBlocks.Abstractions.Caching;

namespace BuildingBlocks.Caching;

/// <summary>
/// Defines the configuration options for caching.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Gets or sets the default cache type. Defaults to in-memory cache.
    /// </summary>
    public string DefaultCacheType { get; set; } = nameof(CacheProviderType.InMemory);

    /// <summary>
    /// Gets or sets the expiration time in minutes. Defaults to 5 minutes.
    /// </summary>
    public double ExpirationTimeInMinute { get; set; } = 5;

    /// <summary>
    /// Gets or sets the serialization type. Defaults to JSON.
    /// </summary>
    public string SerializationType { get; set; } = nameof(CacheSerializationType.Json);

    /// <summary>
    /// Gets or sets the options for Redis cache. Defaults to null.
    /// </summary>
    public RedisCacheOptions? RedisCacheOptions { get; set; } = null;

    /// <summary>
    /// Gets or sets the options for in-memory cache. Defaults to null.
    /// </summary>
    public InMemoryCacheOptions? InMemoryOptions { get; set; } = null;

    /// <summary>
    /// Gets or sets the default cache prefix. Defaults to "Ch_".
    /// </summary>
    public string DefaultCachePrefix { get; set; } = "Ch_";
}

/// <summary>
/// Defines the options for Redis cache.
/// </summary>
public class RedisCacheOptions
{
    /// <summary>
    /// Gets or sets the connection string for Redis.
    /// </summary>
    public string? ConnectionString { get; set; } = null;
}

/// <summary>
/// Defines the options for in-memory cache.
/// </summary>
public class InMemoryCacheOptions
{
}