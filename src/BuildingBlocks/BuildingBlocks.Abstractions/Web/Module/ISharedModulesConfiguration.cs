using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Abstractions.Web.Module;

/// <summary>
/// Defines methods for configuring shared modules within a web application.
/// </summary>
public interface ISharedModulesConfiguration
{
    /// <summary>
    /// Adds services specific to the shared module.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The updated web application builder.</returns>
    WebApplicationBuilder AddSharedModuleServices(WebApplicationBuilder builder);

    /// <summary>
    /// Configures the shared module within the web application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<WebApplication> ConfigureSharedModule(WebApplication app);

    /// <summary>
    /// Maps the shared module's endpoints to the endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    IEndpointRouteBuilder MapSharedModuleEndpoints(IEndpointRouteBuilder endpoints);
}