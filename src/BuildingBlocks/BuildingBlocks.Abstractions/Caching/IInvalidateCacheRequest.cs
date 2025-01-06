using MediatR;

namespace BuildingBlocks.Abstractions.Caching;

/// <summary>
/// Defines a request to invalidate a cache entry.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IInvalidateCacheRequest<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Gets the prefix for the cache key.
    /// </summary>
    string Prefix { get; }

    /// <summary>
    /// Generates the cache keys for the request.
    /// </summary>
    /// <param name="request">The request for which to generate the cache keys.</param>
    /// <returns>A collection of generated cache keys.</returns>
    IEnumerable<string> CacheKeys(TRequest request);
}

/// <summary>
/// Defines a request to invalidate a cache entry without a response type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IInvalidateCacheRequest<in TRequest>
    where TRequest : IRequest
{
}

/// <summary>
/// Defines a request to invalidate a cache entry for a stream request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IStreamInvalidateCacheRequest<in TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    /// <summary>
    /// Gets the prefix for the cache key.
    /// </summary>
    string Prefix { get; }

    /// <summary>
    /// Generates the cache keys for the request.
    /// </summary>
    /// <param name="request">The request for which to generate the cache keys.</param>
    /// <returns>A collection of generated cache keys.</returns>
    IEnumerable<string> CacheKeys(TRequest request);
}