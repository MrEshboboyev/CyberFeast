using BuildingBlocks.Abstractions.Caching;
using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Caching.Behaviors;

/// <summary>
/// A pipeline behavior for handling caching of requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class CachingBehavior<TRequest, TResponse>(
    ILogger<CachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider = cachingProviderFactory
        .GetCachingProvider(cacheOptions.Value.DefaultCacheType);

    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    /// <summary>
    /// Handles the caching behavior for the request and response.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not ICacheRequest<TRequest, TResponse> cacheRequest)
        {
            // No cache policy found, so just continue through the pipeline
            return await next();
        }

        var cacheKey = cacheRequest.CacheKey(request);
        var cachedResponse = await _cacheProvider
            .GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cachedResponse.Value != null)
        {
            logger.LogDebug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );
            return cachedResponse.Value;
        }

        var response = await next();

        var expiredTimeSpan = cacheRequest.AbsoluteExpirationRelativeToNow != TimeSpan.FromMinutes(5)
            ? cacheRequest.AbsoluteExpirationRelativeToNow
            : TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute) != TimeSpan.FromMinutes(5)
                ? TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute)
                : cacheRequest.AbsoluteExpirationRelativeToNow;

        await _cacheProvider.SetAsync(cacheKey, response, expiredTimeSpan, cancellationToken);

        logger.LogDebug(
            "Caching response for {TRequest} with cache key: {CacheKey}",
            typeof(TRequest).FullName,
            cacheKey
        );

        return response;
    }
}

/// <summary>
/// A pipeline behavior for handling caching of streaming requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class StreamCachingBehavior<TRequest, TResponse>(
    ILogger<StreamCachingBehavior<TRequest, TResponse>> logger,
    IEasyCachingProviderFactory cachingProviderFactory,
    IOptions<CacheOptions> cacheOptions)
    : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
    where TResponse : class
{
    private readonly IEasyCachingProvider _cacheProvider =
        cachingProviderFactory.GetCachingProvider(cacheOptions.Value.DefaultCacheType);

    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    /// <summary>
    /// Handles the caching behavior for streaming requests and responses.
    /// </summary>
    public async IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not IStreamCacheRequest<TRequest, TResponse> cacheRequest)
        {
            // If the request does not implement IStreamCacheRequest, go to the next pipeline
            await foreach (var response in next().WithCancellation(cancellationToken))
            {
                yield return response;
            }

            yield break;
        }

        var cacheKey = cacheRequest.CacheKey(request);
        var cachedResponse = _cacheProvider.Get<TResponse>(cacheKey);

        if (cachedResponse != null)
        {
            logger.LogDebug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );

            yield return cachedResponse.Value;
            yield break;
        }

        var expiredTimeSpan = cacheRequest.AbsoluteExpirationRelativeToNow != TimeSpan.FromMinutes(5)
            ? cacheRequest.AbsoluteExpirationRelativeToNow
            : TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute) != TimeSpan.FromMinutes(5)
                ? TimeSpan.FromMinutes(_cacheOptions.ExpirationTimeInMinute)
                : cacheRequest.AbsoluteExpirationRelativeToNow;

        await foreach (var response in next().WithCancellation(cancellationToken))
        {
            _cacheProvider.SetAsync(cacheKey, response, expiredTimeSpan, cancellationToken)
                .GetAwaiter().GetResult();

            logger.LogDebug(
                "Caching response for {TRequest} with cache key: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );

            yield return response;
        }
    }
}