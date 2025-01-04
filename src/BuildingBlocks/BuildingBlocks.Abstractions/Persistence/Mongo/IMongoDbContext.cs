using MongoDB.Driver;

namespace BuildingBlocks.Abstractions.Persistence.Mongo;

/// <summary>
/// Defines a contract for a MongoDB context.
/// </summary>
public interface IMongoDbContext : IDisposable
{
    /// <summary>
    /// Gets a MongoDB collection of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="name">The optional name of the collection.</param>
    /// <returns>The MongoDB collection of the specified type.</returns>
    IMongoCollection<T> GetCollection<T>(string? name = null);

    /// <summary>
    /// Asynchronously saves changes made to the context.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the number of changes saved.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously begins a transaction.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a command to be executed within the context.
    /// </summary>
    /// <param name="func">The asynchronous command to execute.</param>
    void AddCommand(Func<Task> func);
}