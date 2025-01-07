using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IsolationLevel = System.Data.IsolationLevel;

namespace BuildingBlocks.Core.Persistence.EfCore;

/// <summary>
/// Represents a unit of work for Entity Framework Core, providing transactional support and domain event publishing.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public class EfUnitOfWork<TDbContext>(
    TDbContext context,
    IDomainEventsAccessor domainEventsAccessor,
    IDomainEventPublisher domainEventPublisher,
    ILogger<EfUnitOfWork<TDbContext>> logger) : IEFUnitOfWork<TDbContext>
    where TDbContext : EfDbContextBase
{
    private readonly ILogger<EfUnitOfWork<TDbContext>> _logger = logger;

    /// <summary>
    /// Gets the database context.
    /// </summary>
    public TDbContext DbContext => context;

    /// <summary>
    /// Gets the set of entities of a given type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>The set of entities.</returns>
    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class
    {
        return context.Set<TEntity>();
    }

    /// <summary>
    /// Begins a new transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default)
    {
        return context.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    /// <summary>
    /// Begins a new transaction with the default isolation level.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction and publishes domain events.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = domainEventsAccessor.UnCommittedDomainEvents;
        await domainEventPublisher.PublishAsync(domainEvents.ToArray(), cancellationToken);

        await context.CommitTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Saves changes to the database and publishes domain events.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = domainEventsAccessor.UnCommittedDomainEvents;
        await domainEventPublisher.PublishAsync(domainEvents.ToArray(), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.RollbackTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Executes the specified operation, retrying on exception.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RetryOnExceptionAsync(Func<Task> operation)
    {
        return context.RetryOnExceptionAsync(operation);
    }

    /// <summary>
    /// Executes the specified operation, retrying on exception.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
    {
        return context.RetryOnExceptionAsync(operation);
    }

    /// <summary>
    /// Executes the specified action within a transaction.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ExecuteTransactionalAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default)
    {
        return context.ExecuteTransactionalAsync(action, cancellationToken);
    }

    /// <summary>
    /// Executes the specified action within a transaction.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<T> ExecuteTransactionalAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        return context.ExecuteTransactionalAsync(action, cancellationToken);
    }

    /// <summary>
    /// Disposes the unit of work.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}