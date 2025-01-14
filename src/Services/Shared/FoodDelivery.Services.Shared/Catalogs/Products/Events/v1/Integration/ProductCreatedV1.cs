using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Catalogs.Products.Events.V1.Integration;

/// <summary>
/// Represents the event when a product is created.
/// </summary>
public record ProductCreatedV1(
    long Id,
    string Name,
    long CategoryId,
    string CategoryName,
    int Stock) : IntegrationEvent
{
    /// <summary>
    /// Creates a new instance of the <see cref="ProductCreatedV1"/> event with in-line validation.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="name">The product name.</param>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="categoryName">The category name.</param>
    /// <param name="stock">The stock quantity.</param>
    /// <returns>A new instance of the <see cref="ProductCreatedV1"/> event.</returns>
    public static ProductCreatedV1 Of(
        long id,
        string? name,
        long categoryId,
        string? categoryName,
        int stock)
    {
        id.NotBeNegativeOrZero();
        name.NotBeNullOrWhiteSpace();
        categoryId.NotBeNegativeOrZero();
        categoryName.NotBeNullOrWhiteSpace();
        stock.NotBeNegativeOrZero();

        return new ProductCreatedV1(id, name, categoryId, categoryName, stock);
    }
}