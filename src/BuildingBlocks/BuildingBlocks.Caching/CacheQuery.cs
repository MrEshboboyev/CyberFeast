using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Abstractions.Queries;

namespace BuildingBlocks.Caching;

/// <summary>
/// Represents a cacheable query with a request and response.
/// </summary>
public abstract record CacheQuery<TRequest, TResponse> : ICacheQuery<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
    where TResponse : class
{
    /// <summary>
    /// Gets the absolute expiration relative to now. Defaults to 5 minutes.
    /// </summary>
    public virtual TimeSpan AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets the cache key prefix. Defaults to "Ch_".
    /// </summary>
    public virtual string Prefix => "Ch_";

    /// <summary>
    /// Generates a cache key for the specified request.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <returns>A cache key string.</returns>
    public virtual string CacheKey(TRequest request) => $"{Prefix}{typeof(TRequest).Name}";
}