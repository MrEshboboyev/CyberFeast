using BuildingBlocks.Caching.Behaviors;
using BuildingBlocks.Caching.Extensions;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Persistence.EFCore;
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
using FoodDelivery.Services.Identity.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Reflection;

namespace FoodDelivery.Services.Identity.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddCore();

        builder.Services.AddCustomJwtAuthentication(builder.Configuration);
        builder.Services.AddCustomAuthorization(
            rolePolicies: new List<RolePolicy>
            {
                new(IdentityConstants.Role.Admin, new List<string>
                {
                    IdentityConstants.Role.Admin
                }),
                new(IdentityConstants.Role.User, new List<string>
                {
                    IdentityConstants.Role.User
                })
            }
        );
    
        builder.Configuration.AddEnvironmentVariables("food_delivery_identity_env_");

        DotNetEnv.Env.TraversePath().Load();

        builder.AddCompression();

        builder.AddAppProblemDetails();

        builder.AddCustomSerilog();

        builder.AddCustomVersioning();

        builder.AddCustomSwagger(cfg =>
        {
            cfg.Name = "Identity Apis";
            cfg.Title = "Identity Apis";
        });

        builder.AddCustomCors();

        builder.AddCustomOpenTelemetry();

        builder.Services.AddHeaderPropagation(options =>
        {
            options.HeaderNames.Add(MessageHeaders.CorrelationId);
            options.HeaderNames.Add(MessageHeaders.CausationId);
        });

        builder.Services.AddHttpContextAccessor();

        if (builder.Environment.IsTest() == false)
        {
            builder.AddCustomHealthCheck(healthChecksBuilder =>
            {
                var postgresOptions = builder.Configuration.BindOptions<PostgresOptions>();
                var rabbitMqOptions = builder.Configuration.BindOptions<RabbitMqOptions>();

                healthChecksBuilder
                    .AddNpgSql(
                        postgresOptions.ConnectionString!,
                        name: "IdentityService-Postgres-Check",
                        tags: ["postgres", "database", "infra", "identity-service", "live", "ready",]
                    )
                    .AddRabbitMQ(
                        _ => new ConnectionFactory
                        {
                            Uri = new Uri(rabbitMqOptions.ConnectionString)
                        }.CreateConnectionAsync(),
                        name: "IdentityService-RabbitMQ-Check",
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["rabbitmq", "bus", "infra", "identity-service", "live", "ready"]
                    );
            });
        }

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

        builder.Services.AddPostgresMessagePersistence(builder.Configuration);

        builder.AddCustomRateLimit();

        builder.AddCustomRateLimit();

        builder.AddCustomMassTransit(
            configureMessagesTopologies: (context, cfg) =>
            {
                cfg.ConfigureUserMessagesTopology();
            },
            configureMessagingOptions: msgCfg =>
            {
                msgCfg.AutoConfigEndpoints = false;
                msgCfg.OutboxEnabled = true;
                msgCfg.InboxEnabled = true;
            }
        );

        builder.AddCustomEasyCaching();

        builder.Services.AddCustomValidators(Assembly.GetExecutingAssembly());

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return builder;
    }
}
