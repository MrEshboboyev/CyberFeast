using BuildingBlocks.Abstractions.Core.Paging;

namespace BuildingBlocks.Abstractions.Queries;

/// <summary>
/// Defines a query to retrieve a list of items with pagination support.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IListQuery<out TResponse> :
    IPageRequest,
    IQuery<TResponse>
    where TResponse : notnull
{
}