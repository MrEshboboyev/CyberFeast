using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Core.Events.Internal;
using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Customers.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Customers.RestockSubscriptions.Features.ProcessingRestockNotification.v1.Events.Domain;

// we don't pass value-objects and domains to our commands and events, just primitive types
internal record RestockNotificationProcessed(long Id, DateTime ProcessedTime) : DomainEvent;

internal class RestockNotificationProcessedHandler(ICommandBus commandBus, CustomersDbContext customersDbContext)
    : IDomainEventHandler<RestockNotificationProcessed>
{
    public async Task Handle(RestockNotificationProcessed notification, CancellationToken cancellationToken)
    {
        notification.NotBeNull();

        var restockSubscription = await customersDbContext.RestockSubscriptions
            .Include(restockSubscription => restockSubscription.ProductInformation)
            .FirstOrDefaultAsync(x => x.Id == notification.Id, cancellationToken);

        if (restockSubscription is null)
            return;

        await commandBus.SendAsync(
            new UpdateMongoRestockSubscriptionReadModel(
                notification.Id,
                restockSubscription.CustomerId,
                restockSubscription.Email,
                restockSubscription.ProductInformation.Id,
                restockSubscription.ProductInformation.Name,
                restockSubscription.Processed,
                notification.ProcessedTime
            ),
            cancellationToken
        );
    }
}
