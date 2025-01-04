using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Abstractions.Persistence.EFCore
{
    /// <summary>
    /// Defines a contract for a repository that supports paginated queries.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the identifier.</typeparam>
    public interface IPageRepository<TEntity, TKey>
        where TEntity : IHaveIdentity<TKey>
    {
    }

    /// <summary>
    /// Defines a contract for a repository that supports paginated queries with a Guid identifier.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IPageRepository<TEntity> : IPageRepository<TEntity, Guid>
        where TEntity : IHaveIdentity<Guid>
    {
    }
}