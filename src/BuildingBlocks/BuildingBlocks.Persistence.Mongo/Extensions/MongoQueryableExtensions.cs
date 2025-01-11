using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions;
using BuildingBlocks.Core.Linq;
using BuildingBlocks.Core.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace BuildingBlocks.Persistence.Mongo.Extensions;

/// <summary>
/// Provides extension methods for MongoDB IQueryable collections.
/// </summary>
public static class MongoQueryableExtensions
{
    /// <summary>
    /// Applies pagination to the collection and returns a paginated list of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The MongoDB collection to paginate.</param>
    /// <param name="page">The page number to retrieve (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of items.</returns>
    public static async Task<ListResultModel<T>> ApplyPagingAsync<T>(
        this IMongoQueryable<T> collection,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {
        if (page <= 0) page = 1; // Ensure the page number is at least 1.
        if (pageSize <= 0) pageSize = 10; // Ensure the page size is at least 10.

        var isEmpty = await collection.AnyAsync(cancellationToken: cancellationToken) == false;
        if (isEmpty) return ListResultModel<T>.Empty; // Return an empty list if the collection is empty.

        var totalItems = await collection.CountAsync(cancellationToken: cancellationToken); // Count the total items.
        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize); // Calculate the total pages.
        var data = await collection.Skip(page, pageSize)
            .ToListAsync(cancellationToken: cancellationToken); // Get the paginated data.

        return ListResultModel<T>.Create(data, totalItems, page, pageSize); // Return the paginated list.
    }

    /// <summary>
    /// Applies pagination and projection to the collection, returning a paginated list of transformed items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <typeparam name="TR">The type of items to return.</typeparam>
    /// <param name="collection">The MongoDB collection to paginate and project.</param>
    /// <param name="configuration">The AutoMapper configuration provider.</param>
    /// <param name="page">The page number to retrieve (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of transformed items.</returns>
    public static async Task<ListResultModel<TR>> ApplyPagingAsync<T, TR>(
        this IMongoQueryable<T> collection,
        IConfigurationProvider configuration,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default
    ) where TR : notnull
    {
        if (page <= 0) page = 1; // Ensure the page number is at least 1.
        if (pageSize <= 0) pageSize = 10; // Ensure the page size is at least 10.

        var isEmpty = await collection.AnyAsync(cancellationToken: cancellationToken) == false;
        if (isEmpty) return ListResultModel<TR>.Empty; // Return an empty list if the collection is empty.

        var totalItems = await collection.CountAsync(cancellationToken: cancellationToken); // Count the total items.
        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize); // Calculate the total pages.
        var data = collection.Skip(page, pageSize).ProjectTo<TR>(configuration)
            .ToList(); // Get the paginated and projected data.

        return ListResultModel<TR>.Create(data, totalItems, page, pageSize); // Return the paginated list.
    }

    /// <summary>
    /// Skips a specified number of items in the collection and returns the remaining items, limited to a specified number of results per page.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The MongoDB collection.</param>
    /// <param name="page">The page number to retrieve (default is 1).</param>
    /// <param name="resultsPerPage">The number of items per page (default is 10).</param>
    /// <returns>The paginated collection of items.</returns>
    public static IMongoQueryable<T> Skip<T>(this IMongoQueryable<T> collection,
        int page = 1,
        int resultsPerPage = 10)
    {
        if (page <= 0) page = 1; // Ensure the page number is at least 1.
        if (resultsPerPage <= 0) resultsPerPage = 10; // Ensure the results per page is at least 10.

        var skip = (page - 1) * resultsPerPage; // Calculate the number of items to skip.
        var data = MongoQueryable.Skip(collection, skip)
            .Take(resultsPerPage); // Skip the items and take the specified number of results.

        return data; // Return the paginated collection.
    }

    /// <summary>
    /// Applies a set of filter expressions to the collection based on the provided filter models.
    /// </summary>
    /// <typeparam name="TEntity">The type of items in the collection.</typeparam>
    /// <param name="source">The MongoDB collection to filter.</param>
    /// <param name="filters">The filter models to apply.</param>
    /// <returns>The filtered collection of items.</returns>
    public static IMongoQueryable<TEntity> ApplyFilter<TEntity>(
        this IMongoQueryable<TEntity> source,
        IEnumerable<FilterModel>? filters
    ) where TEntity : class
    {
        if (filters is null) return source; // Return the source collection if no filters are provided.

        var filterExpressions =
            new List<Expression<Func<TEntity, bool>>>(); // Initialize a list to hold filter expressions.

        foreach (var (fieldName, comparision, fieldValue) in filters)
        {
            var expr = PredicateBuilder.Build<TEntity>(fieldName, comparision,
                fieldValue); // Build the filter expression.
            filterExpressions.Add(expr); // Add the filter expression to the list.
        }

        return source.Where(filterExpressions
            .Aggregate((expr1, expr2) => expr1.And(expr2))); // Apply the filter expressions to the collection.
    }
}