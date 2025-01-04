namespace BuildingBlocks.Abstractions.Persistence.EFCore;

/// <summary>
/// Defines methods for executing retry logic on database operations that encounter exceptions.
/// </summary>
public interface IRetryDbContextExecution
{
    /// <summary>
    /// Asynchronously retries the specified action in case of an exception.
    /// </summary>
    /// <param name="action">The asynchronous action to retry.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RetryOnExceptionAsync(Func<Exception, Task> action);

    /// <summary>
    /// Asynchronously retries the specified action that returns a result in case of an exception.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the action.</typeparam>
    /// <param name="action">The asynchronous action to retry.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of type TResult.</returns>
    Task<TResult> RetryOnExceptionAsync<TResult>(Func<Exception, Task<TResult>> action);
}