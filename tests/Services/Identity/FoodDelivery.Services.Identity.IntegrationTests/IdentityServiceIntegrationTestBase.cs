using FoodDelivery.Services.Identity.Api;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Tests.Shared.Fixtures;
using Tests.Shared.TestBase;
using Xunit.Abstractions;

namespace FoodDelivery.Services.Identity.IntegrationTests;

// note: each class could have only one collection
[Collection(IntegrationTestCollection.Name)]
public class IdentityServiceIntegrationTestBase(
    SharedFixtureWithEfCore<IdentityApiMetadata, IdentityDbContext> sharedFixture,
    ITestOutputHelper outputHelper
) : IntegrationTestBase<IdentityApiMetadata, IdentityDbContext>(sharedFixture, outputHelper)
{
    // We don't need to inject `CustomersServiceMockServersFixture` class fixture in the constructor because it initialized by `collection fixture` and its static properties are accessible in the codes
}
