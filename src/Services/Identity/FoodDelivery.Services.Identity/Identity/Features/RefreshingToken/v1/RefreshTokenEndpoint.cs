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

namespace FoodDelivery.Services.Identity.Identity.Features.RefreshingToken.v1;

public static class RefreshTokenEndpoint
{
    internal static RouteHandlerBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/refresh-token", Handle)
            .RequireAuthorization()
            // .Produces<RefreshTokenResult>(StatusCodes.Status200OK))
            // .ProducesProblem(StatusCodes.Status404NotFound)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(RefreshToken))
            .WithDisplayName(nameof(RefreshToken).Humanize())
            .WithSummaryAndDescription(nameof(RefreshToken).Humanize(), nameof(RefreshToken).Humanize());

        async Task<Results<Ok<RefreshTokenResponse>, NotFoundHttpProblemResult, ValidationProblem>> Handle(
            [AsParameters] RefreshTokenRequestParameters requestParameters
        )
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;

            var command = RefreshToken.Of(request.AccessToken, request.RefreshToken);

            var result = await commandBus.SendAsync(command, cancellationToken);

            var response = mapper.Map<RefreshTokenResponse>(result);

            return TypedResults.Ok(response);
        }
    }
}

internal record RefreshTokenRequestParameters(
    [FromBody] RefreshTokenRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<RefreshTokenRequest>;

// These parameters can be pass null from the user
public record RefreshTokenRequest(string? AccessToken, string? RefreshToken);

internal record RefreshTokenResponse(
    Guid UserId,
    string UserName,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken
);
