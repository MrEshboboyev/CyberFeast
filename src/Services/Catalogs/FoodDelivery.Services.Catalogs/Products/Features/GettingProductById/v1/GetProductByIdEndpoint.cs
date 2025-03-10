using AutoMapper;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Catalogs.Products.Dtos.v1;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Catalogs.Products.Features.GettingProductById.v1;

// GET api/v1/catalog/products/{id}
internal static class GetProductByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetProductByIdEndpoint(this IEndpointRouteBuilder app)
    {
        // return app.MapQueryEndpoint<GetProductByIdRequestParameters, GetProductByIdResponse, GetProductById,
        //         GetProductByIdResult>("/{id}")
        return app.MapGet("/{id}", Handle)
            // .RequireAuthorization()
            .WithTags(ProductsConfigs.Tag)
            .WithName(nameof(GetProductById))
            .WithDisplayName(nameof(GetProductById).Humanize())
            .WithSummaryAndDescription(nameof(GetProductById).Humanize(), nameof(GetProductById).Humanize())
            // .Produces<GetProductByIdResponse>("Product fetched successfully.", StatusCodes.Status200OK)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            // .ProducesProblem(StatusCodes.Status404NotFound)
            // .ProducesProblem(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0);

        async Task<
            Results<
                Ok<GetProductByIdResponse>,
                ValidationProblem,
                NotFoundHttpProblemResult,
                UnAuthorizedHttpProblemResult
            >
        > Handle([AsParameters] GetProductByIdRequestParameters requestParameters)
        {
            var (id, _, queryProcessor, mapper, cancellationToken) = requestParameters;
            var result = await queryProcessor.SendAsync(GetProductById.Of(id), cancellationToken);

            return TypedResults.Ok(new GetProductByIdResponse(result.Product));
        }
    }
}

internal record GetProductByIdRequestParameters(
    [FromRoute] long Id,
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;

internal record GetProductByIdResponse(ProductDto Product);
