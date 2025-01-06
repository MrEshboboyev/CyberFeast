using BuildingBlocks.Abstractions.Queries;

namespace BuildingBlocks.Abstractions.Caching;

/// <summary>
/// Defines a query that is cacheable.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICacheQuery<in TRequest, TResponse> :
    IQuery<TResponse>,
    ICacheRequest<TRequest, TResponse>
    where TResponse : class
    where TRequest : IQuery<TResponse>
{
}