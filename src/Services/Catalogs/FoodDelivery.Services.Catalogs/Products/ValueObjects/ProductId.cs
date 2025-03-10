using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Extensions;

namespace FoodDelivery.Services.Catalogs.Products.ValueObjects;

public record ProductId : AggregateId
{
    // EF
    private ProductId(long value)
        : base(value) { }

    // validations should be placed here instead of constructor
    public static ProductId Of(long id) => new(id.NotBeNegativeOrZero());

    public static implicit operator long(ProductId id) => id.Value;
}