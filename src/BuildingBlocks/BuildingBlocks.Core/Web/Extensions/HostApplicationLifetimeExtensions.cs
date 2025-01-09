using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Core.Web.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IHostApplicationLifetime"/> interface.
/// </summary>
public static class HostApplicationLifetimeExtensions
{
    /// <summary>
    /// Waits for the application to start up or the stopping token to be triggered.
    /// </summary>
    /// <param name="lifetime">The application lifetime.</param>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating whether the application started successfully.</returns>
    public static async Task<bool> WaitForAppStartup(
        this IHostApplicationLifetime lifetime,
        CancellationToken stoppingToken)
    {
        var startedSource = new TaskCompletionSource();
        var cancelledSource = new TaskCompletionSource();

        await using var reg1 = lifetime
            .ApplicationStarted
            .Register(() => startedSource.SetResult());
        await using var reg2 = stoppingToken
            .Register(() => cancelledSource.SetResult());

        var completedTask = await Task
            .WhenAny(startedSource.Task, cancelledSource.Task)
            .ConfigureAwait(false);

        // If the completed task was the "app started" task, return true; otherwise, false
        return completedTask == startedSource.Task;
    }
}