using MediatR;

namespace BuildingBlocks.Abstractions.Caching;

/// <summary>
/// Defines the structure for a cacheable request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICacheRequest<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Gets the absolute expiration relative to now.
    /// </summary>
    TimeSpan AbsoluteExpirationRelativeToNow { get; }

    /// <summary>
    /// Gets the prefix for the cache key.
    /// </summary>
    string Prefix { get; }

    /// <summary>
    /// Generates the cache key for the request.
    /// </summary>
    /// <param name="request">The request for which to generate the cache key.</param>
    /// <returns>The generated cache key.</returns>
    string CacheKey(TRequest request);
}

/// <summary>
/// Defines the structure for a cacheable stream request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IStreamCacheRequest<in TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    /// <summary>
    /// Gets the absolute expiration relative to now.
    /// </summary>
    TimeSpan AbsoluteExpirationRelativeToNow { get; }

    /// <summary>
    /// Gets the prefix for the cache key.
    /// </summary>
    string Prefix { get; }

    /// <summary>
    /// Generates the cache key for the request.
    /// </summary>
    /// <param name="request">The request for which to generate the cache key.</param>
    /// <returns>The generated cache key.</returns>
    string CacheKey(TRequest request);
}