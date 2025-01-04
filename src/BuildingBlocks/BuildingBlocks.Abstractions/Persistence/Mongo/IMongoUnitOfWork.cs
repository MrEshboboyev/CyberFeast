namespace BuildingBlocks.Abstractions.Persistence.Mongo;

/// <summary>
/// Defines a contract for a MongoDB unit of work.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public interface IMongoUnitOfWork<out TContext> : IUnitOfWork<TContext>
    where TContext : class, IMongoDbContext
{
}