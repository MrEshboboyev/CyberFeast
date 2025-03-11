using FoodDelivery.Services.Identity.Api;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Tests.Shared.Fixtures;

namespace FoodDelivery.Services.Identity.IntegrationTests;

// note: each class could have only one collection, but it can implement multiple ICollectionFixture in its definitions
[CollectionDefinition(Name)]
public class IntegrationTestCollection
    : ICollectionFixture<SharedFixtureWithEfCore<IdentityApiMetadata, IdentityDbContext>>
{
    public const string Name = "Integration Test";
}
