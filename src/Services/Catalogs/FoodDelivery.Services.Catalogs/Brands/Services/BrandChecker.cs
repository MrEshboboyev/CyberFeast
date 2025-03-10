using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Catalogs.Brands.Contracts;
using FoodDelivery.Services.Catalogs.Brands.ValueObjects;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using FoodDelivery.Services.Catalogs.Shared.Extensions;

namespace FoodDelivery.Services.Catalogs.Brands.Services;

public class BrandChecker(ICatalogDbContext catalogDbContext) : IBrandChecker
{
    public bool BrandExists(BrandId? brandId)
    {
        brandId.NotBeNull();
        var brand = catalogDbContext.FindBrand(brandId);

        return brand is not null;
    }
}