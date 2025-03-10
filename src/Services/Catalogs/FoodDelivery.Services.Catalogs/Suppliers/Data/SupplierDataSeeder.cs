using BuildingBlocks.Abstractions.Persistence;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Catalogs.Suppliers.Data;

public class SupplierDataSeeder(ICatalogDbContext dbContext) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (await dbContext.Suppliers.AnyAsync())
            return;

        var suppliers = new SupplierFaker().Generate(5);
        await dbContext.Suppliers.AddRangeAsync(suppliers);

        await dbContext.SaveChangesAsync();
    }

    public int Order => 2;
}

// because AutoFaker generate data also for private set and init members (not read only get) it
// doesn't work properly with CustomInstantiator