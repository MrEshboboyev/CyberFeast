using AutoMapper;
using BuildingBlocks.Abstractions.Core.Paging;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Core.Paging;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Identity.Users.Dtos.v1;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Users.Features.GettingUsers.v1;

internal static class GetUsersEndpoint
{
    internal static RouteHandlerBuilder MapGetUsersByPageEndpoint(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", Handle)
            .RequireAuthorization()
            .WithTags(UsersConfigs.Tag)
            .WithName(nameof(GetUsers))
            .WithSummaryAndDescription(nameof(GetUsers).Humanize(), nameof(GetUsers).Humanize())
            .WithDisplayName(nameof(GetUsers).Humanize())
            // .Produces<GetProductsResponse>("Products fetched successfully.", StatusCodes.Status200OK)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            // .ProducesProblem(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetUsersResponse>, ValidationProblem, UnAuthorizedHttpProblemResult>> Handle(
            [AsParameters] GetUsersRequestParameters requestParameters
        )
        {
            var (context, queryProcessor, mapper, cancellationToken) = requestParameters;

            var query = GetUsers.Of(
                new PageRequest
                {
                    PageNumber = requestParameters.PageNumber,
                    PageSize = requestParameters.PageSize,
                    SortOrder = requestParameters.SortOrder,
                    Filters = requestParameters.SortOrder
                }
            );

            var result = await queryProcessor.SendAsync(query, cancellationToken);

            return TypedResults.Ok(new GetUsersResponse(result.IdentityUsers));
        }
    }
}

internal record GetUsersRequestParameters(
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : PageRequest, IHttpQuery;

internal record GetUsersResponse(IPageList<IdentityUserDto> IdentityUsers);
