using BuildingBlocks.Core.Persistence.EFCore;
using BuildingBlocks.Core.Persistence.EFCore.Interceptors;
using FoodDelivery.Services.Catalogs.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FoodDelivery.Services.Catalogs.UnitTests.Common;

public static class DbContextFactory
{
    public static CatalogDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<long>>()
            .AddInterceptors(
                new AuditInterceptor(), 
                new SoftDeleteInterceptor(), 
                new ConcurrencyInterceptor())
            .Options;

        var context = new CatalogDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public static async Task Destroy(CatalogDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }
}
