namespace BuildingBlocks.Abstractions.Queries;

/// <summary>
/// Defines methods for sending queries and stream queries.
/// </summary>
public interface IQueryBus
{
    /// <summary>
    /// Asynchronously sends a query and returns a result.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of the query.</returns>
    Task<TResponse> SendAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
        where TResponse : notnull;

    /// <summary>
    /// Asynchronously sends a stream query and returns a stream of results.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The stream query to send.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An async enumerable of the results of the stream query.</returns>
    IAsyncEnumerable<TResponse> SendAsync<TResponse>(
        IStreamQuery<TResponse> query,
        CancellationToken cancellationToken = default)
        where TResponse : notnull;
}