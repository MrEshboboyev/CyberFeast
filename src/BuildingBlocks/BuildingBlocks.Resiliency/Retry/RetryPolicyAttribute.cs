namespace BuildingBlocks.Resiliency.Retry;

/// <summary>
/// Applies a retry policy to MediatR requests.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class RetryPolicyAttribute : Attribute
{
    private int _retryCount = 3;
    private int _sleepDuration = 200;

    /// <summary>
    /// Gets or sets the number of retry attempts.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the value is less than 1.</exception>
    public int RetryCount
    {
        get => _retryCount;
        set
        {
            if (value < 1)
                throw new ArgumentException("Retry count must be higher than 1.", nameof(value));

            _retryCount = value;
        }
    }

    /// <summary>
    /// Gets or sets the sleep duration between retries in milliseconds.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the value is less than 1.</exception>
    public int SleepDuration
    {
        get => _sleepDuration;
        set
        {
            if (value < 1)
                throw new ArgumentException("Sleep duration must be higher than 1ms.", nameof(value));

            _sleepDuration = value;
        }
    }
}