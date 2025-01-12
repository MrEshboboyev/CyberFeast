using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BuildingBlocks.Validation.Extensions;

/// <summary>
/// Provides extension methods for registering custom validators and validation behaviors.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers custom validators from the specified assembly in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the validators to.</param>
    /// <param name="assembly">The assembly containing the validators.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCustomValidators(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithLifetime(ServiceLifetime.Transient));

        return services;
    }
}