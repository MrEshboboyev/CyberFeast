using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Core.Persistence.EFCore.Interceptors;

/// <summary>
/// Intercepts save changes operations to handle auditing for entities.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts the saving changes operation to set audit properties before saving.
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

        var now = DateTime.Now;

        // var userId = GetCurrentUser(); // TODO: Get current user
        foreach (var entry in eventData.Context.ChangeTracker.Entries<IHaveAudit>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues[nameof(IHaveAudit.LastModified)] = now;
                    entry.CurrentValues[nameof(IHaveAudit.LastModifiedBy)] = 1;
                    break;
                case EntityState.Added:
                    entry.CurrentValues[nameof(IHaveAudit.Created)] = now;
                    entry.CurrentValues[nameof(IHaveAudit.CreatedBy)] = 1;
                    break;
            }
        }

        foreach (var entry in eventData.Context.ChangeTracker.Entries<IHaveCreator>())
        {
            if (entry.State != EntityState.Added) continue;

            entry.CurrentValues[nameof(IHaveCreator.Created)] = now;
            entry.CurrentValues[nameof(IHaveCreator.CreatedBy)] = 1;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}