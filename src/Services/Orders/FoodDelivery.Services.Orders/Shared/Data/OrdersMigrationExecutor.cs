using BuildingBlocks.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodDelivery.Services.Orders.Shared.Data;

public class OrdersMigrationExecutor(
    OrdersDbContext ordersDbContext, 
    ILogger<OrdersMigrationExecutor> logger) : IMigrationExecutor
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Migration worker started");

        logger.LogInformation("Updating identity database...");

        await ordersDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

        logger.LogInformation("identity database Updated");
    }
}