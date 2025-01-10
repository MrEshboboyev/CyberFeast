using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Events;

namespace BuildingBlocks.Core.Commands;

/// <summary>
/// Implements the IAsyncCommandBus interface to facilitate sending asynchronous commands.
/// </summary>
public class AsyncCommandBus(
    IMessagePersistenceService messagePersistenceService,
    IMessageMetadataAccessor messageMetadataAccessor
) : IAsyncCommandBus
{
    /// <summary>
    /// Sends an asynchronous command and wraps it in an EventEnvelope for persistence.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to send.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendExternalAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IAsyncCommand
    {
        var correlationId = messageMetadataAccessor.GetCorrelationId();
        var causationId = messageMetadataAccessor.GetCautionId();
        var eventEnvelope = EventEnvelope.From(command, correlationId, causationId);

        return messagePersistenceService.AddPublishMessageAsync(eventEnvelope, cancellationToken);
    }
}