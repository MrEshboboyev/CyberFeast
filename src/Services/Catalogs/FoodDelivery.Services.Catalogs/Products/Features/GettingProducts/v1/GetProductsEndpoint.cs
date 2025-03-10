using AutoMapper;
using BuildingBlocks.Abstractions.Core.Paging;
using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Core.Paging;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Catalogs.Products.Dtos.v1;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Catalogs.Products.Features.GettingProducts.v1;

internal static class GetProductsEndpoint
{
    internal static RouteHandlerBuilder MapGetProductsByPageEndpoint(this IEndpointRouteBuilder app)
    {
        // return app.MapQueryEndpoint<GetProductsRequestParameters, GetProductsResponse, GetProducts,
        //         GetProductsResult>("/")
        return app.MapGet("/", Handle)
            // .RequireAuthorization()
            .WithTags(ProductsConfigs.Tag)
            .WithName(nameof(GetProducts))
            .WithSummaryAndDescription(nameof(GetProducts).Humanize(), nameof(GetProducts).Humanize())
            .WithDisplayName(nameof(GetProducts).Humanize())
            // Api Documentations will produce automatically by typed result in minimal apis.
            // .Produces<GetProductsResponse>("Products fetched successfully.", StatusCodes.Status200OK)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            // .ProducesProblem(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0);

        async Task<Results<Ok<GetProductsResponse>, ValidationProblem, UnAuthorizedHttpProblemResult>> Handle(
            [AsParameters] GetProductsRequestParameters requestParameters
        )
        {
            var (context, queryProcessor, mapper, cancellationToken, _, _, _, _) = requestParameters;

            var query = GetProducts.Of(
                new PageRequest
                {
                    PageNumber = requestParameters.PageNumber,
                    PageSize = requestParameters.PageSize,
                    SortOrder = requestParameters.SortOrder,
                    Filters = requestParameters.SortOrder
                }
            );

            var result = await queryProcessor.SendAsync(query, cancellationToken);

            return TypedResults.Ok(new GetProductsResponse(result.Products));
        }
    }
}

internal record GetProductsRequestParameters(
    HttpContext HttpContext,
    IQueryBus QueryBus,
    IMapper Mapper,
    CancellationToken CancellationToken,
    int PageSize = 10,
    int PageNumber = 1,
    string? Filters = null,
    string? SortOrder = null
) : IHttpQuery, IPageRequest;

internal record GetProductsResponse(IPageList<ProductDto> Products);
