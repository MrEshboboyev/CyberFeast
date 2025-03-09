using BuildingBlocks.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace FoodDelivery.Services.Orders.Orders.ValueObjects;

public record CustomerInfo
{
    // EF
    private CustomerInfo() { }

    public string Name { get; private set; } = null!;
    public long CustomerId { get; private set; }

    public static CustomerInfo Of([NotNull] string? name, long customerId)
    {
        // validations should be placed here instead of constructor
        name.NotBeEmptyOrNull();
        customerId.NotBeNegativeOrZero();

        return new CustomerInfo { Name = name, CustomerId = customerId };
    }

    public void Deconstruct(out string name, out long customerId) => (name, customerId) = (Name, CustomerId);
}
