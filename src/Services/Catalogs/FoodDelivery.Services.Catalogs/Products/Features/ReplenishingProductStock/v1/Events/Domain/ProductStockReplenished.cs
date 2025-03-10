using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Core.Events.Internal;
using BuildingBlocks.Core.Extensions;

namespace FoodDelivery.Services.Catalogs.Products.Features.ReplenishingProductStock.v1.Events.Domain;

// we don't pass value-objects and domains to our commands and events, just primitive types
internal record ProductStockReplenished(
    long ProductId,
    int AvailableStock,
    int RestockThreshold,
    int MaxStockThreshold,
    int ReplenishedQuantity
) : DomainEvent
{
    public static ProductStockReplenished Of(
        long productId,
        int availableStock,
        int restockThreshold,
        int maxStockThreshold,
        int replenishedQuantity
    )
    {
        productId.NotBeNegativeOrZero();
        availableStock.NotBeNegativeOrZero();
        restockThreshold.NotBeNegativeOrZero();
        maxStockThreshold.NotBeNegativeOrZero();
        replenishedQuantity.NotBeNegativeOrZero();

        return new ProductStockReplenished(
            productId,
            availableStock,
            restockThreshold,
            maxStockThreshold,
            replenishedQuantity
        );

        // // Also if validation rules are more complex we can use `fluent validation`
        // return new ProductStockReplenishedValidator().HandleValidation(
        //     new ProductStockReplenished(
        //         productId,
        //         availableStock,
        //         restockThreshold,
        //         maxStockThreshold,
        //         replenishedQuantity
        //     )
        // );
    }
}

internal class ProductStockReplenishedHandler : IDomainEventHandler<ProductStockReplenished>
{
    public Task Handle(ProductStockReplenished notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
