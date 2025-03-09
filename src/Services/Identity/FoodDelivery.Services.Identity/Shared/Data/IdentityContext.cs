using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence;
using FoodDelivery.Services.Identity.Shared.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;

namespace FoodDelivery.Services.Identity.Shared.Data;

/// <summary>
/// Custom DbContext for Identity service, integrating EF Core and ASP.NET Identity.
/// Includes naming conventions and transactional capabilities.
/// </summary>
public class IdentityContext(DbContextOptions<IdentityContext> options)
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >(options),
        IDbFacadeResolver,
        IDomainEventContext,
        ITransactionDbContextExecution
{
    /// <summary>
    /// Configures the model, applying naming conventions and configurations.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations from the assembly
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Replace table names
            // Convert table names to snake_case
            entity.SetTableName(entity.GetTableName()?.Underscore());

            var objectIdentifier = StoreObjectIdentifier.Table(
                entity.GetTableName()?.Underscore()!,
                entity.GetSchema()
            );

            // Replace column names
            // Convert column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName(objectIdentifier)?.Underscore());
            }

            // Convert key and foreign key names to snake_case
            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.Underscore());
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName()?.Underscore());
            }
        }
    }

    /// <summary>
    /// Executes a transactional operation asynchronously.
    /// </summary>
    public Task ExecuteTransactionalAsync(
        Func<Task> action, 
        CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken
            );

            try
            {
                await action();

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    /// <summary>
    /// Executes a transactional operation asynchronously, returning a result.
    /// </summary>
    public Task<T> ExecuteTransactionalAsync<T>(
        Func<Task<T>> action, 
        CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();

        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken
            );

            try
            {
                var result = await action();

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
    {
        return new List<IDomainEvent>();
    }

    public void MarkUncommittedDomainEventAsCommitted()
    {
        // Method intentionally left empty.
    }
}
