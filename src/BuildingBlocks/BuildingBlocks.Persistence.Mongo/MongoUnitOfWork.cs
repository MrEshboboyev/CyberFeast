using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.Mongo;

namespace BuildingBlocks.Persistence.Mongo;

/// <summary>
/// Implements a unit of work pattern for MongoDB, managing transactions and committing changes.
/// </summary>
/// <typeparam name="TContext">The type of the MongoDB context.</typeparam>
public class MongoUnitOfWork<TContext>(TContext context) : IMongoUnitOfWork<TContext>, ITransactionAble
    where TContext : MongoDbContext
{
    /// <summary>
    /// Gets the MongoDB context.
    /// </summary>
    public TContext Context { get; } = context;

    /// <summary>
    /// Commits the changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins a transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.RollbackTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.CommitTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes of the unit of work instance.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}