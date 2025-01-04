namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// The unit of work pattern.
/// Ensures that a series of operations are completed as a single transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Starts a transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// The unit of work pattern with a context.
/// </summary>
/// <typeparam name="TContext">The type of the context.</typeparam>
public interface IUnitOfWork<out TContext> : IUnitOfWork
    where TContext : class
{
    /// <summary>
    /// Gets the context of the specified type.
    /// </summary>
    TContext Context { get; }
}