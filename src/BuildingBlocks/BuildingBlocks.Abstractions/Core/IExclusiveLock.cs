namespace BuildingBlocks.Abstractions.Core;

/// <summary>
/// Defines a contract for acquiring and releasing exclusive locks.
/// </summary>
public interface IExclusiveLock : IDisposable
{
    /// <summary>
    /// Asynchronously acquires an exclusive lock on the specified object.
    /// </summary>
    /// <param name="obj">The object to lock.</param>
    /// <param name="token">A cancellation token to observe while waiting for the lock to be acquired.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the locked object.</returns>
    Task<object> AcquireAsync(object obj, CancellationToken token = default);

    /// <summary>
    /// Asynchronously releases the lock on the specified object.
    /// </summary>
    /// <param name="obj">The object to unlock.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ReleaseAsync(object obj);

    /// <summary>
    /// Executes a specified action within the context of an exclusive lock on the given object.
    /// </summary>
    /// <typeparam name="T">The type of the object to lock.</typeparam>
    /// <param name="obj">The object to lock.</param>
    /// <param name="action">The action to perform within the lock context.</param>
    /// <param name="token">A cancellation token to observe while performing the action.</param>
    void Execute<T>(T obj, Action<T> action, CancellationToken token = default);

    /// <summary>
    /// Asynchronously executes a specified function within the context of an exclusive lock on the given object.
    /// </summary>
    /// <typeparam name="T">The type of the object to lock.</typeparam>
    /// <param name="obj">The object to lock.</param>
    /// <param name="func">The function to perform within the lock context.</param>
    /// <param name="token">A cancellation token to observe while performing the function.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteAsync<T>(T obj, Func<T, Task> func, CancellationToken token = default);
}