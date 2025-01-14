using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Catalogs.Products.Events.V1.Integration;

/// <summary>
/// Represents the event when the stock of a product is replenished.
/// </summary>
public record ProductStockReplenishedV1(
    long ProductId,
    int NewStock,
    int ReplenishedQuantity) : IntegrationEvent
{
    /// <summary>
    /// Creates a new instance of the <see cref="ProductStockReplenishedV1"/> event with in-line validation.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="newStock">The new stock quantity.</param>
    /// <param name="replenishedQuantity">The replenished quantity.</param>
    /// <returns>A new instance of the <see cref="ProductStockReplenishedV1"/> event.</returns>
    public static ProductStockReplenishedV1 Of(
        long productId,
        int newStock,
        int replenishedQuantity)
    {
        productId.NotBeNegativeOrZero();
        newStock.NotBeNegativeOrZero();
        replenishedQuantity.NotBeNegativeOrZero();

        return new ProductStockReplenishedV1(productId, newStock, replenishedQuantity);
    }
}