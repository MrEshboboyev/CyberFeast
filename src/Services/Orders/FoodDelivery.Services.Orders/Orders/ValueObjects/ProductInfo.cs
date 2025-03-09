using BuildingBlocks.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace FoodDelivery.Services.Orders.Orders.ValueObjects;

public record ProductInfo
{
    // EF
    private ProductInfo() { }

    public string Name { get; private set; } = null!;
    public long ProductId { get; private set; }
    public decimal Price { get; private set; }

    public static ProductInfo Of([NotNull] string? name, long productId, decimal price)
    {
        return new ProductInfo
        {
            Name = name.NotBeEmptyOrNull(),
            ProductId = productId.NotBeNegativeOrZero(),
            Price = price.NotBeNegativeOrZero()
        };
    }

    public void Deconstruct(out string name, out long productId, out decimal price) =>
        (name, productId, price) = (Name, ProductId, Price);
}
