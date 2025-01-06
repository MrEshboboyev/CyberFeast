using MediatR;

namespace BuildingBlocks.Abstractions.Queries;

/// <summary>
/// Defines a query that returns a result of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}

/// <summary>
/// Defines a stream query that returns a stream of results of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public interface IStreamQuery<out T> : IStreamRequest<T>
    where T : notnull
{
}