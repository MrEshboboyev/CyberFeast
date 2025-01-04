using System.Data.Common;

namespace BuildingBlocks.Abstractions.Persistence.EFCore;

/// <summary>
/// Defines a contract for creating and managing database connections.
/// </summary>
public interface IConnectionFactory : IDisposable
{
    /// <summary>
    /// Asynchronously gets an existing database connection or creates a new one.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and contains the database connection.</returns>
    Task<DbConnection> GetOrCreateConnectionAsync();
}