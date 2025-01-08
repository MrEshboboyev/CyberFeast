using BuildingBlocks.Abstractions.Queries;
using MediatR;

namespace BuildingBlocks.Core.Queries;

/// <summary>
/// Handles sending queries using MediatR.
/// </summary>
public class QueryBus(IMediator mediator) : IQueryBus
{
    /// <summary>
    /// Sends a query and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public Task<TResponse> SendAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default) where TResponse : notnull
    {
        return mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Sends a stream query and returns an asynchronous stream of responses.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The stream query to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of responses.</returns>
    public IAsyncEnumerable<TResponse> SendAsync<TResponse>(
        IStreamQuery<TResponse> query,
        CancellationToken cancellationToken = default) where TResponse : notnull
    {
        return mediator.CreateStream(query, cancellationToken);
    }
}