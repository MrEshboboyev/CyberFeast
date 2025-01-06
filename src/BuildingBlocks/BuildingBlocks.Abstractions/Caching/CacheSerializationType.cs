namespace BuildingBlocks.Abstractions.Caching;

/// <summary>
/// Defines the possible types of cache serialization.
/// </summary>
public enum CacheSerializationType
{
    /// <summary>
    /// Represents JSON serialization.
    /// </summary>
    Json = 0,

    /// <summary>
    /// Represents MessagePack serialization.
    /// </summary>
    MessagePack = 1,
}