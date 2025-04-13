using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BuildingBlocks.Abstractions.Core.Diagnostics;

/// <summary>
/// Provides a contract for working with diagnostics instrumentation such as <see cref="ActivitySource"/> and <see cref="Meter"/>.
/// Allows execution of logic within a diagnostic <see cref="Activity"/> context, enabling observability features like tracing and metrics.
/// </summary>
public interface IDiagnosticsProvider : IDisposable
{
    /// <summary>
    /// Gets the name used for instrumentation (e.g., service name, component name) for identifying telemetry sources.
    /// </summary>
    string InstrumentationName { get; }

    /// <summary>
    /// Gets the <see cref="ActivitySource"/> used to start and manage <see cref="Activity"/> instances for tracing operations.
    /// </summary>
    ActivitySource ActivitySource { get; }

    /// <summary>
    /// Gets the <see cref="Meter"/> instance used for collecting metrics such as counters, histograms, etc.
    /// </summary>
    Meter Meter { get; }

    /// <summary>
    /// Executes an asynchronous action within a created <see cref="Activity"/> context using the specified <paramref name="createActivityInfo"/>.
    /// This allows the action to be traced with telemetry data.
    /// </summary>
    /// <param name="createActivityInfo">Metadata used to configure the new activity.</param>
    /// <param name="action">The asynchronous action to be executed within the activity context.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteActivityAsync(
        CreateActivityInfo createActivityInfo,
        Func<Activity?, CancellationToken, Task> action,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes an asynchronous function that returns a result within a created <see cref="Activity"/> context.
    /// The activity is configured using the provided <paramref name="createActivityInfo"/> metadata.
    /// </summary>
    /// <typeparam name="TResult">The return type of the action.</typeparam>
    /// <param name="createActivityInfo">Metadata used to configure the new activity.</param>
    /// <param name="action">The asynchronous function to be executed within the activity context.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>A task representing the asynchronous operation, returning a result.</returns>
    Task<TResult?> ExecuteActivityAsync<TResult>(
        CreateActivityInfo createActivityInfo,
        Func<Activity?, CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default
    );
}
