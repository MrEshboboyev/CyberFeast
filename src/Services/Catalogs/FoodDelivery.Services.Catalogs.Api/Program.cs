using Bogus;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Web;
using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web.Minimal.Extensions;
using BuildingBlocks.Web.Modules;
using FoodDelivery.Services.Catalogs;
using Microsoft.AspNetCore.Mvc;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Catalogs Service").Centered().Color(Color.FromInt32(new Faker().Random.Int(1, 255))));

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(
    (context, options) =>
    {
        var isDevMode =
            context.HostingEnvironment.IsDevelopment()
            || context.HostingEnvironment.IsTest()
            || context.HostingEnvironment.IsStaging();

        // Handling Captive Dependency Problem
        // CreateDefaultBuilder and WebApplicationBuilder in minimal apis sets `ServiceProviderOptions.ValidateScopes` and `ServiceProviderOptions.ValidateOnBuild` to true if the app's environment is Development.
        // check dependencies are used in a valid lifetime scope
        options.ValidateScopes = isDevMode;

        // validate dependencies on the startup immediately instead of waiting for using the service - Issue with masstransit #85
        // options.ValidateOnBuild = isDevMode;
    }
);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddValidatedOptions<AppOptions>();

// register endpoints
builder.AddMinimalEndpoints(typeof(CatalogsMetadata).Assembly);

/*----------------- Module Services Setup ------------------*/
builder.AddModulesServices();

var app = builder.Build();

if (app.Environment.IsDependencyTest())
{
    return;
}

/*----------------- Module Middleware Setup ------------------*/
await app.ConfigureModules();

// in .net 6 and above we don't need UseRouting and UseEndpoints but if ordering is important we should write it
// app.UseRouting();

/*----------------- Module Routes Setup ------------------*/
app.MapModulesEndpoints();

// automatic discover minimal endpoints
app.MapMinimalEndpoints();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("docker"))
{
    // should register as last middleware for discovering all endpoints and its versions correctly
    app.UseCustomSwagger();
}

await app.RunAsync();
