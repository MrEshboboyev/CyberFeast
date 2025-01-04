namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines a contract for executing database migrations.
/// </summary>
public interface IMigrationExecutor
{
    /// <summary>
    /// Executes the migration logic asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteAsync(CancellationToken cancellationToken);
}