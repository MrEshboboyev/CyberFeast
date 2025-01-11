using BuildingBlocks.Abstractions.Domain;
using Sieve.Services;

namespace BuildingBlocks.Persistence.Mongo;

/// <summary>
/// A repository class for MongoDB with generic entities and ID types, inheriting from the base repository class.
/// </summary>
/// <typeparam name="TDbContext">The type of the MongoDB context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity ID.</typeparam>
public class MongoRepository<TDbContext, TEntity, TKey>(TDbContext dbContext, ISieveProcessor sieveProcessor)
    : MongoRepositoryBase<TDbContext, TEntity, TKey>(dbContext, sieveProcessor)
    where TEntity : class, IHaveIdentity<TKey>
    where TDbContext : MongoDbContext;

/// <summary>
/// A repository class for MongoDB with generic entities and GUID IDs, inheriting from the base repository class.
/// </summary>
/// <typeparam name="TDbContext">The type of the MongoDB context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class MongoRepository<TDbContext, TEntity>(TDbContext dbContext, ISieveProcessor sieveProcessor)
    : MongoRepository<TDbContext, TEntity, Guid>(dbContext, sieveProcessor)
    where TEntity : class, IHaveIdentity<Guid>
    where TDbContext : MongoDbContext;