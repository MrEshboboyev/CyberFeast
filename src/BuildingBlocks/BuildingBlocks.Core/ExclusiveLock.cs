using System.Collections.Concurrent;
using BuildingBlocks.Abstractions.Core;
using SqlStreamStore.Logging;

namespace BuildingBlocks.Core;

/// <summary>
/// Implements the IExclusiveLock interface to provide exclusive locking mechanisms using semaphores.
/// </summary>
public class ExclusiveLock : IExclusiveLock
{
    private readonly ConcurrentDictionary<object, SemaphoreSlim> _semaphoreDictionary = new();
    private readonly ConcurrentDictionary<object, object> _lockDictionary = new();
    private readonly ILog _logger = LogProvider.For<ExclusiveLock>();

    /// <summary>
    /// Acquires an exclusive lock for the specified object asynchronously.
    /// </summary>
    /// <param name="obj">The object to lock.</param>
    /// <param name="token">A token to cancel the operation.</param>
    /// <returns>A task representing the lock acquisition.</returns>
    public Task<object> AcquireAsync(object obj, CancellationToken token = default)
    {
        var theLock = _lockDictionary.GetOrAdd(obj, o => new object());
        var semaphore = _semaphoreDictionary.GetOrAdd(theLock,
            _ => new SemaphoreSlim(1, 1));

        return semaphore.WaitAsync(token).ContinueWith(t => theLock, token);
    }

    /// <summary>
    /// Releases the exclusive lock for the specified object.
    /// </summary>
    /// <param name="obj">The object to unlock.</param>
    /// <returns>A task representing the lock release.</returns>
    public Task ReleaseAsync(object obj)
    {
        var semaphore = _semaphoreDictionary.GetOrAdd(obj,
            _ => new SemaphoreSlim(1, 1));
        semaphore.Release();
        return Task.FromResult(0);
    }

    /// <summary>
    /// Executes a synchronous action with an exclusive lock on the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to lock.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="token">A token to cancel the operation.</param>
    public void Execute<T>(T obj, Action<T> action, CancellationToken token = default)
    {
        var theLock = _lockDictionary.GetOrAdd(obj, _ => new object());
        var semaphore = _semaphoreDictionary.GetOrAdd(theLock,
            _ => new SemaphoreSlim(1, 1));
        semaphore.Wait(token);
        try
        {
            action(obj);
        }
        catch (System.Exception e)
        {
            _logger.Error("Exception when performing exclusive execute", e);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Executes an asynchronous function with an exclusive lock on the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to lock.</param>
    /// <param name="func">The function to execute.</param>
    /// <param name="token">A token to cancel the operation.</param>
    public async Task ExecuteAsync<T>(
        T obj,
        Func<T, Task> func,
        CancellationToken token = default(CancellationToken))
    {
        var theLock = _lockDictionary.GetOrAdd(obj, _ => new object());
        var semaphore = _semaphoreDictionary.GetOrAdd(theLock,
            _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(token);
        try
        {
            await func(obj);
        }
        catch (System.Exception e)
        {
            _logger.ErrorException("Exception when performing exclusive execute async", e);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Disposes all semaphore locks.
    /// </summary>
    public void Dispose()
    {
        foreach (var slim in _semaphoreDictionary.Values)
        {
            slim.Dispose();
        }
    }
}