using BuildingBlocks.Abstractions.Commands;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Abstractions.Web.MinimalApi;

/// <summary>
/// Defines properties for an HTTP command with a specified request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IHttpCommand<TRequest>
{
    /// <summary>
    /// Gets the request.
    /// </summary>
    TRequest Request { get; init; }

    /// <summary>
    /// Gets the HTTP context.
    /// </summary>
    HttpContext HttpContext { get; init; }

    /// <summary>
    /// Gets the command bus.
    /// </summary>
    ICommandBus CommandBus { get; init; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    CancellationToken CancellationToken { get; init; }
}

/// <summary>
/// Defines properties for an HTTP command.
/// </summary>
public interface IHttpCommand
{
    /// <summary>
    /// Gets the HTTP context.
    /// </summary>
    HttpContext HttpContext { get; init; }

    /// <summary>
    /// Gets the command bus.
    /// </summary>
    ICommandBus CommandBus { get; init; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    CancellationToken CancellationToken { get; init; }
}