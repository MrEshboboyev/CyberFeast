using System.Collections.Immutable;
using System.Data;
using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.Core.Persistence.EfCore;

/// <summary>
/// Base class for Entity Framework Core database contexts, providing additional functionality for domain events and transactions.
/// </summary>
public abstract class EfDbContextBase(DbContextOptions options)
    : DbContext(options),
        IDbFacadeResolver,
        IDbContext,
        IDomainEventContext
{
    // private readonly IDomainEventPublisher _domainEventPublisher;
    private IDbContextTransaction? _currentTransaction;

    #region DbContext

    /// <summary>
    /// Configures the model by adding soft deletes and versioning.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        AddingSoftDeletes(modelBuilder);
        AddingVersioning(modelBuilder);
    }

    #region Private Methods

    /// <summary>
    /// Adds versioning to entities that implement IHaveAggregateVersion.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    private static void AddingVersioning(ModelBuilder builder)
    {
        var types = builder.Model.GetEntityTypes().Where(x => x.ClrType.IsAssignableTo(typeof(IHaveAggregateVersion)));
        foreach (var entityType in types)
        {
            builder
                .Entity(entityType.ClrType)
                .Property(nameof(IHaveAggregateVersion.OriginalVersion))
                .IsConcurrencyToken();
        }
    }

    /// <summary>
    /// Adds soft delete functionality to entities that implement IHaveSoftDelete.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    private static void AddingSoftDeletes(ModelBuilder builder)
    {
        var types = builder.Model.GetEntityTypes().Where(x => x.ClrType.IsAssignableTo(typeof(IHaveSoftDelete)));
        foreach (var entityType in types)
        {
            // 1. Add the IsDeleted property
            entityType.AddProperty("IsDeleted", typeof(bool));

            // 2. Create the query filter
            var parameter = Expression.Parameter(entityType.ClrType);

            // EF.Property<bool>(TEntity, "IsDeleted")
            var propertyMethodInfo = typeof(EF).GetMethod("Property")?.MakeGenericMethod(typeof(bool));
            var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));

            // EF.Property<bool>(TEntity, "IsDeleted") == false
            BinaryExpression compareExpression = Expression.MakeBinary(
                ExpressionType.Equal,
                isDeletedProperty,
                Expression.Constant(false)
            );

            // TEntity => EF.Property<bool>(TEntity, "IsDeleted") == false
            var lambda = Expression.Lambda(compareExpression, parameter);

            builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }

    #endregion

    #endregion

    #region IDbContext

    /// <summary>
    /// Begins a new transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    #region IRetryDbContextExecution

    /// <summary>
    /// Executes the specified operation, retrying on exception.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RetryOnExceptionAsync(Func<Task> operation)
    {
        return Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    /// <summary>
    /// Executes the specified operation, retrying on exception.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
    {
        return Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    #endregion

    #region ITransactionDbContextExecution

    /// <summary>
    /// Executes the specified action within a transaction.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    /// <summary>
    /// Executes the specified action within a transaction.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await action();

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    #endregion

    #endregion

    #region IDomainEventContext

    /// <summary>
    /// Gets all uncommitted domain events.
    /// </summary>
    /// <returns>A read-only list of uncommitted domain events.</returns>
    public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
    {
        var domainEvents = ChangeTracker
            .Entries<IHaveAggregate>()
            .Where(x => x.Entity.GetUncommittedDomainEvents().Any())
            .SelectMany(x => x.Entity.GetUncommittedDomainEvents())
            .ToList();

        return domainEvents.ToImmutableList();
    }

    /// <summary>
    /// Marks all uncommitted domain events as committed.
    /// </summary>
    public void MarkUncommittedDomainEventAsCommitted()
    {
        ChangeTracker
            .Entries<IHaveAggregate>()
            .Where(x => x.Entity.GetUncommittedDomainEvents().Any())
            .ToList()
            .ForEach(x => x.Entity.MarkUncommittedDomainEventAsCommitted());
    }

    #endregion
}