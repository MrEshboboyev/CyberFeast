using BuildingBlocks.Abstractions.Web.Module;
using BuildingBlocks.Core.Web.Extensions;
using FoodDelivery.Services.Identity.Identity.Features.GettingClaims.v1;
using FoodDelivery.Services.Identity.Identity.Features.RefreshingToken.v1;
using FoodDelivery.Services.Identity.Identity.Features.RevokingRefreshToken.v1;
using FoodDelivery.Services.Identity.Shared;
using FoodDelivery.Services.Identity.Shared.Extensions.WebApplicationBuilderExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FoodDelivery.Services.Identity.Identity;

internal class IdentityConfigs : IModuleConfiguration
{
    public const string Tag = "Identity";
    public const string IdentityPrefixUri = $"{SharedModulesConfiguration.IdentityModulePrefixUri}";

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        builder.AddCustomIdentity(builder.Configuration);

        if (builder.Environment.IsTest() == false)
            builder.AddCustomIdentityServer();

        return builder;
    }

    public Task<WebApplication> ConfigureModule(WebApplication app)
    {
        return Task.FromResult(app);
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var identityVersionGroup = endpoints
            .NewVersionedApi(name: Tag)
            .WithTags(Tag);

        // create a new subgroup for v1 version
        var identityGroupV1 = identityVersionGroup.MapGroup(IdentityPrefixUri).HasApiVersion(1.0);

        // create a new subgroup for v2 version
        var identityGroupV2 = identityVersionGroup.MapGroup(IdentityPrefixUri).HasApiVersion(2.0);

        identityGroupV1
            .MapGet(
                "/user-role",
                [Authorize(
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = IdentityConstants.Role.User
                )]
                () => new { Role = IdentityConstants.Role.User }
            )
            .WithTags(Tag);

        identityGroupV1
            .MapGet(
                "/admin-role",
                [Authorize(
                    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                    Roles = IdentityConstants.Role.Admin
                )]
                () => new { Role = IdentityConstants.Role.Admin }
            )
            .WithTags(Tag);

        // TO-DO - add more endpoints
        identityGroupV1.MapRefreshTokenEndpoint();
        identityGroupV1.MapRevokeTokenEndpoint();
        identityGroupV1.MapGetClaimsEndpoint();

        return endpoints;
    }
}
