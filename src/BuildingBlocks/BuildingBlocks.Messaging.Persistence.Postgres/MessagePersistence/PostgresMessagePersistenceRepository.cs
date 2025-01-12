using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;

/// <summary>
/// Implements the IMessagePersistenceRepository interface for managing persistent messages in PostgreSQL using EF Core.
/// </summary>
public class PostgresMessagePersistenceRepository : IMessagePersistenceRepository
{
    private readonly MessagePersistenceDbContext _persistenceDbContext;
    private readonly ILogger<PostgresMessagePersistenceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresMessagePersistenceRepository"/> class.
    /// </summary>
    /// <param name="persistenceDbContext">The message persistence database context.</param>
    /// <param name="logger">The logger for logging messages.</param>
    public PostgresMessagePersistenceRepository(
        MessagePersistenceDbContext persistenceDbContext,
        ILogger<PostgresMessagePersistenceRepository> logger)
    {
        _persistenceDbContext = persistenceDbContext;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new store message to the database asynchronously.
    /// </summary>
    /// <param name="storeMessage">The store message to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(StoreMessage storeMessage, CancellationToken cancellationToken = default)
    {
        await _persistenceDbContext.StoreMessages.AddAsync(storeMessage, cancellationToken);
        await _persistenceDbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing store message in the database asynchronously.
    /// </summary>
    /// <param name="storeMessage">The store message to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(StoreMessage storeMessage, CancellationToken cancellationToken = default)
    {
        _persistenceDbContext.StoreMessages.Update(storeMessage);
        await _persistenceDbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Changes the state of a store message in the database asynchronously.
    /// </summary>
    /// <param name="messageId">The ID of the message to change state.</param>
    /// <param name="status">The new status of the message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ChangeStateAsync(
        Guid messageId,
        MessageStatus status,
        CancellationToken cancellationToken = default)
    {
        var message = await _persistenceDbContext.StoreMessages.FirstOrDefaultAsync(
            x => x.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.ChangeState(status);
            await _persistenceDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves all store messages from the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of store messages.</returns>
    public async Task<IReadOnlyList<StoreMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return (await _persistenceDbContext.StoreMessages.AsNoTracking().ToListAsync(cancellationToken)).AsReadOnly();
    }

    /// <summary>
    /// Retrieves store messages by a filter predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only list of store messages that match the filter.</returns>
    public async Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
        Expression<Func<StoreMessage, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return (await _persistenceDbContext.StoreMessages
            .Where(predicate)
            .AsNoTracking()
            .ToListAsync(cancellationToken)).AsReadOnly();
    }

    /// <summary>
    /// Retrieves a store message by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the store message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The store message if found; otherwise, null.</returns>
    public async Task<StoreMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _persistenceDbContext.StoreMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <summary>
    /// Removes a store message from the database asynchronously.
    /// </summary>
    /// <param name="storeMessage">The store message to remove.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the message was removed successfully; otherwise, <c>false</c>.</returns>
    public async Task<bool> RemoveAsync(StoreMessage storeMessage, CancellationToken cancellationToken = default)
    {
        _persistenceDbContext.StoreMessages.Remove(storeMessage);
        await _persistenceDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Cleans up all store messages from the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CleanupMessages()
    {
        if (!await _persistenceDbContext.StoreMessages.AnyAsync())
            return;

        _persistenceDbContext.StoreMessages.RemoveRange(_persistenceDbContext.StoreMessages);
        await _persistenceDbContext.SaveChangesAsync();
    }
}