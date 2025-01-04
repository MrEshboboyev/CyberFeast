using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Query;

namespace BuildingBlocks.Abstractions.Persistence.EFCore;

/// <summary>
/// Defines methods for an Entity Framework Core repository that supports inclusion of related entities in queries.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IEfRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IHaveIdentity<TId>
{
    /// <summary>
    /// Synchronously gets entities with related data included.
    /// </summary>
    /// <param name="includes">A function to include related entities.</param>
    /// <returns>An enumerable of entities with related data included.</returns>
    IEnumerable<TEntity> GetInclude(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null);

    /// <summary>
    /// Synchronously gets entities with related data included and optionally applies a predicate.
    /// </summary>
    /// <param name="predicate">An expression to filter the entities.</param>
    /// <param name="includes">A function to include related entities.</param>
    /// <param name="withTracking">A boolean value indicating whether to track the entities.</param>
    /// <returns>An enumerable of entities with related data included.</returns>
    IEnumerable<TEntity> GetInclude(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
        bool withTracking = true);

    /// <summary>
    /// Asynchronously gets entities with related data included.
    /// </summary>
    /// <param name="includes">A function to include related entities.</param>
    /// <returns>A task that represents the asynchronous operation and contains an enumerable of entities with related data included.</returns>
    Task<IEnumerable<TEntity>> GetIncludeAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null);

    /// <summary>
    /// Asynchronously gets entities with related data included and optionally applies a predicate.
    /// </summary>
    /// <param name="predicate">An expression to filter the entities.</param>
    /// <param name="includes">A function to include related entities.</param>
    /// <param name="withTracking">A boolean value indicating whether to track the entities.</param>
    /// <returns>A task that represents the asynchronous operation and contains an enumerable of entities with related data included.</returns>
    Task<IEnumerable<TEntity>> GetIncludeAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
        bool withTracking = true);
}