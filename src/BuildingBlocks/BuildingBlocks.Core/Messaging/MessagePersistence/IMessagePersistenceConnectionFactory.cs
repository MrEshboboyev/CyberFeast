using BuildingBlocks.Abstractions.Persistence.EFCore;

namespace BuildingBlocks.Core.Messaging.MessagePersistence;

/// <summary>
/// Represents a connection factory for message persistence, extending the <see cref="IConnectionFactory"/> interface.
/// </summary>
public interface IMessagePersistenceConnectionFactory : IConnectionFactory
{
}