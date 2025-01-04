namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines a contract for data seeders.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Gets the order in which the data seeder should be executed.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Seeds all the necessary data asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SeedAllAsync();
}