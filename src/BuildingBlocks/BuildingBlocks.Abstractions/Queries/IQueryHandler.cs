using MediatR;

namespace BuildingBlocks.Abstractions.Queries;

/// <summary>
/// Defines a handler for a query that returns a result of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}

/// <summary>
/// Defines a handler for a stream query that returns a stream of results of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the stream query.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IStreamQueryHandler<in TQuery, out TResponse> : IStreamRequestHandler<TQuery, TResponse>
    where TQuery : IStreamQuery<TResponse>
    where TResponse : notnull
{
}