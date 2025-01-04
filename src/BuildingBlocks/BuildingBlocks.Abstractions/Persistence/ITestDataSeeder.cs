namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines a contract for test data seeders.
/// </summary>
public interface ITestDataSeeder
{
    /// <summary>
    /// Gets the order in which the test data seeder should be executed.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Seeds all the test data asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SeedAllAsync();
}