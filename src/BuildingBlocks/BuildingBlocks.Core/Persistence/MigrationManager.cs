using BuildingBlocks.Abstractions.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Persistence;

/// <summary>
/// Manages the execution of database migrations before running background services.
/// </summary>
/// <param name="serviceScopeFactory">The service scope factory.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="environment">The web host environment.</param>
public class MigrationManager(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<MigrationManager> logger,
    IWebHostEnvironment environment) : IMigrationManager
{
    private readonly IWebHostEnvironment _environment = environment;

    /// <summary>
    /// Executes all registered migrations sequentially.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        var migrations = serviceScope.ServiceProvider.GetServices<IMigrationExecutor>();

        foreach (var migration in migrations)
        {
            logger.LogInformation("Migration '{Migration}' started...", migrations.GetType().Name);
            await migration.ExecuteAsync(stoppingToken);
            logger.LogInformation("Migration '{Migration}' ended...", migration.GetType().Name);
        }
    }
}