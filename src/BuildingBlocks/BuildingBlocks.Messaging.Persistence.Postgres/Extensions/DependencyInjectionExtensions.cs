using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Messaging.Persistence.Postgres.Extensions;

/// <summary>
/// Provides extension methods for configuring and adding message persistence services for PostgreSQL.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds message persistence services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <param name="configurator">Optional action to configure message persistence options.</param>
    public static void AddPostgresMessagePersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MessagePersistenceOptions>? configurator = null)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var options = configuration.BindOptions<MessagePersistenceOptions>(configurator);

        // Add options to the dependency injection container
        services.AddValidationOptions(configurator);

        // Add the message persistence connection factory
        services.TryAddScoped<IMessagePersistenceConnectionFactory>(
            _ => new NpgsqlMessagePersistenceConnectionFactory(
                options.ConnectionString.NotBeEmptyOrNull()
            ));

        // Configure and add the message persistence DbContext
        services.AddDbContext<MessagePersistenceDbContext>(
            (_, opt) =>
            {
                opt.UseNpgsql(
                        options.ConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(
                                options.MigrationAssembly
                                ?? typeof(MessagePersistenceDbContext)
                                    .Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(
                                5,
                                TimeSpan.FromSeconds(10),
                                null);
                        })
                    .UseSnakeCaseNamingConvention();
            });

        // Replace the message persistence repository with a PostgreSQL implementation
        services.Replace(
            ServiceDescriptor.Scoped<IMessagePersistenceRepository, PostgresMessagePersistenceRepository>()
        );
    }
}