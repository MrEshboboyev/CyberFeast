using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Web.Module;
using FoodDelivery.Services.Catalogs.Products.Features.CreatingProduct.v1;
using FoodDelivery.Services.Catalogs.Products.Features.DebitingProductStock.v1;
using FoodDelivery.Services.Catalogs.Products.Features.GettingProductById.v1;
using FoodDelivery.Services.Catalogs.Products.Features.GettingProductsView.v1;
using FoodDelivery.Services.Catalogs.Products.Features.ReplenishingProductStock.v1;
using FoodDelivery.Services.Catalogs.Products.Features.UpdatingProduct.v1;
using FoodDelivery.Services.Catalogs.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FoodDelivery.Services.Catalogs.Products;

internal class ProductsConfigs : IModuleConfiguration
{
    public const string Tag = "Products";
    public const string ProductsPrefixUri = $"{SharedModulesConfiguration.CatalogModulePrefixUri}/products";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<IEventMapper, ProductEventMapper>();

        return builder;
    }

    public Task<WebApplication> ConfigureModule(WebApplication app)
    {
        return Task.FromResult(app);
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // changed from MapApiGroup to NewVersionedApi in v7.0.0
        var products = endpoints.NewVersionedApi(name: Tag).WithTags(Tag);

        // create a new subgroup for each version
        var productsV1 = products.MapGroup(ProductsPrefixUri).HasDeprecatedApiVersion(0.9).HasApiVersion(1.0);

        // create a new subgroup for each version
        var productsV2 = products.MapGroup(ProductsPrefixUri).HasApiVersion(2.0);

        productsV1.MapCreateProductsEndpoint();
        productsV1.MapUpdateProductEndpoint();
        productsV1.MapDebitProductStockEndpoint();
        productsV1.MapReplenishProductStockEndpoint();
        productsV1.MapGetProductByIdEndpoint();
        productsV1.MapGetProductsViewEndpoint();

        return endpoints;
    }
}
