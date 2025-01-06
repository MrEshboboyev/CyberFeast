using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Events.Internal;

namespace BuildingBlocks.Abstractions.Messaging.PersistMessage;

/// <summary>
/// Defines methods for managing the persistence of messages in a storage system.
/// </summary>
public interface IMessagePersistenceService
{
    /// <summary>
    /// Retrieves messages based on a filter.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains a read-only list of filtered messages.</returns>
    Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
        Expression<Func<StoreMessage, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a published message to the storage.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The event envelope containing the message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddPublishMessageAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Adds a received message to the storage.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The event envelope containing the message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddReceivedMessageAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Adds an internal command to the storage.
    /// </summary>
    /// <typeparam name="TInternalCommand">The type of the internal command.</typeparam>
    /// <param name="internalCommand">The internal command to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddInternalMessageAsync<TInternalCommand>(
        TInternalCommand internalCommand,
        CancellationToken cancellationToken = default)
        where TInternalCommand : IInternalCommand;

    /// <summary>
    /// Adds a domain notification event to the storage.
    /// </summary>
    /// <typeparam name="TDomainNotification">The type of the domain notification event.</typeparam>
    /// <param name="notification">The domain notification event to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddNotificationAsync<TDomainNotification>(
        TDomainNotification notification,
        CancellationToken cancellationToken = default)
        where TDomainNotification : IDomainNotificationEvent;

    /// <summary>
    /// Processes a message by its ID.
    /// </summary>
    /// <param name="messageId">The ID of the message to process.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes all messages.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessAllAsync(CancellationToken cancellationToken = default);
}