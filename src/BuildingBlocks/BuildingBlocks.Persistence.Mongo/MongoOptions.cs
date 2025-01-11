namespace BuildingBlocks.Persistence.Mongo;

/// <summary>
/// Defines configuration options for MongoDB.
/// </summary>
public class MongoOptions
{
    /// <summary>
    /// Gets or sets the connection string for MongoDB.
    /// </summary>
    public string? ConnectionString { get; set; } = null;

    /// <summary>
    /// Gets or sets the database name for MongoDB.
    /// </summary>
    public string? DatabaseName { get; set; } = null;

    /// <summary>
    /// Gets a unique ID.
    /// </summary>
    public static Guid UniqueId { get; set; } = Guid.NewGuid();
}