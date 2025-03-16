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
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TEntity>
            .Create(
                items.AsReadOnly(),
                pageRequest.PageNumber,
                pageRequest.PageSize,
                total);
    }

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
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(
            items.AsReadOnly(),
            pageRequest.PageNumber,
            pageRequest.PageSize, total);
    }

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
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(
            items.AsReadOnly(),
            pageRequest.PageNumber,
            pageRequest.PageSize,
            total);
    }

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
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(
            items.AsReadOnly(),
            pageRequest.PageNumber,
            pageRequest.PageSize,
            total);
    }

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
