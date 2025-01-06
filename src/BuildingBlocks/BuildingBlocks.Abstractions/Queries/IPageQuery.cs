using BuildingBlocks.Abstractions.Core.Paging;

namespace BuildingBlocks.Abstractions.Queries;

/// <summary>
/// Defines a query to retrieve a paginated list of items.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IPageQuery<out TResponse> : IPageRequest, IQuery<TResponse>
    where TResponse : notnull
{
}