using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Problem.HttpResults;
using FoodDelivery.Services.Catalogs.Products.Models;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Catalogs.Products.Features.UpdatingProduct.v1;

// PUT api/v1/catalog/products/{id}
public static class UpdateProductEndpoint
{
    internal static RouteHandlerBuilder MapUpdateProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // return endpoints.MapCommandEndpoint<UpdateProductRequest, UpdateProduct>("/");
        return endpoints
            .MapPost("/{id}", Handle)
            .WithTags(ProductsConfigs.Tag)
            .RequireAuthorization()
            .WithName(nameof(UpdateProduct))
            .WithDisplayName(nameof(UpdateProduct).Humanize())
            .WithSummaryAndDescription(nameof(UpdateProduct).Humanize(), nameof(UpdateProduct).Humanize())
            // .Produces("Product updated successfully.", StatusCodes.Status204NoContent)
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            // .ProducesProblem("UnAuthorized request.", StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0);

        async Task<Results<NoContent, UnAuthorizedHttpProblemResult, ValidationProblem>> Handle(
            [AsParameters] UpdateProductRequestParameters requestParameters
        )
        {
            var (request, id, context, commandBus, mapper, cancellationToken) = requestParameters;

            var command = mapper.Map<UpdateProduct>(request);
            command = command with { Id = id };

            await commandBus.SendAsync(command, cancellationToken);

            return TypedResults.NoContent();
        }
    }
}

internal record UpdateProductRequestParameters(
    [FromBody] UpdateProductRequest Request,
    [FromRoute] long Id,
    HttpContext HttpContext,
    ICommandBus CommandBus,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<UpdateProductRequest>;

// parameters can be pass as null from the user
public record UpdateProductRequest(
    long Id,
    string? Name,
    decimal Price,
    int RestockThreshold,
    int MaxStockThreshold,
    ProductStatus Status,
    ProductType ProductType,
    ProductColor ProductColor,
    int Height,
    int Width,
    int Depth,
    string? Size,
    long CategoryId,
    long SupplierId,
    long BrandId,
    string? Description
);
