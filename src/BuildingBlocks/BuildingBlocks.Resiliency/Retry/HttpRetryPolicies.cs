using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BuildingBlocks.Resiliency.Retry;

/// <summary>
/// Provides HTTP retry policies using Polly.
/// </summary>
public static class HttpRetryPolicies
{
    /// <summary>
    /// Gets the HTTP retry policy based on the provided options and logger.
    /// </summary>
    /// <param name="logger">The logger to use for logging retry attempts.</param>
    /// <param name="retryPolicyConfig">The retry policy configuration options.</param>
    /// <returns>The HTTP retry policy.</returns>
    public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy(
        ILogger logger,
        IRetryPolicyOptions retryPolicyConfig
    )
    {
        // Use the base policy builder and configure the retry logic.
        return HttpPolicyBuilders
            .GetBaseBuilder()
            .WaitAndRetryAsync(
                retryPolicyConfig.RetryCount,
                ComputeDuration,
                (result, timeSpan, retryCount, context) =>
                {
                    OnHttpRetry(result, timeSpan, retryCount, context, logger);
                }
            );
    }

    #region Private Methods

    /// <summary>
    /// Logs retry attempts with the provided logger.
    /// </summary>
    /// <param name="result">The result of the HTTP request.</param>
    /// <param name="timeSpan">The time to wait before the next retry.</param>
    /// <param name="retryCount">The current retry count.</param>
    /// <param name="context">The Polly context.</param>
    /// <param name="logger">The logger to use for logging.</param>
    private static void OnHttpRetry(
        DelegateResult<HttpResponseMessage> result,
        TimeSpan timeSpan,
        int retryCount,
        Context context,
        ILogger logger
    )
    {
        // Check if the result is not null and log accordingly.
        if (result.Result != null)
        {
            logger.LogWarning(
                "Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}",
                result.Result.StatusCode,
                timeSpan,
                retryCount
            );
        }
        else
        {
            logger.LogWarning(
                "Request failed due to a network failure. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}",
                timeSpan,
                retryCount
            );
        }
    }

    /// <summary>
    /// Computes the duration to wait before the next retry.
    /// </summary>
    /// <param name="retryAttempt">The current retry attempt.</param>
    /// <returns>The duration to wait before the next retry.</returns>
    private static TimeSpan ComputeDuration(int retryAttempt)
    {
        // Use exponential backoff with jitter.
        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
               + TimeSpan.FromMilliseconds(new Random().Next(0, 100));
    }

    #endregion
}