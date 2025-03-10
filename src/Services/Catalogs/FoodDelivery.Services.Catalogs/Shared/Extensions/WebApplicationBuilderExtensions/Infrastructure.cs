using BuildingBlocks.Caching.Behaviors;
using BuildingBlocks.Caching.Extensions;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.Core.Web.HeaderPropagation.Extensions;
using BuildingBlocks.Email;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Integration.MassTransit;
using BuildingBlocks.Logging;
using BuildingBlocks.Logging.Extensions;
using BuildingBlocks.Messaging.Persistence.Postgres.Extensions;
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.Persistence.EfCore.Postgres;
using BuildingBlocks.Security.Jwt;
using BuildingBlocks.Swagger;
using BuildingBlocks.Validation;
using BuildingBlocks.Validation.Extensions;
using BuildingBlocks.Web.Extensions;
using BuildingBlocks.Web.RateLimit;
using BuildingBlocks.Web.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BuildingBlocks.Core.Persistence.EFCore;
using FoodDelivery.Services.Catalogs.Products;
using RabbitMQ.Client;
using MessageHeaders = BuildingBlocks.Core.Messaging.MessageHeaders;

namespace FoodDelivery.Services.Catalogs.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddCore();

        builder.Services.AddCustomJwtAuthentication(builder.Configuration);
        builder.Services.AddCustomAuthorization(
            rolePolicies: new List<RolePolicy>
            {
                new(CatalogConstants.Role.Admin, new List<string> { CatalogConstants.Role.Admin }),
                new(CatalogConstants.Role.User, new List<string> { CatalogConstants.Role.User })
            }
        );

        builder.Services.AddEmailService(builder.Configuration);

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenStreamBehavior(typeof(StreamLoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            cfg.AddOpenStreamBehavior(typeof(StreamRequestValidationBehavior<,>));
            cfg.AddOpenStreamBehavior(typeof(StreamCachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(InvalidateCachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(EfTransactionBehavior<,>));
        });

        DotNetEnv.Env.TraversePath().Load();

        // if we need to reload app by change settings, we can use volume map for watching our new setting from config-files folder in the the volume or change appsettings.json file in the volume map
        var configFolder = builder.Configuration.GetValue<string>("ConfigurationFolder") ?? "config-files/";
        builder.Configuration.AddKeyPerFile(configFolder, true, true);
        
        builder.Configuration.AddEnvironmentVariables("food_delivery_catalogs_env_");

        builder.AddCustomVersioning();

        builder.AddCustomSwagger(cfg =>
        {
            cfg.Name = "Catalogs Apis";
            cfg.Title = "Catalogs Apis";
        });

        builder.AddCustomCors();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddPostgresMessagePersistence(builder.Configuration);

        builder.AddCompression();

        builder.AddAppProblemDetails();

        builder.AddCustomSerilog();

        builder.AddCustomOpenTelemetry();

        builder.Services.AddHeaderPropagation(options =>
        {
            options.HeaderNames.Add(MessageHeaders.CorrelationId);
            options.HeaderNames.Add(MessageHeaders.CausationId);
        });

        builder.AddCustomRateLimit();

        builder.AddCustomMassTransit(
            (busRegistrationContext, busFactoryConfigurator) =>
            {
                busFactoryConfigurator.ConfigureProductMessagesTopology();
            }
        );

        if (builder.Environment.IsTest() == false)
        {
            builder.AddCustomHealthCheck(healthChecksBuilder =>
            {
                var postgresOptions = builder.Configuration.BindOptions<PostgresOptions>(nameof(PostgresOptions));
                var rabbitMqOptions = builder.Configuration.BindOptions<RabbitMqOptions>(nameof(RabbitMqOptions));

                postgresOptions.NotBeNull();
                rabbitMqOptions.NotBeNull();

                healthChecksBuilder
                    .AddNpgSql(
                        postgresOptions.ConnectionString!,
                        name: "CatalogsService-Postgres-Check",
                        tags: ["postgres", "infra", "database", "catalogs-service", "live", "ready",]
                    )
                    .AddRabbitMQ(
                        _ => new ConnectionFactory()
                        {
                            Uri = new Uri(rabbitMqOptions.ConnectionString)
                        }.CreateConnectionAsync(),
                        name: "CatalogsService-RabbitMQ-Check",
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["rabbitmq", "infra", "bus", "catalogs-service", "live", "ready"]
                    );
            });
        }

        builder.Services.AddCustomValidators(Assembly.GetExecutingAssembly());
        builder.Services.AddAutoMapper(x =>
        {
            x.AddProfile<ProductMappers>();
        });

        builder.AddCustomEasyCaching();

        return builder;
    }
}
