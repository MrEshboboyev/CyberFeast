using AutoMapper;
using FoodDelivery.Services.Catalogs.Shared.Data;
using Tests.Shared.XunitCategories;

namespace FoodDelivery.Services.Catalogs.UnitTests.Common;

[CollectionDefinition(nameof(QueryTestCollection))]
public class QueryTestCollection : ICollectionFixture<CatalogsServiceUnitTestBase> { }

// note: each class could have only one collection
[Collection(UnitTestCollection.Name)]
[CategoryTrait(TestCategory.Unit)]
public class CatalogsServiceUnitTestBase : IAsyncDisposable
{
    public IMapper Mapper { get; } = MapperFactory.Create();
    public CatalogDbContext CatalogDbContext { get; } = DbContextFactory.Create();

    public async ValueTask DisposeAsync()
    {
        await DbContextFactory.Destroy(CatalogDbContext);
    }
}
