using FoodDelivery.Services.Customers.Shared.Data;
using Tests.Shared.Fixtures;

namespace FoodDelivery.Services.Customers.IntegrationTests;

// note: each class could have only one collection, but it can implement multiple ICollectionFixture
// in its definitions
[CollectionDefinition(Name)]
public class IntegrationTestCollection
    : ICollectionFixture<
        SharedFixtureWithEfCoreAndMongo<Api.CustomersApiMetadata, CustomersDbContext, CustomersReadDbContext>
    >
{
    public const string Name = "Integration Test";
}
