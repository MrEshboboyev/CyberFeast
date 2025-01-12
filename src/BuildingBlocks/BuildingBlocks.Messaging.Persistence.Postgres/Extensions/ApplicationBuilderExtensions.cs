using BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Persistence.Postgres.Extensions;

/// <summary>
/// Provides extension methods for applying database migrations for message persistence.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Applies database migrations for message persistence at runtime.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="logger">The logger for logging migration details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task UsePostgresPersistenceMessage(
        this IApplicationBuilder app,
        ILogger logger)
    {
        await ApplyDatabaseMigrations(app, logger);
    }

    /// <summary>
    /// Applies the database migrations.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="logger">The logger for logging migration details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task ApplyDatabaseMigrations(
        this IApplicationBuilder app,
        ILogger logger)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var messagePersistenceDbContext =
            serviceScope.ServiceProvider.GetRequiredService<MessagePersistenceDbContext>();

        logger.LogInformation("Applying persistence-message migrations...");

        await messagePersistenceDbContext.Database.MigrateAsync();

        logger.LogInformation("Persistence-message migrations applied");
    }
}