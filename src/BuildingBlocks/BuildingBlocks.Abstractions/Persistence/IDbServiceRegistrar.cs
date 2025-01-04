using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines a contract for registering database services.
/// </summary>
public interface IDbServiceRegistrar
{
    /// <summary>
    /// Registers database services into the provided IServiceCollection.
    /// </summary>
    /// <param name="services">The service collection to register the services into.</param>
    void Register(IServiceCollection services);
}