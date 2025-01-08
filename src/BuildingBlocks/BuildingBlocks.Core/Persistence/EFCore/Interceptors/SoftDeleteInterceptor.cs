using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Core.Persistence.EFCore.Interceptors;

/// <summary>
/// Intercepts save changes operations to handle soft deletion for entities.
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts the saving changes operation to set the IsDeleted property for entities being soft deleted or added before saving.
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

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is IHaveSoftDelete)
                        entry.CurrentValues["IsDeleted"] = false;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is IHaveSoftDelete)
                    {
                        entry.State = EntityState.Modified;
                        eventData.Context.Entry(entry.Entity).CurrentValues["IsDeleted"] = true;
                    }

                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}