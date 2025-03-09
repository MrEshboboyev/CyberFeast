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

namespace FoodDelivery.Services.Identity.Identity.Features.SendingEmailVerificationCode.v1;

public static class SendEmailVerificationCodeEndpoint
{
    internal static RouteHandlerBuilder MapSendEmailVerificationCodeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/send-email-verification-code", Handle)
            .AllowAnonymous()
            .MapToApiVersion(1.0)
            // .Produces(StatusCodes.Status204NoContent)
            // .ProducesProblem(StatusCodes.Status409Conflict)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithName(nameof(SendEmailVerificationCode))
            .WithDisplayName(nameof(SendEmailVerificationCode).Humanize())
            .WithSummaryAndDescription(
                nameof(SendEmailVerificationCode).Humanize(),
                nameof(SendEmailVerificationCode).Humanize()
            );

        async Task<Results<NoContent, ConflictHttpProblemResult, ValidationProblem>> Handle(
            [AsParameters] SendEmailVerificationCodeRequestParameters requestParameters
        )
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;
            var command = SendEmailVerificationCode.Of(request.Email);

            await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.NoContent();
        }
    }
}

public record SendEmailVerificationCodeRequest(string? Email);

internal record SendEmailVerificationCodeRequestParameters(
    [FromBody] SendEmailVerificationCodeRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<SendEmailVerificationCodeRequest>;
