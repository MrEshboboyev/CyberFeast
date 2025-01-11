using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.Mongo;
using Humanizer;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo;

/// <summary>
/// Provides a context for interacting with MongoDB.
/// </summary>
public class MongoDbContext : IMongoDbContext, ITransactionDbContextExecution
{
    /// <summary>
    /// Gets or sets the MongoDB client session.
    /// </summary>
    public IClientSessionHandle? Session { get; set; }

    /// <summary>
    /// Gets the MongoDB database.
    /// </summary>
    public IMongoDatabase Database { get; }

    /// <summary>
    /// Gets the MongoDB client.
    /// </summary>
    public IMongoClient MongoClient { get; }

    protected readonly IList<Func<Task>> Commands;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContext"/> class.
    /// </summary>
    /// <param name="options">The MongoDB options.</param>
    public MongoDbContext(MongoOptions options)
    {
        MongoClient = new MongoClient(options.ConnectionString);
        var databaseName = options.DatabaseName;
        Database = MongoClient.GetDatabase(databaseName);

        // Every command will be stored, and it'll be processed at SaveChanges
        Commands = new List<Func<Task>>();
    }

    /// <summary>
    /// Gets a MongoDB collection for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="name">The name of the collection.</param>
    /// <returns>The MongoDB collection.</returns>
    public IMongoCollection<T> GetCollection<T>(string? name = null)
    {
        return Database.GetCollection<T>(name ?? typeof(T).Name.Pluralize().Underscore());
    }

    /// <summary>
    /// Disposes of the context instance.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Saves the changes to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of commands processed.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = Commands.Count;

        // Standalone servers do not support transactions.
        using (Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken))
        {
            try
            {
                Session.StartTransaction();

                await SaveAction();

                await Session.CommitTransactionAsync(cancellationToken);
            }
            catch (NotSupportedException)
            {
                await SaveAction();
            }
            catch (Exception ex)
            {
                await Session.AbortTransactionAsync(cancellationToken);
                Commands.Clear();
                throw;
            }
        }

        Commands.Clear();

        return result;
    }

    private async Task SaveAction()
    {
        var commandTasks = Commands.Select(c => c());

        await Task.WhenAll(commandTasks);
    }

    /// <summary>
    /// Begins a transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        Session.StartTransaction();
    }

    /// <summary>
    /// Commits a transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Session is { IsInTransaction: true })
            await Session.CommitTransactionAsync(cancellationToken);

        Session?.Dispose();
    }

    /// <summary>
    /// Rolls back a transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Session?.AbortTransactionAsync(cancellationToken)!;
    }

    /// <summary>
    /// Adds a command to be executed at SaveChanges.
    /// </summary>
    /// <param name="func">The command to add.</param>
    public void AddCommand(Func<Task> func)
    {
        Commands.Add(func);
    }

    /// <summary>
    /// Executes an action within a transaction asynchronously.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await action();

            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Executes a function within a transaction asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="action">The function to execute.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result of the function.</returns>
    public async Task<T> ExecuteTransactionalAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();

            await CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}