using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Messaging.MessagePersistence.InMemory;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IMessagePersistenceRepository"/> interface for storing and managing messages.
/// </summary>
public class InMemoryMessagePersistenceRepository : IMessagePersistenceRepository
{
    private static readonly ConcurrentDictionary<Guid, StoreMessage> _messages = new();

    /// <summary>
    /// Adds a message to the in-memory store asynchronously.
    /// </summary>
    /// <param name="storeMessage">The message to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task AddAsync(
        StoreMessage storeMessage,
        CancellationToken cancellationToken = default)
    {
        storeMessage.NotBeNull();

        _messages.TryAdd(storeMessage.Id, storeMessage);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing message in the in-memory store asynchronously.
    /// </summary>
    /// <param name="storeMessage">The message to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateAsync(
        StoreMessage storeMessage,
        CancellationToken cancellationToken = default)
    {
        if (_messages.ContainsKey(storeMessage.Id))
        {
            _messages[storeMessage.Id] = storeMessage;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Changes the state of a message in the in-memory store asynchronously.
    /// </summary>
    /// <param name="messageId">The ID of the message to change state.</param>
    /// <param name="status">The new status of the message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ChangeStateAsync(
        Guid messageId,
        MessageStatus status,
        CancellationToken cancellationToken = default)
    {
        _messages.TryGetValue(messageId, out var message);

        message?.ChangeState(status);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves all messages from the in-memory store asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a list of all messages as the result.</returns>
    public Task<IReadOnlyList<StoreMessage>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = _messages
            .Select(x => x.Value)
            .ToImmutableList();

        return Task.FromResult<IReadOnlyList<StoreMessage>>(result);
    }

    /// <summary>
    /// Retrieves messages from the in-memory store that match a specified filter asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a list of filtered messages as the result.</returns>
    public Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
        Expression<Func<StoreMessage, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        predicate.NotBeNull();

        var result = _messages
            .Select(x => x.Value)
            .Where(predicate.Compile())
            .ToImmutableList();

        return Task.FromResult<IReadOnlyList<StoreMessage>>(result);
    }

    /// <summary>
    /// Retrieves a message by its ID from the in-memory store asynchronously.
    /// </summary>
    /// <param name="id">The ID of the message to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the retrieved message as the result.</returns>
    public Task<StoreMessage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = _messages
            .FirstOrDefault(x => x.Key == id)
            .Value;

        return Task.FromResult(result);
    }

    /// <summary>
    /// Removes a message from the in-memory store asynchronously.
    /// </summary>
    /// <param name="storeMessage">The message to remove.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating whether the removal was successful.</returns>
    public Task<bool> RemoveAsync(
        StoreMessage storeMessage,
        CancellationToken cancellationToken = default)
    {
        storeMessage.NotBeNull();

        var result = _messages.Remove(storeMessage.Id, out _);

        return Task.FromResult(result);
    }

    /// <summary>
    /// Cleans up all messages in the in-memory store.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CleanupMessages()
    {
        foreach (var storeMessage in _messages)
        {
            _messages.TryRemove(storeMessage);
        }

        return Task.CompletedTask;
    }
}