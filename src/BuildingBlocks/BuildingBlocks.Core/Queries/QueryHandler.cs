using BuildingBlocks.Abstractions.Queries;

namespace BuildingBlocks.Core.Queries;

/// <summary>
/// Provides a base implementation for query handlers.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the query and returns a response.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    protected abstract Task<TResponse> HandleQueryAsync(
        TQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the request by calling <see cref="HandleQueryAsync"/>.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public Task<TResponse> Handle(
        TQuery request,
        CancellationToken cancellationToken)
    {
        return HandleQueryAsync(request, cancellationToken);
    }
}