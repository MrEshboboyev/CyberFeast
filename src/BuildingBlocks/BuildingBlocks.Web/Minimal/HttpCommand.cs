using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Minimal;

/// <summary>
/// Represents an HTTP command with a request, HTTP context, command bus, mapper, and cancellation token.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public record HttpCommand<TRequest>(
    TRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<TRequest>;