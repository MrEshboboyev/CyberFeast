using BuildingBlocks.Abstractions.Queries;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Abstractions.Web.MinimalApi;

/// <summary>
/// Defines properties for an HTTP query.
/// </summary>
public interface IHttpQuery
{
    /// <summary>
    /// Gets the HTTP context.
    /// </summary>
    HttpContext HttpContext { get; init; }

    /// <summary>
    /// Gets the query bus.
    /// </summary>
    IQueryBus QueryBus { get; init; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    CancellationToken CancellationToken { get; init; }
}