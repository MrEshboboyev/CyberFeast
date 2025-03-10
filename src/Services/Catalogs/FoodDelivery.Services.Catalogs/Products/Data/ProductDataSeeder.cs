using BuildingBlocks.Abstractions.Persistence;
using FoodDelivery.Services.Catalogs.Brands;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Catalogs.Products.Data;

public class ProductDataSeeder(ICatalogDbContext dbContext) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (await dbContext.Products.AnyAsync())
            return;

        var products = new ProductFaker().Generate(5);

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }

    public int Order => 4;
}

// because AutoFaker generate data also for private set and init members (not read only get)
// it doesn't work properly with `CustomInstantiator` and we should exclude theme one by one
