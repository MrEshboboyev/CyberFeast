using AutoMapper;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Minimal;

/// <summary>
/// Represents an HTTP query with an HTTP context, query bus, mapper, and cancellation token.
/// </summary>
public record HttpQuery(
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;