using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Persistence.EfCore.Postgres;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;

/// <summary>
/// Creates connections for the message persistence repository using the specified connection string.
/// </summary>
public class NpgsqlMessagePersistenceConnectionFactory(string connectionString)
    : NpgsqlConnectionFactory(connectionString), IMessagePersistenceConnectionFactory;