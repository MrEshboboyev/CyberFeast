using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Customers.Customers.Features.GettingCustomerByCustomerId.v1;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Customers.Customers.Features.CreatingCustomer.v1;

internal class CreateCustomerEndpoint
    : ICommandMinimalEndpoint<
        CreateCustomerRequest,
        CreateCustomerRequestParameters,
        CreatedAtRoute<CreateCustomerResponse>,
        UnAuthorizedHttpProblemResult,
        ValidationProblem
    >
{
    public string GroupName => CustomersConfigs.Tag;
    public string PrefixRoute => CustomersConfigs.CustomersPrefixUri;
    public double Version => 1.0;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder
            .MapPost("/", HandleAsync)
            .RequireAuthorization()
            .WithName(nameof(CreateCustomer))
            .WithDisplayName(nameof(CreateCustomer).Humanize())
            .WithSummaryAndDescription(nameof(CreateCustomer).Humanize(), nameof(CreateCustomer).Humanize());

        // .Produces<CreateCustomerRequest>("Customer created successfully.", StatusCodes.Status201Created)
        // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        // .ProducesProblem("UnAuthorized request.", StatusCodes.Status401Unauthorized)
    }

    public async Task<
        Results<CreatedAtRoute<CreateCustomerResponse>, UnAuthorizedHttpProblemResult, ValidationProblem>
    > HandleAsync([AsParameters] CreateCustomerRequestParameters requestParameters)
    {
        var (request, context, commandBus, cancellationToken) = requestParameters;

        var command = CreateCustomer.Of(request.Email);

        var result = await commandBus.SendAsync(command, cancellationToken);

        return TypedResults.CreatedAtRoute(
            new CreateCustomerResponse(result.CustomerId),
            nameof(GetCustomerByCustomerId),
            new { customerId = result.CustomerId }
        );
    }
}

internal record CreateCustomerRequestParameters(
    [FromBody] CreateCustomerRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    CancellationToken CancellationToken
) : IHttpCommand<CreateCustomerRequest>;

internal record CreateCustomerRequest(string? Email);

public record CreateCustomerResponse(long CustomerId);
