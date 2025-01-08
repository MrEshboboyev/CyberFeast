using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Events;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Core.Persistence.EFCore.Interceptors;

/// <summary>
/// Intercepts save changes operations to handle concurrency for entities.
/// </summary>
public class ConcurrencyInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts the saving changes operation to increment the version number for entities with domain events before saving.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The interception result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in eventData.Context.ChangeTracker.Entries<IHaveDomainEvents>())
        {
            var events = entry.Entity.GetUncommittedDomainEvents();
            if (!events.Any()) continue;

            if (entry.Entity is IHaveAggregateVersion av)
            {
                entry.CurrentValues[nameof(IHaveAggregateVersion.OriginalVersion)] =
                    av.OriginalVersion + 1;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}