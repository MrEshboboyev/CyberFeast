using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.Core.Paging;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Sieve.Services;

namespace BuildingBlocks.Persistence.Mongo;

/// <summary>
/// A base repository class for MongoDB with generic entities and ID types.
/// </summary>
/// <typeparam name="TDbContext">The type of the MongoDB context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity ID.</typeparam>
public class MongoRepositoryBase<TDbContext, TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IHaveIdentity<TId>
    where TDbContext : MongoDbContext
{
    private readonly ISieveProcessor _sieveProcessor;

    /// <summary>
    /// The MongoDB context.
    /// </summary>
    protected TDbContext Context { get; }

    /// <summary>
    /// The MongoDB collection for the entity type.
    /// </summary>
    protected IMongoCollection<TEntity> DbSet { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepositoryBase{TDbContext, TEntity, TId}"/> class.
    /// </summary>
    /// <param name="context">The MongoDB context.</param>
    /// <param name="sieveProcessor">The Sieve processor for filtering and sorting.</param>
    public MongoRepositoryBase(TDbContext context, ISieveProcessor sieveProcessor)
    {
        Context = context;
        _sieveProcessor = sieveProcessor;
        DbSet = Context.GetCollection<TEntity>();
    }

    /// <summary>
    /// Disposes of the repository instance.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finds an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity, or null if not found.</returns>
    public async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
        return await DbSet.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Finds a single entity by a predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity, or null if not found.</returns>
    public Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Find(predicate).SingleOrDefaultAsync(cancellationToken: cancellationToken)!;
    }

    /// <summary>
    /// Finds entities by a predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of entities.</returns>
    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(predicate).ToListAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Checks if any entity satisfies a predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if any entity satisfies the predicate, otherwise false.</returns>
    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Where(predicate);
        var count = await DbSet.CountDocumentsAsync(filter, null, cancellationToken);
        return count > 0;
    }

    /// <summary>
    /// Retrieves all entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of entities.</returns>
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsQueryable().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Projects entities to a specified result type based on a configuration and optional predicate and sortExpression.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="configuration">The AutoMapper configuration provider.</param>
    /// <param name="predicate">An optional predicate to filter entities.</param>
    /// <param name="sortExpression">An optional sort expression.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of the projected results.</returns>
    public IAsyncEnumerable<TResult> ProjectBy<TResult, TSortKey>(
        IConfigurationProvider configuration,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TSortKey>>? sortExpression = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (sortExpression is not null)
        {
            query = query.OrderByDescending(sortExpression);
        }

        return query.ProjectTo<TResult>(configuration).ToAsyncEnumerable();
    }

    /// <summary>
    /// Retrieves entities by applying a paging filter with sorting asynchronously.
    /// </summary>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="pageRequest">The paging request.</param>
    /// <param name="sortExpression">The sort expression.</param>
    /// <param name="predicate">An optional predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of entities.</returns>
    public async Task<IPageList<TEntity>> GetByPageFilter<TSortKey>(
        IPageRequest pageRequest,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsQueryable()
            .ApplyPagingAsync(pageRequest, _sieveProcessor, predicate, sortExpression, cancellationToken);
    }

    /// <summary>
    /// Retrieves projected entities by applying a paging filter with sorting asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="pageRequest">The paging request.</param>
    /// <param name="configuration">The AutoMapper configuration provider.</param>
    /// <param name="sortExpression">The sort expression.</param>
    /// <param name="predicate">An optional predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of projected results.</returns>
    public async Task<IPageList<TResult>> GetByPageFilter<TResult, TSortKey>(
        IPageRequest pageRequest,
        IConfigurationProvider configuration,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        return await DbSet
            .AsQueryable()
            .ApplyPagingAsync<TEntity, TResult, TSortKey>(
                pageRequest,
                _sieveProcessor,
                configuration,
                predicate,
                sortExpression,
                cancellationToken);
    }

    /// <summary>
    /// Retrieves projected entities by applying a paging filter with sorting asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="pageRequest">The paging request.</param>
    /// <param name="projectionFunc">A function to project the query results.</param>
    /// <param name="sortExpression">The sort expression.</param>
    /// <param name="predicate">An optional predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of projected results.</returns>
    public async Task<IPageList<TResult>> GetByPageFilter<TResult, TSortKey>(
        IPageRequest pageRequest,
        Func<IQueryable<TEntity>, IQueryable<TResult>> projectionFunc,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        return await DbSet
            .AsQueryable()
            .ApplyPagingAsync<TEntity, TResult, TSortKey>(
                pageRequest,
                _sieveProcessor,
                projectionFunc,
                predicate,
                sortExpression,
                cancellationToken);
    }

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added entity.</returns>
    public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.AddCommand(async () => { await DbSet.InsertOneAsync(entity, null, cancellationToken); });

        return Task.FromResult(entity);
    }

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated entity.</returns>
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.AddCommand(async () =>
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            await DbSet.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        });

        return Task.FromResult(entity);
    }

    /// <summary>
    /// Deletes a range of entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Context.AddCommand(async () =>
        {
            var idsToDelete = entities.Select(x => x.Id).ToList();
            var filter = Builders<TEntity>.Filter.In(x => x.Id, idsToDelete);
            await DbSet.DeleteManyAsync(filter, cancellationToken);
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes entities matching a predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        Context.AddCommand(async () => { await DbSet.DeleteOneAsync(predicate, cancellationToken); });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a single entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.AddCommand(async () =>
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            await DbSet.DeleteOneAsync(filter, cancellationToken);
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        Context.AddCommand(async () =>
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            await DbSet.DeleteOneAsync(filter, cancellationToken);
        });

        return Task.CompletedTask;
    }
}