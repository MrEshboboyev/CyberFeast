using BuildingBlocks.Core.Extensions.ServiceCollection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace BuildingBlocks.Swagger;

/// <summary>
/// Provides extension methods for registering and configuring Swagger services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds custom Swagger services to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configurator">Optional action to configure Swagger options.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCustomSwagger(
        this WebApplicationBuilder builder,
        Action<SwaggerOptions>? configurator = null
    )
    {
        builder.Services.AddCustomSwagger(configurator);

        return builder;
    }

    /// <summary>
    /// Adds custom Swagger services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurator">Optional action to configure Swagger options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCustomSwagger(
        this IServiceCollection services,
        Action<SwaggerOptions>? configurator = null
    )
    {
        services.AddEndpointsApiExplorer();

        services.TryAddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddValidatedOptions(configurator: configurator);

        services.AddSwaggerGen(options =>
        {
            // Add enums with values fix filters
            options.AddEnumsWithValuesFixFilters();

            // Add custom operation filters
            options.OperationFilter<SwaggerDefaultValues>();
            options.OperationFilter<ApiVersionOperationFilter>();

            // Configure security schemes
            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Name = JwtBearerDefaults.AuthenticationScheme,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            var apiKeyScheme = new OpenApiSecurityScheme
            {
                Description = "Api key needed to access the endpoints. X-Api-Key: My_API_Key",
                In = ParameterLocation.Header,
                Name = Constants.ApiKeyConstants.HeaderName,
                Scheme = Constants.ApiKeyConstants.DefaultScheme,
                Type = SecuritySchemeType.ApiKey,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = Constants.ApiKeyConstants.HeaderName
                }
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);
            options.AddSecurityDefinition(Constants.ApiKeyConstants.HeaderName, apiKeyScheme);

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        bearerScheme,
                        Array.Empty<string>()
                    },
                    {
                        apiKeyScheme,
                        Array.Empty<string>()
                    }
                }
            );

            // Resolve conflicting actions
            options.ResolveConflictingActions(
                apiDescriptions => apiDescriptions.First());

            // Enable annotations
            options.EnableAnnotations();
        });

        return services;
    }

    /// <summary>
    /// Uses custom Swagger configuration in the web application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();

            // Build a Swagger endpoint for each discovered API version.
            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });

        return app;
    }
}