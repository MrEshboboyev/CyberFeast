using AutoMapper;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Identity.Features.GettingClaims.v1;

internal static class GetClaimsEndpoint
{
    internal static RouteHandlerBuilder MapGetClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/claims", Handle)
            .RequireAuthorization()
            .WithTags(IdentityConfigs.Tag)
            .WithName(nameof(GetClaims))
            .WithSummaryAndDescription(nameof(GetClaims).Humanize(), nameof(GetClaims).Humanize())
            .WithDisplayName(nameof(GetClaims).Humanize())
            .MapToApiVersion(1.0);
        
        // .Produces<GetClaimsResponse>(statusCode: StatusCodes.Status200OK)
        // .ProducesProblem(StatusCodes.Status401Unauthorized);
        async Task<Results<Ok<GetClaimsResponse>, ValidationProblem, UnAuthorizedHttpProblemResult>> Handle(
            [AsParameters] GetClaimsRequestParameters requestParameters
        )
        {
            var (context, queryProcessor, mapper, cancellationToken) = requestParameters;
            var result = await queryProcessor.SendAsync(GetClaims.Of(), cancellationToken);

            return TypedResults.Ok(new GetClaimsResponse(result.Claims));
        }
    }
}

internal record GetClaimsRequestParameters(
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;

internal record GetClaimsResponse(IEnumerable<ClaimDto>? Claims);
