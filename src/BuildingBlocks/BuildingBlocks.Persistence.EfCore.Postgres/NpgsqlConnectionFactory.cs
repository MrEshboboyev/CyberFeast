using System.Data;
using System.Data.Common;
using BuildingBlocks.Abstractions.Persistence.EFCore;
using BuildingBlocks.Core.Extensions;
using Npgsql;

namespace BuildingBlocks.Persistence.EfCore.Postgres;

/// <summary>
/// Provides an implementation of <see cref="IConnectionFactory"/> for managing Npgsql connections.
/// </summary>
public class NpgsqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;
    private DbConnection? _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlConnectionFactory"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string for Npgsql.</param>
    public NpgsqlConnectionFactory(string? connectionString)
    {
        connectionString.NotBeNullOrWhiteSpace();
        _connectionString = connectionString;
    }

    /// <summary>
    /// Gets or creates a database connection asynchronously.
    /// </summary>
    /// <returns>The database connection.</returns>
    public async Task<DbConnection> GetOrCreateConnectionAsync()
    {
        if (_connection is not null && _connection.State == ConnectionState.Open)
            return _connection;

        _connection = new NpgsqlConnection(_connectionString);
        await _connection.OpenAsync();

        return _connection;
    }

    /// <summary>
    /// Disposes the database connection.
    /// </summary>
    public void Dispose()
    {
        if (_connection is { State: ConnectionState.Open })
            _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}