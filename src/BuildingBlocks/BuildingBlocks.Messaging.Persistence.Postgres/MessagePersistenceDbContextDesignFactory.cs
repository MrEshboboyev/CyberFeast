using BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;
using BuildingBlocks.Persistence.EfCore.Postgres;

namespace BuildingBlocks.Messaging.Persistence.Postgres;

/// <summary>
/// Provides a design-time factory for the <see cref="MessagePersistenceDbContext"/> class.
/// </summary>
public class MessagePersistenceDbContextDesignFactory
    : DbContextDesignFactoryBase<MessagePersistenceDbContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePersistenceDbContextDesignFactory"/> class.
    /// </summary>
    public MessagePersistenceDbContextDesignFactory()
        : base("ConnectionStrings:PostgresMessaging")
    {
    }
}