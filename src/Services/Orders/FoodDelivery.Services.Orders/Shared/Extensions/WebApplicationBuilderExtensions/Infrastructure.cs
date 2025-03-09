using BuildingBlocks.Caching.Behaviors;
using BuildingBlocks.Caching.Extensions;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;
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
using BuildingBlocks.Web.Versioning;
using FoodDelivery.Services.Orders.Customers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.RateLimiting;
using BuildingBlocks.Core.Persistence.EFCore;
using RabbitMQ.Client;

namespace FoodDelivery.Services.Orders.Shared.Extensions.WebApplicationBuilderExtensions;

internal static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddCore();

        builder.Services.AddCustomJwtAuthentication(builder.Configuration);
        builder.Services.AddCustomAuthorization(
            rolePolicies: new List<RolePolicy>
            {
                new(OrdersConstants.Role.Admin, new List<string> { OrdersConstants.Role.Admin }),
                new(OrdersConstants.Role.User, new List<string> { OrdersConstants.Role.User })
            }
        );

        builder.Configuration.AddEnvironmentVariables("food_delivery_orders_env_");

        DotNetEnv.Env.TraversePath().Load();

        builder.AddCompression();

        builder.AddAppProblemDetails();

        builder.AddCustomSerilog();

        builder.AddCustomVersioning();

        builder.AddCustomSwagger(cfg =>
        {
            cfg.Name = "Orders Apis";
            cfg.Title = "Orders Apis";
        });

        builder.AddCustomCors();

        builder.Services.AddHttpContextAccessor();

        builder.AddCustomOpenTelemetry();

        builder.Services.AddHeaderPropagation(options =>
        {
            options.HeaderNames.Add(MessageHeaders.CorrelationId);
            options.HeaderNames.Add(MessageHeaders.CausationId);
        });

        if (builder.Environment.IsTest() == false)
        {
            builder.AddCustomHealthCheck(healthChecksBuilder =>
            {
                var postgresOptions = builder.Configuration.BindOptions<PostgresOptions>();
                var rabbitMqOptions = builder.Configuration.BindOptions<RabbitMqOptions>();

                postgresOptions.NotBeNull();
                rabbitMqOptions.NotBeNull();

                healthChecksBuilder
                    .AddNpgSql(
                        postgresOptions.ConnectionString,
                        name: "OrdersService-Postgres-Check",
                        tags: ["postgres", "database", "infra", "orders-service", "live", "ready",]
                    )
                    .AddRabbitMQ(
                        _ => new ConnectionFactory()
                        {
                            Uri = new Uri(rabbitMqOptions.ConnectionString)
                        }.CreateConnectionAsync(),
                        name: "OrdersService-RabbitMQ-Check",
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["rabbitmq", "bus", "infra", "orders-service", "live", "ready"]
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

        builder.Services.AddRateLimiter(options =>
        {
            // rate limiter that limits all to 10 requests per minute, per authenticated username (or hostname if not authenticated)
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 10,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }
                )
            );
        });

        builder.AddCustomMassTransit(
            configureMessagesTopologies: (context, cfg) =>
            {
                cfg.AddCustomerEndpoints(context);
            },
            configureMessagingOptions: msgCfg =>
            {
                msgCfg.AutoConfigEndpoints = false;
                msgCfg.OutboxEnabled = true;
                msgCfg.InboxEnabled = true;
            }
        );

        builder.Services.AddCustomValidators(Assembly.GetExecutingAssembly());

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.AddCustomEasyCaching();

        return builder;
    }
}
