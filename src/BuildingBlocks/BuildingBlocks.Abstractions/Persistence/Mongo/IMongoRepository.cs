using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Abstractions.Persistence.Mongo;

/// <summary>
/// Defines a contract for a MongoDB repository.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IHaveIdentity<TId>
{
}