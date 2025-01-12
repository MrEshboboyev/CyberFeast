using System.Linq;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
/// Provides extension methods for configuring CORS in an ASP.NET Core application.
/// </summary>
public static class CorsExtensions
{
    private const string CorsPolicyName = "AllowSpecificOrigins";

    /// <summary>
    /// Adds CORS services to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCustomCors(this WebApplicationBuilder builder)
    {
        // Add validated CORS options
        builder.Services.AddValidatedOptions<CorsOptions>();

        // Add CORS services
        builder.Services.AddCors();

        return builder;
    }

    /// <summary>
    /// Configures CORS in the web application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The updated web application.</returns>
    public static WebApplication UseCustomCors(this WebApplication app)
    {
        var options = app.Services.GetService<CorsOptions>();

        // Configure CORS policy based on allowed URLs
        app.UseCors(policy =>
        {
            if (options?.AllowedUrls is not null && options.AllowedUrls.Any())
            {
                policy.WithOrigins(options.AllowedUrls.ToArray());
            }
            else
            {
                policy.AllowAnyOrigin();
            }

            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });

        return app;
    }
}