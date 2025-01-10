using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Abstractions.Types;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Web.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Core.Messaging.BackgroundServices;

/// <summary>
/// A background service that handles the persistence of messages.
/// </summary>
public class MessagePersistenceBackgroundService(
    ILogger<MessagePersistenceBackgroundService> logger,
    IOptions<MessagePersistenceOptions> options,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime lifetime,
    IMachineInstanceInfo machineInstanceInfo
) : BackgroundService
{
    private readonly MessagePersistenceOptions _options = options.Value;

    /// <summary>
    /// Executes the background service's main logic.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await lifetime.WaitForAppStartup(stoppingToken))
        {
            return;
        }

        logger.LogInformation(
            "MessagePersistence Background Service is starting on client '{@ClientId}' and group '{@ClientGroup}'",
            machineInstanceInfo.ClientId,
            machineInstanceInfo.ClientGroup
        );

        await ProcessAsync(stoppingToken);
    }

    /// <summary>
    /// Performs any necessary cleanup when the background service is stopping.
    /// </summary>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "MessagePersistence Background Service is stopping on client '{@ClientId}' and group '{@ClientGroup}'",
            machineInstanceInfo.ClientId,
            machineInstanceInfo.ClientGroup
        );

        return base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Processes all messages periodically.
    /// </summary>
    private async Task ProcessAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IMessagePersistenceService>();
                await service.ProcessAllAsync(stoppingToken);
            }

            var delay = _options.Interval is not null
                ? TimeSpan.FromSeconds((int)_options.Interval)
                : TimeSpan.FromSeconds(30);

            await Task.Delay(delay, stoppingToken);
        }
    }
}