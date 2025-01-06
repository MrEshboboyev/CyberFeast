using System.Linq.Expressions;

namespace BuildingBlocks.Abstractions.Messaging.PersistMessage;

/// <summary>
/// Defines a repository for persisting messages.
/// </summary>
public interface IMessagePersistenceRepository
{
    /// <summary>
    /// Asynchronously adds a new message to the repository.
    /// </summary>
    /// <param name="storeMessage">The message to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(
        StoreMessage storeMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing message in the repository.
    /// </summary>
    /// <param name="storeMessage">The message to update.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(
        StoreMessage storeMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously changes the state of a message.
    /// </summary>
    /// <param name="messageId">The ID of the message.</param>
    /// <param name="status">The new status of the message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ChangeStateAsync(
        Guid messageId,
        MessageStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all messages from the repository.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains a read-only list of messages.</returns>
    Task<IReadOnlyList<StoreMessage>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves messages from the repository based on a filter.
    /// </summary>
    /// <param name="predicate">The filter to apply to the messages.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains a read-only list of messages.</returns>
    Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
        Expression<Func<StoreMessage, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a message by its ID.
    /// </summary>
    /// <param name="id">The ID of the message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the message.</returns>
    Task<StoreMessage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously removes a message from the repository.
    /// </summary>
    /// <param name="storeMessage">The message to remove.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and indicates whether the removal was successful.</returns>
    Task<bool> RemoveAsync(
        StoreMessage storeMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up messages from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CleanupMessages();
}