using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Core.Linq;
using BuildingBlocks.Core.Queries;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Persistence.EFCore.Extensions;

/// <summary>
/// Provides extension methods for querying and manipulating data using Entity Framework Core.
/// </summary>
public static class EfCoreQueryableExtensions
{
    /// <summary>
    /// Applies pagination to a queryable collection and returns a paginated list of items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="collection">The queryable collection.</param>
    /// <param name="page">The requested page number.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of items.</returns>
    public static async Task<ListResultModel<T>> ApplyPagingAsync<T>(
        this IQueryable<T> collection,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default) where T : notnull
    {
        if (page <= 0)
            page = 1;

        if (pageSize <= 0)
            pageSize = 10;

        var isEmpty = await collection.AnyAsync(cancellationToken: cancellationToken) == false;
        if (isEmpty)
            return ListResultModel<T>.Empty;

        var totalItems = await collection.CountAsync(cancellationToken: cancellationToken);

        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);

        var data = await collection
            .Limit(page, pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return ListResultModel<T>.Create(data, totalItems, page, pageSize);
    }

    /// <summary>
    /// Projects a queryable collection to another type, applies pagination, and returns a paginated list of projected items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the original collection.</typeparam>
    /// <typeparam name="TR">The type to project the items to.</typeparam>
    /// <param name="collection">The queryable collection.</param>
    /// <param name="configuration">The AutoMapper configuration provider.</param>
    /// <param name="page">The requested page number.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of projected items.</returns>
    public static async Task<ListResultModel<TR>> ApplyPagingAsync<T, TR>(
        this IQueryable<T> collection,
        IConfigurationProvider configuration,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default) where TR : notnull
    {
        if (page <= 0)
            page = 1;

        if (pageSize <= 0)
            pageSize = 10;

        var isEmpty = await collection.AnyAsync(cancellationToken: cancellationToken) == false;
        if (isEmpty)
            return ListResultModel<TR>.Empty;

        var totalItems = await collection.CountAsync(cancellationToken: cancellationToken);
        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
        var data = await collection
            .Limit(page, pageSize)
            .ProjectTo<TR>(configuration)
            .ToListAsync(cancellationToken: cancellationToken);

        return ListResultModel<TR>.Create(data, totalItems, page, pageSize);
    }

    /// <summary>
    /// Applies pagination to a queryable collection.
    /// </summary>
    /// <typeparam name="TEntity">The type of the items in the collection.</typeparam>
    /// <param name="source">The queryable collection.</param>
    /// <param name="page">The requested page number.</param>
    /// <param name="size">The requested page size.</param>
    /// <returns>A queryable collection with pagination applied.</returns>
    public static IQueryable<TEntity> ApplyPaging<TEntity>(
        this IQueryable<TEntity> source,
        int page,
        int size) where TEntity : class
    {
        return source.Skip(page * size).Take(size);
    }

    /// <summary>
    /// Limits the number of items in a queryable collection based on the specified page and results per page.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="collection">The queryable collection.</param>
    /// <param name="page">The requested page number.</param>
    /// <param name="resultsPerPage">The number of results per page.</param>
    /// <returns>A queryable collection with the specified limits applied.</returns>
    public static IQueryable<T> Limit<T>(
        this IQueryable<T> collection,
        int page = 1,
        int resultsPerPage = 10)
    {
        if (page <= 0)
            page = 1;

        if (resultsPerPage <= 0)
            resultsPerPage = 10;

        var skip = (page - 1) * resultsPerPage;
        var data = collection.Skip(skip).Take(resultsPerPage);

        return data;
    }

    /// <summary>
    /// Applies filters to a queryable collection based on the specified filter models.
    /// </summary>
    /// <typeparam name="TEntity">The type of the items in the collection.</typeparam>
    /// <param name="source">The queryable collection.</param>
    /// <param name="filters">The filter models.</param>
    /// <returns>A queryable collection with the filters applied.</returns>
    public static IQueryable<TEntity> ApplyFilter<TEntity>(
        this IQueryable<TEntity> source,
        IEnumerable<FilterModel>? filters) where TEntity : class
    {
        if (filters is null)
            return source;

        var filterExpressions = new List<Expression<Func<TEntity, bool>>>();

        foreach (var (fieldName, comparison, fieldValue) in filters)
        {
            var expr = PredicateBuilder.Build<TEntity>(
                fieldName,
                comparison,
                fieldValue);

            filterExpressions.Add(expr);
        }

        return source.Where(filterExpressions.Aggregate(
            (expr1, expr2) => expr1.And(expr2)));
    }

    /// <summary>
    /// Applies to include paths to a queryable collection for related data.
    /// </summary>
    /// <typeparam name="TEntity">The type of the items in the collection.</typeparam>
    /// <param name="source">The queryable collection.</param>
    /// <param name="navigationPropertiesPath">The list of include paths.</param>
    /// <returns>A queryable collection with the include paths applied.</returns>
    public static IQueryable<TEntity> ApplyIncludeList<TEntity>(
        this IQueryable<TEntity> source,
        IEnumerable<string>? navigationPropertiesPath) where TEntity : class
    {
        return navigationPropertiesPath is null
            ? source
            : navigationPropertiesPath
                .Aggregate(
                    source,
                    (current, navigationPropertyPath) =>
                        current.Include(navigationPropertyPath));
    }
}