namespace BuildingBlocks.Core.Threading;

/// <summary>
/// Provides a scope that temporarily suppresses the current synchronization context.
/// </summary>
public static class NoSynchronizationContextScope
{
    /// <summary>
    /// Enters a scope where the synchronization context is suppressed.
    /// </summary>
    /// <returns>A disposable object that restores the original synchronization context upon disposal.</returns>
    public static Disposable Enter()
    {
        var context = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);
        return new Disposable(context);
    }

    /// <summary>
    /// A disposable object that restores the original synchronization context upon disposal.
    /// </summary>
    public readonly struct Disposable : IDisposable
    {
        private readonly SynchronizationContext? _synchronizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable"/> struct.
        /// </summary>
        /// <param name="synchronizationContext">The original synchronization context.</param>
        public Disposable(SynchronizationContext? synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Restores the original synchronization context.
        /// </summary>
        public void Dispose() => SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
    }
}