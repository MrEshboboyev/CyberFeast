namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines methods for executing database operations within a transactional context.
/// </summary>
public interface ITransactionDbContextExecution
{
    /// <summary>
    /// Executes the provided asynchronous action within a transaction.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ExecuteTransactionalAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the provided asynchronous action that returns a result of type T within a transaction.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the action.</typeparam>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of type T.</returns>
    public Task<T> ExecuteTransactionalAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);
}