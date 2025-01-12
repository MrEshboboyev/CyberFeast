using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web.Minimal.Extensions;

/// <summary>
/// Provides extension methods for mapping command and query endpoints for minimal APIs.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a POST endpoint for handling a command with a request and command.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="pattern">The URL pattern for the endpoint.</param>
    /// <param name="mapRequestToCommand">Optional function to map the request to a command.</param>
    /// <returns>The configured route handler builder.</returns>
    public static RouteHandlerBuilder MapCommandEndpoint<TRequest, TCommand>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Func<TRequest, TCommand>? mapRequestToCommand = null)
        where TRequest : class
        where TCommand : ICommand
    {
        // Map a POST endpoint and configure its metadata
        return builder.MapPost(pattern, Handle)
            .WithName(typeof(TCommand).Name)
            .WithDisplayName(typeof(TCommand).Name.Humanize())
            .WithSummaryAndDescription(
                typeof(TCommand).Name.Humanize(), typeof(TCommand).Name.Humanize());

        // Handle the command asynchronously
        async Task<NoContent> Handle([AsParameters] HttpCommand<TRequest> requestParameters)
        {
            var (request, _, commandBus, mapper, cancellationToken) = requestParameters;

            var command = mapRequestToCommand is not null
                ? mapRequestToCommand(request)
                : mapper.Map<TCommand>(request);

            await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.NoContent();
        }
    }

    /// <summary>
    /// Maps a POST endpoint for handling a command with a request, response, command, and command result.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="pattern">The URL pattern for the endpoint.</param>
    /// <param name="statusCode">The status code for the response.</param>
    /// <param name="mapRequestToCommand">Optional function to map the request to a command.</param>
    /// <param name="mapCommandResultToResponse">Optional function to map the command result to a response.</param>
    /// <param name="getId">Optional function to get the ID from the response.</param>
    /// <returns>The configured route handler builder.</returns>
    public static RouteHandlerBuilder MapCommandEndpoint<
        TRequest, TResponse, TCommand, TCommandResult>(
        this IEndpointRouteBuilder builder,
        string pattern,
        int statusCode,
        Func<TRequest, TCommand>? mapRequestToCommand = null,
        Func<TCommandResult, TResponse>? mapCommandResultToResponse = null,
        Func<TResponse, long>? getId = null)
        where TRequest : class
        where TResponse : class
        where TCommand : ICommand<TCommandResult>
        where TCommandResult : class
    {
        // Map a POST endpoint and configure its metadata
        return builder.MapPost(pattern, Handle)
            .WithName(typeof(TCommand).Name)
            .WithDisplayName(typeof(TCommand).Name.Humanize())
            .WithSummaryAndDescription(
                typeof(TCommand).Name.Humanize(), typeof(TCommand).Name.Humanize());

        // Handle the command asynchronously and return the appropriate response
        async Task<IResult> Handle([AsParameters] HttpCommand<TRequest> requestParameters)
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;
            var host = $"{context.Request.Scheme}://{context.Request.Host}";

            var command = mapRequestToCommand is not null
                ? mapRequestToCommand(request)
                : mapper.Map<TCommand>(request);

            var result = await commandBus.SendAsync(command, cancellationToken);

            var response = mapCommandResultToResponse is not null
                ? mapCommandResultToResponse(result)
                : mapper.Map<TResponse>(result);

            return statusCode switch
            {
                StatusCodes.Status201Created => getId is not null
                    ? TypedResults.Created($"{host}{pattern}/{getId(response)}", response)
                    : TypedResults.Ok(response),
                StatusCodes.Status401Unauthorized => TypedResultsExtensions.UnAuthorizedProblem(),
                StatusCodes.Status500InternalServerError => TypedResultsExtensions.InternalProblem(),
                StatusCodes.Status202Accepted => TypedResults.Accepted(new Uri($"{host}{pattern}"), response),
                _ => TypedResults.Ok(response)
            };
        }
    }

    /// <summary>
    /// Maps a GET endpoint for handling a query with request parameters, response, query, and query result.
    /// </summary>
    /// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TQueryResult">The type of the query result.</typeparam>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="pattern">The URL pattern for the endpoint.</param>
    /// <param name="mapRequestToQuery">Function to map the request parameters to the query.</param>
    /// <param name="mapQueryResultToResponse">Function to map the query result to the response.</param>
    /// <returns>The configured route handler builder.</returns>
    public static RouteHandlerBuilder MapQueryEndpoint<
        TRequestParameters, TResponse, TQuery, TQueryResult>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Func<TRequestParameters, TQuery> mapRequestToQuery,
        Func<TQueryResult, TResponse> mapQueryResultToResponse)
        where TRequestParameters : IHttpQuery
        where TResponse : class
        where TQuery : IQuery<TQueryResult>
        where TQueryResult : class
    {
        // Map a GET endpoint and configure its metadata
        return builder.MapGet(pattern, Handle)
            .WithName(typeof(TQuery).Name)
            .WithDisplayName(typeof(TQuery).Name.Humanize())
            .WithSummaryAndDescription(
                typeof(TQuery).Name.Humanize(), typeof(TQuery).Name.Humanize());

        // Handle the query asynchronously and return the appropriate response
        async Task<Ok<TResponse>> Handle([AsParameters] TRequestParameters requestParameters)
        {
            var queryProcessor = requestParameters.QueryBus;
            var cancellationToken = requestParameters.CancellationToken;

            var query = mapRequestToQuery(requestParameters);

            var result = await queryProcessor.SendAsync(query, cancellationToken);

            var response = mapQueryResultToResponse(result);

            return TypedResults.Ok(response);
        }
    }
}