using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.Core.Paging;
using BuildingBlocks.Core.Paging;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace BuildingBlocks.Core.Extensions;

/// <summary>
/// Provides extension methods for applying pagination, sorting, and filtering using Sieve on IQueryable objects.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination, sorting, and filtering to an IQueryable<TEntity>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="queryable">The queryable object.</param>
    /// <param name="pageRequest">The page request containing pagination parameters.</param>
    /// <param name="sieveProcessor">The Sieve processor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of entities.</returns>
    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        var result = sieveProcessor.Apply(
            sieveModel, 
            queryable,
            applyPagination: false);
#pragma warning disable AsyncFixer02
        var total = result.Count();
#pragma warning restore AsyncFixer02
        result = sieveProcessor.Apply(
            sieveModel, 
            queryable,
            applyFiltering: false,
            applySorting: false);

        var items = await result
            .AsNoTracking()
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TEntity>
            .Create(
                items.AsReadOnly(),
                pageRequest.PageNumber, 
                pageRequest.PageSize,
                total);
    }

    /// <summary>
    /// Projects entities to another type and applies pagination, sorting, and filtering.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <param name="queryable">The queryable object.</param>
    /// <param name="pageRequest">The page request containing pagination parameters.</param>
    /// <param name="sieveProcessor">The Sieve processor.</param>
    /// <param name="configurationProvider">The AutoMapper configuration provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected entities.</returns>
    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        IConfigurationProvider configurationProvider,
        CancellationToken cancellationToken)
        where TEntity : class
        where TResult : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        var result = sieveProcessor.Apply(
            sieveModel,
            queryable,
            applyPagination: false);
#pragma warning disable AsyncFixer02
        var total = result.Count();
#pragma warning restore AsyncFixer02
        result = sieveProcessor.Apply(
            sieveModel,
            queryable,
            applyFiltering: false,
            applySorting: false); // Only applies pagination

        var projectedQuery = result.ProjectTo<TResult>(configurationProvider);

        var items = await projectedQuery
            .AsNoTracking()
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(
            items.AsReadOnly(),
            pageRequest.PageNumber,
            pageRequest.PageSize, total);
    }

    /// <summary>
    /// Projects entities to another type and applies pagination, sorting, and filtering.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <param name="queryable">The queryable object.</param>
    /// <param name="pageRequest">The page request containing pagination parameters.</param>
    /// <param name="sieveProcessor">The Sieve processor.</param>
    /// <param name="projectionFunc">The function to project entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected entities.</returns>
    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Func<IQueryable<TEntity>, IQueryable<TResult>> projectionFunc,
        CancellationToken cancellationToken)
        where TEntity : class
        where TResult : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        var result = sieveProcessor.Apply(
            sieveModel, 
            queryable, 
            applyPagination: false);
#pragma warning disable AsyncFixer02
        // The provider for the source 'IQueryable' doesn't implement 'IAsyncQueryProvider'. Only providers that implement 'IAsyncQueryProvider' can be used for Entity Framework asynchronous operations.
        var total = result.Count();
#pragma warning restore AsyncFixer02
        result = sieveProcessor.Apply(
            sieveModel, 
            queryable, 
            applyFiltering: false,
            applySorting: false); // Only applies pagination

        var projectedQuery = projectionFunc(result);

        var items = await projectedQuery
            .AsNoTracking()
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(
            items.AsReadOnly(), 
            pageRequest.PageNumber, 
            pageRequest.PageSize, 
            total);
    }

    /// <summary>
    /// Projects entities to another type, applies pagination, sorting, and filtering, and allows specifying a sort expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="collection">The queryable collection.</param>
    /// <param name="pageRequest">The page request containing pagination parameters.</param>
    /// <param name="sieveProcessor">The Sieve processor.</param>
    /// <param name="projectionFunc">The function to project entities.</param>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="sortExpression">The expression to sort entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected entities.</returns>
    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Func<IQueryable<TEntity>, IQueryable<TResult>> projectionFunc,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TSortKey>>? sortExpression = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TResult : class
    {
        var query = collection;
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (sortExpression is not null)
        {
            query = query.OrderByDescending(sortExpression);
        }

        return await query.ApplyPagingAsync(pageRequest, sieveProcessor, projectionFunc, cancellationToken);
    }

    /// <summary>
    /// Projects entities to another type and applies pagination, sorting, and filtering using a mapping function.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <param name="queryable">The queryable object.</param>
    /// <param name="pageRequest">The page request containing pagination parameters.</param>
    /// <param name="sieveProcessor">The Sieve processor.</param>
    /// <param name="map">The function to map entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected entities.</returns>
    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Func<TEntity, TResult> map,
        CancellationToken cancellationToken)
        where TEntity : class
        where TResult : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        var result = sieveProcessor.Apply(
            sieveModel,
            queryable,
            applyPagination: false);
#pragma warning disable AsyncFixer02
        var total = result.Count();
#pragma warning restore AsyncFixer02
        result = sieveProcessor.Apply(
            sieveModel, 
            queryable, 
            applyFiltering: false,
            applySorting: false); // Only applies pagination

        var items = await result
            .Select(x => map(x))
            .AsNoTracking()
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(
            items.AsReadOnly(), 
            pageRequest.PageNumber, 
            pageRequest.PageSize, 
            total);
    }

    /// <summary>
    /// Projects entities to another type, applies pagination, sorting, and filtering, and allows specifying a sort expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type to project to.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="collection">The queryable collection.</param>
    /// <param name="pageRequest">The page request containing pagination parameters.</param>
    /// <param name="sieveProcessor">The Sieve processor.</param>
    /// <param name="configuration">The AutoMapper configuration.</param>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="sortExpression">The expression to sort entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected entities.</returns>
    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        IConfigurationProvider configuration,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TSortKey>>? sortExpression = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TResult : class
    {
        var query = collection;
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (sortExpression is not null)
        {
            query = query.OrderByDescending(sortExpression);
        }

        return await query.ApplyPagingAsync<TEntity, TResult>(
            pageRequest,
            sieveProcessor,
            configuration,
            cancellationToken
        );
    }

    /// <summary>
    /// Applies pagination, sorting, and filtering to an IQueryable<TEntity> and allows specifying a sort expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TSortKey">The type of the sort key.</typeparam>
    /// <param name="collection">The queryable collection.</param>
    /// <param name="pageRequest">The page request containing pagination parameters.</param>
    /// <param name="sieveProcessor">The Sieve processor.</param>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="sortExpression">The expression to sort entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of entities.</returns>
    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TSortKey>>? sortExpression = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var query = collection;
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (sortExpression is not null)
        {
            query = query.OrderByDescending(sortExpression);
        }

        return await query.ApplyPagingAsync(
            pageRequest,
            sieveProcessor,
            cancellationToken);
    }
}