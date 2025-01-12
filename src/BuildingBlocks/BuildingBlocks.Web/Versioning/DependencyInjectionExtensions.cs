using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Versioning;

/// <summary>
/// Provides methods to add custom API versioning support to an ASP.NET Core application.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds custom API versioning services to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configurator">Optional action to configure API versioning options.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCustomVersioning(
        this WebApplicationBuilder builder,
        Action<ApiVersioningOptions>? configurator = null)
    {
        // Add API versioning support
        builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true; // Add version headers
                options.AssumeDefaultVersionWhenUnspecified = true; // Assume default version if not specified
                options.DefaultApiVersion = new ApiVersion(1, 0); // Set default API version
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new QueryStringApiVersionReader(),
                    new UrlSegmentApiVersionReader());

                configurator?.Invoke(options);
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // Format API version group names
                options.SubstituteApiVersionInUrl = true; // Substitute API version in URL segments
            })
            .AddMvc(); // Add MVC support

        return builder;
    }
}