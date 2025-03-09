using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodDelivery.Services.Identity.Shared.Data;

/// <summary>
/// Executes database migrations for the IdentityContext.
/// Ensures schema updates and logs the migration process.
/// </summary>
public class IdentityMigrationExecutor(
    IdentityContext identityContext, 
    ILogger<IdentityMigrationExecutor> logger) : IMigrationExecutor
{
    /// <summary>
    /// Applies pending migrations asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Migration worker started");

        logger.LogInformation("Updating identity database...");

        await identityContext.Database.MigrateAsync(cancellationToken: cancellationToken);

        logger.LogInformation("identity database Updated");
    }
}
