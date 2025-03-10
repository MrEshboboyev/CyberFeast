using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Core.Events.Internal;
using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Catalogs.Shared.Contracts;

namespace FoodDelivery.Services.Catalogs.Products.Features.DebitingProductStock.v1.Events.Domain;

// we don't pass value-objects and domains to our commands and events, just primitive types
internal record ProductRestockThresholdReached(
    long ProductId,
    int AvailableStock,
    int RestockThreshold,
    int MaxStockThreshold,
    int Quantity
) : DomainEvent
{
    public static ProductRestockThresholdReached Of(
        long productId,
        int availableStock,
        int restockThreshold,
        int maxStockThreshold,
        int quantity
    )
    {
        productId.NotBeNegativeOrZero();
        availableStock.NotBeNegativeOrZero();
        restockThreshold.NotBeNegativeOrZero();
        maxStockThreshold.NotBeNegativeOrZero();
        quantity.NotBeNegativeOrZero();

        return new ProductRestockThresholdReached(
            productId,
            availableStock,
            restockThreshold,
            maxStockThreshold,
            quantity
        );
    }
}

internal class ProductRestockThresholdReachedHandler(ICatalogDbContext catalogDbContext)
    : IDomainEventHandler<ProductRestockThresholdReached>
{
    private readonly ICatalogDbContext _catalogDbContext = catalogDbContext;

    public Task Handle(ProductRestockThresholdReached notification, CancellationToken cancellationToken)
    {
        notification.NotBeNull();

        // For example email get more products
        return Task.CompletedTask;
    }
}
