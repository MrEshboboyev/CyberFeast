using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Core.Events.Internal;
using BuildingBlocks.Core.Extensions;

namespace FoodDelivery.Services.Catalogs.Products.Features.DebitingProductStock.v1.Events.Domain;

// we don't pass value-objects and domains to our commands and events, just primitive types
public record ProductStockDebited(
    long ProductId,
    int AvailableStock,
    int RestockThreshold,
    int MaxStockThreshold,
    int DebitQuantity
) : DomainEvent
{
    public static ProductStockDebited Of(
        long productId,
        int availableStock,
        int restockThreshold,
        int maxStockThreshold,
        int debitQuantity
    )
    {
        productId.NotBeNegativeOrZero();
        availableStock.NotBeNegativeOrZero();
        restockThreshold.NotBeNegativeOrZero();
        maxStockThreshold.NotBeNegativeOrZero();
        debitQuantity.NotBeNegativeOrZero();

        return new ProductStockDebited(productId, availableStock, restockThreshold, maxStockThreshold, debitQuantity);

        // // Also if validation rules are more complex we can use `fluent validation`
        // return new ProductStockDebitedValidator().HandleValidation(
        //     new ProductStockDebited(
        //         productId,
        //         availableStock,
        //         restockThreshold,
        //         maxStockThreshold,
        //         debitQuantity
        //     )
        // );
    }
}

internal class ProductStockDebitedHandler : IDomainEventHandler<ProductStockDebited>
{
    public Task Handle(ProductStockDebited notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
