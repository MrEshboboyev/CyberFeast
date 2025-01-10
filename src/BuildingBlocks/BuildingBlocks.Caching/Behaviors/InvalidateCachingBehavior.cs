using BuildingBlocks.Abstractions.Caching;
using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Caching.Behaviors;

/// <summary>
/// A pipeline behavior for handling cache invalidation of requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class InvalidateCachingBehavior<TRequest, TResponse>(
    ILogger<InvalidateCachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider = cachingProviderFactory
        .GetCachingProvider(cacheOptions.Value.DefaultCacheType);

    /// <summary>
    /// Handles the cache invalidation behavior for the request and response.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not IInvalidateCacheRequest<TRequest, TResponse> cacheRequest)
        {
            // No cache policy found, so just continue through the pipeline
            return await next();
        }

        var cacheKeys = cacheRequest.CacheKeys(request);
        var response = await next();

        foreach (var cacheKey in cacheKeys)
        {
            await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
            logger.LogDebug("Cache data with cache key: {CacheKey} invalidated", cacheKey);
        }

        return response;
    }
}

/// <summary>
/// A pipeline behavior for handling cache invalidation of streaming requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class StreamInvalidateCachingBehavior<TRequest, TResponse>(
    ILogger<StreamInvalidateCachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions)
    : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider = cachingProviderFactory
        .GetCachingProvider(cacheOptions.Value.DefaultCacheType);

    /// <summary>
    /// Handles the cache invalidation behavior for streaming requests and responses.
    /// </summary>
    public async IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not IStreamInvalidateCacheRequest<TRequest, TResponse> cacheRequest)
        {
            // If the request does not implement IStreamCacheRequest, go to the next pipeline
            await foreach (var response in next().WithCancellation(cancellationToken))
            {
                yield return response;
            }

            yield break;
        }

        await foreach (var response in next().WithCancellation(cancellationToken))
        {
            var cacheKeys = cacheRequest.CacheKeys(request);

            foreach (var cacheKey in cacheKeys)
            {
                await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
                logger.LogDebug("Cache data with cache key: {CacheKey} invalidated", cacheKey);
            }

            yield return response;
        }
    }
}