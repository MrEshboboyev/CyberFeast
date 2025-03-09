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

namespace FoodDelivery.Services.Identity.Identity.Features.VerifyingEmail.v1;

public static class VerifyEmailEndpoint
{
    internal static RouteHandlerBuilder MapSendVerifyEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/verify-email", Handle)
            .AllowAnonymous()
            .MapToApiVersion(1.0)
            // .Produces(StatusCodes.Status204NoContent)
            // .ProducesProblem(StatusCodes.Status409Conflict)
            // .ProducesProblem(StatusCodes.Status500InternalServerError)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(VerifyEmail))
            .WithDisplayName(nameof(VerifyEmail).Humanize())
            .WithSummaryAndDescription(nameof(VerifyEmail).Humanize(), nameof(VerifyEmail).Humanize());

        async Task<Results<NoContent, ConflictHttpProblemResult, InternalHttpProblemResult, ValidationProblem>> Handle(
            [AsParameters] VerifyEmailRequestParameters requestParameters
        )
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;

            var command = VerifyEmail.Of(request.Email, request.Code);

            await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.NoContent();
        }
    }
}

internal record VerifyEmailRequest(string? Email, string? Code);

internal record VerifyEmailRequestParameters(
    [FromBody] VerifyEmailRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<VerifyEmailRequest>;
