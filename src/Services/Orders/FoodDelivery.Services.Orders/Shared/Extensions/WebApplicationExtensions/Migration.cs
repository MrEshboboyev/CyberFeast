using BuildingBlocks.Abstractions.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FoodDelivery.Services.Orders.Shared.Extensions.WebApplicationExtensions;

public static partial class WebApplicationExtensions
{
    public static async Task MigrateDatabases(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationManager>();

        await migrationManager.ExecuteAsync(CancellationToken.None);
    }
}