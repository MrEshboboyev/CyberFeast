using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Abstractions.Web.Module;

/// <summary>
/// Defines methods for configuring a module within a web application.
/// </summary>
public interface IModuleConfiguration
{
    /// <summary>
    /// Adds services specific to the module.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The updated web application builder.</returns>
    WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder);

    /// <summary>
    /// Configures the module within the web application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<WebApplication> ConfigureModule(WebApplication app);

    /// <summary>
    /// Maps the module's endpoints to the endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}