using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Catalogs.Products.Features.GettingProductById.v1;
using FoodDelivery.Services.Catalogs.Products.Models;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Catalogs.Products.Features.CreatingProduct.v1;

// POST api/v1/catalog/products
internal static class CreateProductEndpoint
{
    internal static RouteHandlerBuilder MapCreateProductsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // return endpoints.MapCommandEndpoint<
        //     CreateProductRequest,
        //     CreateProductResponse,
        //     CreateProduct,
        //     CreateProductResult
        // >("/", StatusCodes.Status201Created, getId: response => response.Id);

        return endpoints
            .MapPost("/", Handle)
            .WithTags(ProductsConfigs.Tag)
            .RequireAuthorization()
            .WithName(nameof(CreateProduct))
            .WithDisplayName(nameof(CreateProduct).Humanize())
            .WithSummaryAndDescription(nameof(CreateProduct).Humanize(), nameof(CreateProduct).Humanize())
            // .Produces<CreateProductResponse>("Product created successfully.", StatusCodes.Status201Created)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            // .ProducesProblem("UnAuthorized request.", StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0);

        async Task<
            Results<CreatedAtRoute<CreateProductResponse>, UnAuthorizedHttpProblemResult, ValidationProblem>
        > Handle([AsParameters] CreateProductRequestParameters requestParameters)
        {
            var (request, context, commandBus, mapper, cancellationToken) = requestParameters;

            var command = mapper.Map<CreateProduct>(request);

            var result = await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.CreatedAtRoute(
                new CreateProductResponse(result.Id),
                nameof(GetProductById),
                new { id = result.Id }
            );
        }
    }
}

internal record CreateProductRequestParameters(
    [FromBody] CreateProductRequest Request,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<CreateProductRequest>;

internal record CreateProductResponse(long Id);

// parameters can be pass as null from the user
internal record CreateProductRequest(
    string? Name,
    decimal Price,
    int Stock,
    int RestockThreshold,
    int MaxStockThreshold,
    int Height,
    int Width,
    int Depth,
    string? Size,
    long CategoryId,
    long SupplierId,
    long BrandId,
    string? Description = null,
    ProductColor Color = ProductColor.Black,
    ProductStatus Status = ProductStatus.Available,
    ProductType ProductType = ProductType.Food,
    IEnumerable<CreateProductImageRequest>? Images = null
);

internal record CreateProductImageRequest(string ImageUrl, bool IsMain);
