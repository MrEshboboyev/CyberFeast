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

namespace FoodDelivery.Services.Identity.Identity.Features.RevokingRefreshToken.v1;

public static class RevokeRefreshTokenEndpoint
{
    internal static IEndpointRouteBuilder MapRevokeTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/revoke-refresh-token", Handle)
            .RequireAuthorization()
            .MapToApiVersion(1.0)
            // .Produces(StatusCodes.Status204NoContent)
            // .ProducesProblem(StatusCodes.Status404NotFound)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(RevokeRefreshToken))
            .WithDisplayName(nameof(RevokeRefreshToken).Humanize())
            .WithSummaryAndDescription(nameof(RevokeRefreshToken).Humanize(), nameof(RevokeRefreshToken).Humanize());

        return endpoints;

        async Task<Results<NoContent, NotFoundHttpProblemResult, ValidationProblem>> Handle(
            [AsParameters] RevokeRefreshTokenRequestParameters requestParameters
        )
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;

            var command = RevokeRefreshToken.Of(request.RefreshToken);
            await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.NoContent();
        }
    }
}

public record RevokeRefreshTokenRequest(string? RefreshToken);

internal record RevokeRefreshTokenRequestParameters(
    [FromBody] RevokeRefreshTokenRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<RevokeRefreshTokenRequest>;
