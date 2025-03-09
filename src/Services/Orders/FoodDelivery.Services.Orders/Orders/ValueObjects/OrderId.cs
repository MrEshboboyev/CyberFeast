using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Extensions;

namespace FoodDelivery.Services.Orders.Orders.ValueObjects;

public record OrderId : AggregateId
{
    // EF
    private OrderId(long value) : base(value) { }

    // validations should be placed here instead of constructor
    public static OrderId Of(long id) => new(id.NotBeNegativeOrZero());

    public static implicit operator long(OrderId id) => id.Value;
}
