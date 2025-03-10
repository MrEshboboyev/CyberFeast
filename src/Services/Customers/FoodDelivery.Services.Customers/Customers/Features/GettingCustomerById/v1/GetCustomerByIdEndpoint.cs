using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Customers.Customers.Dtos.v1;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Customers.Customers.Features.GettingCustomerById.v1;

internal class GetCustomerByIdEndpointEndpoint
    : IQueryMinimalEndpoint<
        GetCustomerByIdRequestParameters,
        Ok<GetCustomerByIdResponse>,
        ValidationProblem,
        NotFoundHttpProblemResult,
        UnAuthorizedHttpProblemResult
    >
{
    public string GroupName => CustomersConfigs.Tag;
    public string PrefixRoute => CustomersConfigs.CustomersPrefixUri;
    public double Version => 1.0;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder
            .MapGet("/{id:guid}", HandleAsync)
            .RequireAuthorization()
            .RequireAuthorization()
            .WithName(nameof(GetCustomerById))
            .WithDisplayName(nameof(GetCustomerById).Humanize())
            .WithSummaryAndDescription(nameof(GetCustomerById).Humanize(), nameof(GetCustomerById).Humanize());

        // .Produces<GetCustomerByIdResponse>("Customer fetched successfully.", StatusCodes.Status200OK)
        // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        // .ProducesProblem(StatusCodes.Status404NotFound)
        // .ProducesProblem(StatusCodes.Status401Unauthorized)
    }

    public async Task<
        Results<
            Ok<GetCustomerByIdResponse>,
            ValidationProblem,
            NotFoundHttpProblemResult,
            UnAuthorizedHttpProblemResult
        >
    > HandleAsync([AsParameters] GetCustomerByIdRequestParameters requestParameters)
    {
        var (id, _, queryProcessor, cancellationToken) = requestParameters;
        var result = await queryProcessor.SendAsync(GetCustomerById.Of(id), cancellationToken);

        return TypedResults.Ok(new GetCustomerByIdResponse(result.Customer));
    }
}

internal record GetCustomerByIdRequestParameters(
    [FromRoute] Guid Id,
    HttpContext HttpContext,
    IQueryBus QueryBus,
    CancellationToken CancellationToken
) : IHttpQuery;

public record GetCustomerByIdResponse(CustomerReadDto Customer);
