using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Catalogs.Products.Events.V1.Integration;

/// <summary>
/// Represents the event when the stock of a product is debited.
/// </summary>
public record ProductStockDebitedV1(
    long ProductId,
    int NewStock,
    int DebitedQuantity) : IntegrationEvent
{
    /// <summary>
    /// Creates a new instance of the <see cref="ProductStockDebitedV1"/> event with in-line validation.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="newStock">The new stock quantity.</param>
    /// <param name="debitedQuantity">The debited quantity.</param>
    /// <returns>A new instance of the <see cref="ProductStockDebitedV1"/> event.</returns>
    public static ProductStockDebitedV1 Of(
        long productId,
        int newStock,
        int debitedQuantity)
    {
        productId.NotBeNegativeOrZero();
        newStock.NotBeNegativeOrZero();
        debitedQuantity.NotBeNegativeOrZero();

        return new ProductStockDebitedV1(productId, newStock, debitedQuantity);
    }
}