namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines a contract for managing and executing database migrations.
/// </summary>
public interface IMigrationManager
{
    /// <summary>
    /// Manages and executes the migration logic asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteAsync(CancellationToken cancellationToken);
}