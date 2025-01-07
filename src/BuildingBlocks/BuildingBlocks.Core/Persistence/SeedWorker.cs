using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Persistence;

/// <summary>
/// Handles the execution of data seeding operations.
/// </summary>
/// <param name="serviceScopeFactory">The service scope factory.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="webHostEnvironment">The web host environment.</param>
public class SeedWorker(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SeedWorker> logger,
    IWebHostEnvironment webHostEnvironment) : BackgroundService
{
    /// <summary>
    /// Executes all registered data seeders sequentially.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Seed worker started");

        using var serviceScope = serviceScopeFactory.CreateScope();

        var testSeeders = serviceScope.ServiceProvider.GetServices<ITestDataSeeder>();
        var seeders = serviceScope.ServiceProvider.GetServices<IDataSeeder>();
        if (webHostEnvironment.IsTest())
        {
            foreach (var testDataSeeder in testSeeders.OrderBy(x => x.Order))
            {
                logger.LogInformation("Seeding '{Seed}' started...", testDataSeeder.GetType().Name);
                await testDataSeeder.SeedAllAsync();
                logger.LogInformation("Seeding '{Seed}' ended...", testDataSeeder.GetType().Name);
            }
        }
        else
        {
            foreach (var seeder in seeders.OrderBy(x => x.Order))
            {
                logger.LogInformation("Seeding '{Seed}' started...", seeder.GetType().Name);
                await seeder.SeedAllAsync();
                logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);
            }
        }
    }

    /// <summary>
    /// Logs information when the seed worker stops.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Seed worker stopped");

        return base.StopAsync(cancellationToken);
    }
}