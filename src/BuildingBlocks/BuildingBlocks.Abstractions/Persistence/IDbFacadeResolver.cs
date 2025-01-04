using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines a contract for resolving the DatabaseFacade from an Entity Framework Core context.
/// </summary>
public interface IDbFacadeResolver
{
    /// <summary>
    /// Gets the DatabaseFacade instance.
    /// </summary>
    DatabaseFacade Database { get; }
}