using BuildingBlocks.Abstractions.Caching;
using MediatR;

namespace BuildingBlocks.Caching;

/// <summary>
/// Represents a request to invalidate cache entries with a specific request and response.
/// </summary>
public abstract class InvalidateCacheRequest<TRequest, TResponse> : IInvalidateCacheRequest<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Gets the cache key prefix. Defaults to "Ch_".
    /// </summary>
    public virtual string Prefix => "Ch_";

    /// <summary>
    /// Gets the cache keys for invalidation.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <returns>An enumerable collection of cache keys.</returns>
    public abstract IEnumerable<string> CacheKeys(TRequest request);
}

/// <summary>
/// Represents a request to invalidate cache entries with a specific request.
/// </summary>
public abstract class InvalidateCacheRequest<TRequest> : IInvalidateCacheRequest<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Gets the cache key prefix. Defaults to "Ch_".
    /// </summary>
    public virtual string Prefix => "Ch_";

    /// <summary>
    /// Gets the cache keys for invalidation.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <returns>An enumerable collection of cache keys.</returns>
    public abstract IEnumerable<string> CacheKeys(TRequest request);
}