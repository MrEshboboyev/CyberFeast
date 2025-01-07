using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace BuildingBlocks.Core.Persistence.EfCore;

/// <summary>
/// Represents a repository for managing entities using Entity Framework Core.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public class EfRepository<TDbContext, TEntity, TKey>(
    TDbContext dbContext,
    ISieveProcessor sieveProcessor)
    : EfRepositoryBase<TDbContext, TEntity, TKey>(dbContext, sieveProcessor)
    where TEntity : class, IHaveIdentity<TKey>
    where TDbContext : DbContext
{
}

/// <summary>
/// Represents a repository for managing entities with a GUID key using Entity Framework Core.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class EfRepository<TDbContext, TEntity>(
    TDbContext dbContext,
    ISieveProcessor sieveProcessor)
    : EfRepository<TDbContext, TEntity, Guid>(dbContext, sieveProcessor)
    where TEntity : class, IHaveIdentity<Guid>
    where TDbContext : DbContext
{
}