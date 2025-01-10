using BuildingBlocks.Core.Reflection.Extensions;

namespace BuildingBlocks.Caching;

/// <summary>
/// Provides methods to generate cache keys using strings or the type of the owner class.
/// </summary>
public static class CacheKey
{
    /// <summary>
    /// Generates a cache key by joining the provided keys with a hyphen.
    /// </summary>
    /// <param name="keys">The keys to join.</param>
    /// <returns>A cache key string.</returns>
    public static string With(params string[] keys)
    {
        return string.Join("-", keys);
    }

    /// <summary>
    /// Generates a cache key using the owner type and provided keys.
    /// </summary>
    /// <param name="ownerType">The type of the owner class.</param>
    /// <param name="keys">The keys to join.</param>
    /// <returns>A cache key string.</returns>
    public static string With(Type ownerType, params string[] keys)
    {
        return With($"{ownerType.GetCacheKey()}:{string.Join("-", keys)}");
    }
}