using BuildingBlocks.Abstractions.Web.Module;
using FoodDelivery.Services.Identity.Shared;
using FoodDelivery.Services.Identity.Users.Features.GettingUserByEmail.v1;
using FoodDelivery.Services.Identity.Users.Features.GettingUserById.v1;
using FoodDelivery.Services.Identity.Users.Features.GettingUsers.v1;
using FoodDelivery.Services.Identity.Users.Features.RegisteringUser.v1;
using FoodDelivery.Services.Identity.Users.Features.UpdatingUserState.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Users;

internal class UsersConfigs : IModuleConfiguration
{
    public const string Tag = "Users";
    public const string UsersPrefixUri = $"{SharedModulesConfiguration.IdentityModulePrefixUri}/users";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        return builder;
    }

    public Task<WebApplication> ConfigureModule(WebApplication app)
    {
        return Task.FromResult(app);
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // changed from MapApiGroup to NewVersionedApi in v7.0.0
        var usersVersionGroup = endpoints.NewVersionedApi(Tag).WithTags(Tag);

        // create a new subgroup for each version
        var usersGroupV1 = usersVersionGroup.MapGroup(UsersPrefixUri).HasApiVersion(1.0);

        // create a new subgroup for each version
        var usersGroupV2 = usersVersionGroup.MapGroup(UsersPrefixUri).HasApiVersion(2.0);

        usersGroupV1.MapRegisterNewUserEndpoint();
        usersGroupV1.MapUpdateUserStateEndpoint();
        usersGroupV1.MapGetUserByIdEndpoint();
        usersGroupV1.MapGetUserByEmailEndpoint();
        usersGroupV1.MapGetUsersByPageEndpoint();

        return endpoints;
    }
}
