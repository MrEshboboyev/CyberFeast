using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace BuildingBlocks.Resiliency.Retry;

/// <summary>
/// Provides retry logic for MediatR requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class RetryBehavior<TRequest, TResponse>(
    IEnumerable<IRetryableRequest<TRequest, TResponse>> retryHandlers,
    ILogger<RetryBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request with retry logic.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response from the request handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var retryHandler = retryHandlers.FirstOrDefault();

        // If no retry handler is found, continue through the pipeline.
        if (retryHandler == null)
            return await next();

        // Define circuit breaker policy.
        var circuitBreaker = Policy<TResponse>
            .Handle<Exception>()
            .CircuitBreakerAsync(
                retryHandler.ExceptionsAllowedBeforeCircuitTrip,
                TimeSpan.FromMilliseconds(5000),
                (_, _) => logger.LogDebug("Circuit Tripped!"),
                () => { }
            );

        // Define retry policy.
        var retryPolicy = Policy<TResponse>
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryHandler.RetryAttempts,
                retryAttempt =>
                {
                    var retryDelay = retryHandler.RetryWithExponentialBackoff
                        ? TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * retryHandler.RetryDelay)
                        : TimeSpan.FromMilliseconds(retryHandler.RetryDelay);

                    logger.LogDebug("Retrying, waiting {RetryDelay}...", retryDelay);

                    return retryDelay;
                }
            );

        // Execute the request with retry policy.
        var response = await retryPolicy.ExecuteAsync(() => next());

        return response;
    }
}