using BuildingBlocks.Abstractions.Persistence;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Catalogs.Brands.Data;

public class BrandDataSeeder(ICatalogDbContext context) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (await context.Brands.AnyAsync())
            return;

        // faker works with normal syntax because brand has a default constructor
        var brands = new BrandFaker().Generate(5);

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();
    }

    public int Order => 3;
}

// because AutoFaker generate data also for private set and init members (not read only get) it
// doesn't work properly with `CustomInstantiator` and we should exclude theme one by one