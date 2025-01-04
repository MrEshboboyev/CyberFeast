using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Abstractions.Persistence.EFCore;

/// <summary>
/// Defines a contract for an Entity Framework Core unit of work.
/// </summary>
public interface IEFUnitOfWork : 
    IUnitOfWork,
    ITransactionAble,
    ITransactionDbContextExecution,
    IRetryDbContextExecution
{
}

/// <summary>
/// Defines the interface(s) for a generic unit of work with a specific database context.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public interface IEFUnitOfWork<out TContext> : IEFUnitOfWork
    where TContext : DbContext
{
    /// <summary>
    /// Gets the database context of type TContext.
    /// </summary>
    TContext DbContext { get; }
}