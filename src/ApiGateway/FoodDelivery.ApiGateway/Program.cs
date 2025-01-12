using System.IdentityModel.Tokens.Jwt;
using BuildingBlocks.Logging.Enrichers;
using MassTransit;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Spectre;
using Yarp.ReverseProxy.Transforms;
using MessageHeaders = BuildingBlocks.Core.Messaging.MessageHeaders;

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Spectre("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", LogEventLevel.Information)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Configure YARP reverse proxy with request transforms
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("yarp"))
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(transform =>
        {
            // Add correlation ID to the request headers
            transform.ProxyRequest.Headers.Add(
                MessageHeaders.CorrelationId,
                NewId.NextGuid().ToString());

            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

// Configure Serilog request logging with custom enrichment
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
    opts.GetLevel = LogEnricher.GetLogLevel;
});

// Map root endpoint
app.MapGet("/", async (HttpContext context) =>
{
    await context.Response.WriteAsync("FoodDelivery Api Gateway");
});

// Map reverse proxy
app.MapReverseProxy();

await app.RunAsync();