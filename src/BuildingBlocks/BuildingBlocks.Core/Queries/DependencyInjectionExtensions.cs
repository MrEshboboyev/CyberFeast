using BuildingBlocks.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Queries;

/// <summary>
/// Provides extension methods for setting up dependency injection for queries.
/// </summary>
internal static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the <see cref="QueryBus"/> service as <see cref="IQueryBus"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    internal static IServiceCollection AddQueryBus(this IServiceCollection services)
    {
        services.AddTransient<IQueryBus, QueryBus>();

        return services;
    }
}