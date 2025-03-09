using AutoMapper;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Identity.Users.Dtos.v1;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Users.Features.GettingUserById.v1;

public static class GetUserByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{userId:guid}", Handle)
            .AllowAnonymous()
            // .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
            // .ProducesProblem(StatusCodes.Status404NotFound)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(GetUserById))
            .WithDisplayName(nameof(GetUserById).Humanize())
            .WithSummaryAndDescription(nameof(GetUserById).Humanize(), nameof(GetUserById).Humanize())
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetUserByIdResponse>, ValidationProblem, NotFoundHttpProblemResult>> Handle(
            [AsParameters] GetUserByIdRequestParameters requestParameters
        )
        {
            var (userId, _, queryProcessor, mapper, cancellationToken) = requestParameters;
            var result = await queryProcessor.SendAsync(GetUserById.Of(userId), cancellationToken);

            return TypedResults.Ok(new GetUserByIdResponse(result.IdentityUser));
        }
    }
}

internal record GetUserByIdRequestParameters(
    [FromRoute] Guid UserId,
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;

internal record GetUserByIdResponse(IdentityUserDto? UserIdentity);
