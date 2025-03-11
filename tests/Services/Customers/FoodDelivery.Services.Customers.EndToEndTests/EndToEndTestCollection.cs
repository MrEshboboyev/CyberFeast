using FoodDelivery.Services.Customers.Shared.Data;
using Tests.Shared.Fixtures;

namespace FoodDelivery.Services.Customers.EndToEndTests;

// note: each class could have only one collection, but it can implements multiple ICollectionFixture in its definitions
[CollectionDefinition(Name)]
public class EndToEndTestCollection
    : ICollectionFixture<
        SharedFixtureWithEfCoreAndMongo<Api.CustomersApiMetadata, CustomersDbContext, CustomersReadDbContext>
    >
{
    public const string Name = "End-To-End Test";
}