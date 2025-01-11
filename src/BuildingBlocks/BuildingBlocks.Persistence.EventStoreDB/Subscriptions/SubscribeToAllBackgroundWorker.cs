using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EventStore.Projections;
using BuildingBlocks.Core.Threading;
using BuildingBlocks.Core.Types;
using BuildingBlocks.Persistence.EventStoreDB.Extensions;
using EventStore.Client;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Persistence.EventStoreDB.Subscriptions;

/// <summary>
/// A background service that handles subscriptions to all events in EventStoreDB and publishes them to an internal event bus and projection publisher.
/// </summary>
public class EventStoreDbSubscriptionToAll(
    IOptions<EventStoreDbOptions> eventStoreDbOptions,
    EventStoreClient eventStoreClient,
    IReadProjectionPublisher projectionPublisher,
    IInternalEventBus internalEventBus,
    ISubscriptionCheckpointRepository checkpointRepository,
    ILogger<EventStoreDbSubscriptionToAll> logger
) : BackgroundService
{
    private readonly EventStoreDbOptions _eventStoreDbOptions = eventStoreDbOptions.Value;
    private readonly Lock _resubscribeLock = new();
    private CancellationToken _cancellationToken;

    /// <summary>
    /// Starts the background service and subscribes to all events.
    /// </summary>
    /// <param name="stoppingToken">A token to cancel the operation.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;
        return SubscribeToAll(stoppingToken);
    }

    /// <summary>
    /// Subscribes to all events in EventStoreDB and handles incoming events.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    private async Task SubscribeToAll(CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        logger.LogInformation(
            "Subscription to all '{SubscriptionId}'",
            _eventStoreDbOptions.SubscriptionOptions.SubscriptionId
        );

        var checkpoint = await checkpointRepository.Load(
            _eventStoreDbOptions.SubscriptionOptions.SubscriptionId,
            cancellationToken
        );

        await eventStoreClient.SubscribeToAllAsync(
            checkpoint == null
                ? FromAll.Start
                : FromAll.After(new Position(checkpoint.Value, checkpoint.Value)),
            HandleEvent,
            _eventStoreDbOptions.SubscriptionOptions.ResolveLinkTos,
            HandleDrop,
            new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents()),
            null,
            cancellationToken
        );

        logger.LogInformation(
            "Subscription to all '{SubscriptionId}' started",
            _eventStoreDbOptions.SubscriptionOptions.SubscriptionId
        );
    }

    /// <summary>
    /// Handles incoming events by publishing them to the internal event bus and projection publisher, and storing the checkpoint.
    /// </summary>
    /// <param name="subscription">The stream subscription.</param>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The task representing the asynchronous operation.</returns>
    private async Task HandleEvent(
        StreamSubscription subscription,
        ResolvedEvent resolvedEvent,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (IsEventWithEmptyData(resolvedEvent) || IsCheckpointEvent(resolvedEvent))
                return;

            var streamEvent = resolvedEvent.ToStreamEvent();

            // Publish event to internal event bus and projection publisher
            await internalEventBus.Publish(streamEvent, cancellationToken);
            await projectionPublisher.PublishAsync(streamEvent, cancellationToken);

            await checkpointRepository.Store(
                _eventStoreDbOptions.SubscriptionOptions.SubscriptionId,
                resolvedEvent.Event.Position.CommitPosition,
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(
                "Error consuming message: {ExceptionMessage}{ExceptionStackTrace}",
                e.Message,
                e.StackTrace
            );

            throw;
        }
    }

    /// <summary>
    /// Handles the event when a subscription is dropped, and attempts to resubscribe.
    /// </summary>
    /// <param name="streamSubscription">The stream subscription.</param>
    /// <param name="reason">The reason the subscription was dropped.</param>
    /// <param name="exception">The exception that caused the subscription to be dropped, if any.</param>
    private void HandleDrop(
        StreamSubscription streamSubscription,
        SubscriptionDroppedReason reason,
        Exception? exception
    )
    {
        logger.LogError(
            exception,
            "Subscription to all '{SubscriptionId}' dropped with '{Reason}'",
            _eventStoreDbOptions.SubscriptionOptions.SubscriptionId,
            reason
        );

        if (exception is RpcException { StatusCode: StatusCode.Cancelled })
            return;

        Resubscribe();
    }

    /// <summary>
    /// Attempts to resubscribe to all events in EventStoreDB, retrying with a delay in case of failure.
    /// </summary>
    private void Resubscribe()
    {
        while (true)
        {
            var resubscribed = false;
            try
            {
                lock (_resubscribeLock)
                {
                    using (NoSynchronizationContextScope.Enter())
                    {
                        SubscribeToAll(_cancellationToken).Wait(_cancellationToken);
                    }

                    resubscribed = true;
                }
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Failed to resubscribe to all '{SubscriptionId}' dropped with '{ExceptionMessage}{ExceptionStackTrace}'",
                    _eventStoreDbOptions.SubscriptionOptions.SubscriptionId,
                    exception.Message,
                    exception.StackTrace
                );
            }

            if (resubscribed)
                break;

            // Sleep between reconnections to avoid flooding the database and reduce CPU usage
            Thread.Sleep(1000 + new Random((int)DateTime.UtcNow.Ticks).Next(1000));
        }
    }

    /// <summary>
    /// Checks if the resolved event has empty data.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>True if the event data is empty, otherwise false.</returns>
    private bool IsEventWithEmptyData(ResolvedEvent resolvedEvent)
    {
        if (resolvedEvent.Event.Data.Length != 0)
            return false;

        logger.LogInformation("Event without data received");
        return true;
    }

    /// <summary>
    /// Checks if the resolved event is a checkpoint event.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>True if the event is a checkpoint event, otherwise false.</returns>
    private bool IsCheckpointEvent(ResolvedEvent resolvedEvent)
    {
        if (resolvedEvent.Event.EventType != TypeMapper.GetTypeName<CheckpointStored>())
            return false;

        logger.LogInformation("Checkpoint event - ignoring");
        return true;
    }
}