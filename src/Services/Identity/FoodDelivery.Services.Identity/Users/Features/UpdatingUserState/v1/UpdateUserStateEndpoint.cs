using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Identity.Shared.Models;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Users.Features.UpdatingUserState.v1;

internal static class UpdateUserStateEndpoint
{
    internal static RouteHandlerBuilder MapUpdateUserStateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{userId:guid}/state", Handle)
            .AllowAnonymous()
            // .Produces<RegisterUserResponse>(StatusCodes.Status204NoContent)
            // .ProducesProblem(StatusCodes.Status404NotFound)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(UpdateUserState))
            .WithSummaryAndDescription(nameof(UpdateUserState).Humanize(), nameof(UpdateUserState).Humanize())
            .WithDisplayName(nameof(UpdateUserState).Humanize())
            .MapToApiVersion(1.0);

        async Task<Results<NoContent, NotFoundHttpProblemResult, ValidationProblem>> Handle(
            [AsParameters] UpdateUserStateRequestParameters requestParameters
        )
        {
            var (request, userId, context, commandBus, mapper, cancellationToken) = requestParameters;
            var command = UpdateUserState.Of(userId, request.UserState);

            await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.NoContent();
        }
    }
}

internal record UpdateUserStateRequestParameters(
    [FromBody] UpdateUserStateRequest Request,
    [FromRoute] Guid UserId,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<UpdateUserStateRequest>;

internal record UpdateUserStateRequest(UserState UserState);
