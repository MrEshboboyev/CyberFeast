using BuildingBlocks.Abstractions.Persistence;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Catalogs.Categories.Data;

public class CategoryDataSeeder(ICatalogDbContext dbContext) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (await dbContext.Categories.AnyAsync())
            return;

        // faker works with normal syntax because category has a default constructor
        var categories = new CategoryFaker().Generate(5);

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();
    }

    public int Order => 1;
}
