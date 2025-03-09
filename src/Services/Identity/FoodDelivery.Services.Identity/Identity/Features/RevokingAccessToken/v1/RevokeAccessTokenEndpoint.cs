using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.Web.Minimal.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Identity.Features.RevokingAccessToken.v1;

public static class RevokeAccessTokenEndpoint
{
    public static RouteHandlerBuilder MapRevokeAccessTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/revoke-token", Handle)
            .RequireAuthorization(IdentityConstants.Role.User)
            .MapToApiVersion(1.0)
            // .Produces(StatusCodes.Status204NoContent)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(RevokeAccessToken))
            .WithDisplayName(nameof(RevokeAccessToken).Humanize())
            .WithSummaryAndDescription(nameof(RevokeAccessToken).Humanize(), nameof(RevokeAccessToken).Humanize());

        async Task<Results<NoContent, ValidationProblem>> Handle(
            [AsParameters] RevokeAccessTokenRequestParameters requestParameters
        )
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;
            var token = string.IsNullOrWhiteSpace(request.AccessToken)
                ? GetTokenFromHeader(context)
                : request.AccessToken;

            var command = RevokeAccessToken.Of(token, context.User.Identity!.Name!);
            await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.NoContent();
        }
    }

    private static string? GetTokenFromHeader(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Get<string>("authorization");

        return authorizationHeader;
    }
}

public record RevokeAccessTokenRequest(string? AccessToken);

internal record RevokeAccessTokenRequestParameters(
    [FromBody] RevokeAccessTokenRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<RevokeAccessTokenRequest>;
