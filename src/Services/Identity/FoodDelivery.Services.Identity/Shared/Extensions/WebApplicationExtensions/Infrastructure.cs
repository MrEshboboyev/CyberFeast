using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Logging.Enrichers;
using BuildingBlocks.Messaging.Persistence.Postgres.Extensions;
using BuildingBlocks.Web.Extensions;
using BuildingBlocks.Web.Middlewares.CaptureException;
using BuildingBlocks.Web.Middlewares.HeaderPropagation;
using BuildingBlocks.Web.RateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace FoodDelivery.Services.Identity.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static async Task UseInfrastructure(this WebApplication app)
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions { AllowStatusCode404Response = true });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() || app.Environment.IsTest())
        {
            app.UseDeveloperExceptionPage();

            // .net 8 will add `IExceptionHandlerFeature`in `DisplayExceptionContent` and `SetExceptionHandlerFeatures` methods `DeveloperExceptionPageMiddlewareImpl` class, exact functionality of CaptureException
            // bet before .net 8 preview 5 we should add `IExceptionHandlerFeature` manually with our `UseCaptureException`
            app.UseCaptureException();
        }

        // this middleware should be first middleware
        // request logging just log in information level and above as default
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;

            // this level wil use for request logging
            opts.GetLevel = LogEnricher.GetLogLevel;
        });

        app.UseCustomCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHeaderPropagation();

        await app.UsePostgresPersistenceMessage(app.Logger);

        await app.MigrateDatabases();

        app.UseCustomRateLimit();

        if (app.Environment.IsTest() == false)
        {
            app.UseCustomHealthCheck();
            app.UseIdentityServer();
        }

        // Configure the prometheus endpoint for scraping metrics
        // NOTE: This should only be exposed on an internal port!
        // .RequireHost("*:9100");
        app.MapPrometheusScrapingEndpoint();
    }
}
