using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Core.Web.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IHostEnvironment"/> interface.
/// </summary>
public static class HostEnvironmentExtensions
{
    /// <summary>
    /// Checks if the environment is "Test".
    /// </summary>
    /// <param name="env">The host environment.</param>
    /// <returns><c>true</c> if the environment is "Test"; otherwise, <c>false</c>.</returns>
    public static bool IsTest(this IHostEnvironment env) => env.IsEnvironment(Environments.Test);

    /// <summary>
    /// Checks if the environment is "DependencyTest".
    /// </summary>
    /// <param name="env">The host environment.</param>
    /// <returns><c>true</c> if the environment is "DependencyTest"; otherwise, <c>false</c>.</returns>
    public static bool IsDependencyTest(this IHostEnvironment env) => env.IsEnvironment(Environments.DependencyTest);

    /// <summary>
    /// Checks if the environment is "Docker".
    /// </summary>
    /// <param name="env">The host environment.</param>
    /// <returns><c>true</c> if the environment is "Docker"; otherwise, <c>false</c>.</returns>
    public static bool IsDocker(this IHostEnvironment env) => env.IsEnvironment(Environments.Docker);
}