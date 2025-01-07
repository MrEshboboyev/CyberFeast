using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.Core.Paging;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace BuildingBlocks.Core.Persistence.EfCore;

/// <summary>
/// Provides a base implementation for a repository using Entity Framework Core.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public abstract class EfRepositoryBase<TDbContext, TEntity, TKey>(
    TDbContext dbContext,
    ISieveProcessor sieveProcessor)
    : IRepository<TEntity, TKey>
    where TEntity : class, IHaveIdentity<TKey>
    where TDbContext : DbContext
{
    /// <summary>
    /// Gets the set of entities.
    /// </summary>
    protected DbSet<TEntity> DbSet { get; } = dbContext.Set<TEntity>();

    /// <summary>
    /// Gets the database context.
    /// </summary>
    protected TDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Finds an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity, or <c>null</c> if not found.</returns>
    public Task<TEntity?> FindByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default)
    {
        return DbSet.SingleOrDefaultAsync(
            e => e.Id!.Equals(id),
            cancellationToken);
    }

    /// <summary>
    /// Finds an entity by a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity, or <c>null</c> if not found.</returns>
    public Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        predicate.NotBeNull();

        return DbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Finds entities by a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of matching entities.</returns>
    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if any entities match a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if any entities match the predicate; otherwise, <c>false</c>.</returns>
    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of all entities.</returns>
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Projects entities to another type using AutoMapper.
    /// </summary>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="configuration">The AutoMapper configuration.</param>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="sortExpression">The expression to sort entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous enumerable of projected entities.</returns>
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
    /// Retrieves entities with pagination and optional filtering.
    /// </summary>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="pageRequest">The page request.</param>
    /// <param name="sortExpression">The expression to sort entities.</param>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of entities.</returns>
    public async Task<IPageList<TEntity>> GetByPageFilter<TSortKey>(
        IPageRequest pageRequest,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyPagingAsync(
            pageRequest,
            sieveProcessor,
            predicate,
            sortExpression,
            cancellationToken);
    }

    /// <summary>
    /// Retrieves entities with pagination, filtering, and projection.
    /// </summary>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="pageRequest">The page request.</param>
    /// <param name="configuration">The AutoMapper configuration.</param>
    /// <param name="sortExpression">The expression to sort entities.</param>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected entities.</returns>
    public async Task<IPageList<TResult>> GetByPageFilter<TResult, TSortKey>(
        IPageRequest pageRequest,
        IConfigurationProvider configuration,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        return await DbSet.ApplyPagingAsync<TEntity, TResult, TSortKey>(
            pageRequest,
            sieveProcessor,
            configuration,
            predicate,
            sortExpression,
            cancellationToken);
    }

    /// <summary>
    /// Retrieves entities with pagination, filtering, and projection.
    /// </summary>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="pageRequest">The page request.</param>
    /// <param name="projectionFunc">The function to project entities.</param>
    /// <param name="sortExpression">The expression to sort entities.</param>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected entities.</returns>
    public async Task<IPageList<TResult>> GetByPageFilter<TResult, TSortKey>(
        IPageRequest pageRequest,
        Func<IQueryable<TEntity>, IQueryable<TResult>> projectionFunc,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        return await DbSet.ApplyPagingAsync<TEntity, TResult, TSortKey>(
            pageRequest,
            sieveProcessor,
            projectionFunc,
            predicate,
            sortExpression,
            cancellationToken);
    }

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added entity.</returns>
    public async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        entity.NotBeNull();

        await DbSet.AddAsync(entity, cancellationToken);

        return entity;
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated entity.</returns>
    public Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        entity.NotBeNull();

        var entry = DbContext.Entry(entity);
        entry.State = EntityState.Modified;

        return Task.FromResult(entry.Entity);
    }

    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteRangeAsync(
        IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        entities.NotBeNull();

        foreach (var entity in entities)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// Deletes entities matching a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var items = await DbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);

        await DeleteRangeAsync(items, cancellationToken);
    }

    /// <summary>
    /// Deletes a single entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        entity.NotBeNull();

        DbSet.Remove(entity);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NotFoundException">Thrown if the entity is not found.</exception>
    public async Task DeleteByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default)
    {
        var item = await DbSet.SingleOrDefaultAsync(
            e => e.Id!.Equals(id),
            cancellationToken);

        if (item is null)
            throw new NotFoundException($"Item with ID '{id}' not found");

        DbSet.Remove(item);
    }

    /// <summary>
    /// Disposes the repository.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}