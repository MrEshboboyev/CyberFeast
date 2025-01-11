using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace BuildingBlocks.Resiliency.Fallback;

/// <summary>
/// Provides fallback logic for MediatR requests using Polly.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class FallbackBehavior<TRequest, TResponse>(
    IEnumerable<IFallbackHandler<TRequest, TResponse>> fallbackHandlers,
    ILogger<FallbackBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request with fallback behavior.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response from the request handler or the fallback handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var fallbackHandler = fallbackHandlers.FirstOrDefault();
        if (fallbackHandler == null)
            // No fallback handler found, continue through pipeline
            return await next();

        // Define fallback policy.
        var fallbackPolicy = Policy<TResponse>
            .Handle<Exception>()
            .FallbackAsync(ct =>
            {
                logger.LogDebug(
                    "Initial handler failed. Falling back to `{FullName}@HandleFallback`",
                    fallbackHandler.GetType().FullName
                );
                return fallbackHandler.HandleFallbackAsync(request, cancellationToken);
            });

        // Execute the request with fallback policy.
        var response = await fallbackPolicy.ExecuteAsync(() => next());

        return response;
    }
}