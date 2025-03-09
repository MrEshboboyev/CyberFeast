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

namespace FoodDelivery.Services.Identity.Users.Features.GettingUserByEmail.v1;

public static class GetUserByEmailEndpoint
{
    internal static RouteHandlerBuilder MapGetUserByEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/by-email/{email}", Handle)
            .AllowAnonymous()
            .WithTags(UsersConfigs.Tag)
            .WithName(nameof(GetUserByEmail))
            .WithDisplayName(nameof(GetUserByEmail).Humanize())
            .WithSummaryAndDescription(nameof(GetUserByEmail).Humanize(), nameof(GetUserByEmail).Humanize())
            // .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
            // .ProducesProblem(StatusCodes.Status404NotFound)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetUserByEmailResponse>, ValidationProblem, NotFoundHttpProblemResult>> Handle(
            [AsParameters] GetUserByEmailRequestParameters requestParameters
        )
        {
            var (email, _, queryProcessor, mapper, cancellationToken) = requestParameters;
            var result = await queryProcessor.SendAsync(GetUserByEmail.Of(email), cancellationToken);

            return TypedResults.Ok(new GetUserByEmailResponse(result.UserIdentity));
        }
    }
}

internal record GetUserByEmailRequestParameters(
    [FromRoute] string Email,
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;

internal record GetUserByEmailResponse(IdentityUserDto? UserIdentity);
