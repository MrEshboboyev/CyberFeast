using MediatR;

namespace BuildingBlocks.Resiliency.Retry;

/// <summary>
/// Defines properties for retryable requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRetryableRequest<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Gets the number of retry attempts.
    /// </summary>
    int RetryAttempts => 1;

    /// <summary>
    /// Gets the retry delay in milliseconds.
    /// </summary>
    int RetryDelay => 250;

    /// <summary>
    /// Gets a value indicating whether to use exponential backoff for retries.
    /// </summary>
    bool RetryWithExponentialBackoff => false;

    /// <summary>
    /// Gets the number of exceptions allowed before a circuit breaker trip.
    /// </summary>
    int ExceptionsAllowedBeforeCircuitTrip => 1;
}