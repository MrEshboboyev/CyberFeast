namespace BuildingBlocks.Persistence.EfCore.Postgres;

/// <summary>
/// Defines configuration options for PostgreSQL.
/// </summary>
public class PostgresOptions
{
    /// <summary>
    /// Gets or sets the connection string for PostgreSQL.
    /// </summary>
    public string? ConnectionString { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether to use in-memory database.
    /// </summary>
    public bool UseInMemory { get; set; }

    /// <summary>
    /// Gets or sets the migration assembly for EF Core migrations.
    /// </summary>
    public string? MigrationAssembly { get; set; } = null;
}