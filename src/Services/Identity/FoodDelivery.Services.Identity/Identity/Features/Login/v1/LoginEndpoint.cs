using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Identity.Features.Login.v1;

internal static class LoginEndpoint
{
    internal static RouteHandlerBuilder MapLoginUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/login", Handle)
            .AllowAnonymous()
            .MapToApiVersion(1.0)
            // .Produces<LoginResponse>(StatusCodes.Status200OK)
            // .ProducesProblem(StatusCodes.Status404NotFound)
            // .ProducesProblem(StatusCodes.Status500InternalServerError)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(Login))
            .WithDisplayName(nameof(Login).Humanize())
            .WithSummaryAndDescription(nameof(Login).Humanize(), nameof(Login).Humanize());

        async Task<
            Results<Ok<LoginResponse>, InternalHttpProblemResult, ForbiddenHttpProblemResult, ValidationProblem>
        > Handle([AsParameters] LoginRequestParameters requestParameters)
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;

            var command = Login.Of(request.UserNameOrEmail, request.Password, request.Remember);

            var result = await commandBus.SendAsync(command, cancellationToken);

            var response = mapper.Map<LoginResponse>(result);

            return TypedResults.Ok(response);
        }
    }
}

internal record LoginRequestParameters(
    [FromBody] LoginRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<LoginRequest>;

// These parameters can be pass null from the user
internal record LoginRequest(string? UserNameOrEmail, string? Password, bool Remember);

internal record LoginResponse(
    Guid UserId,
    string UserName,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken
);
