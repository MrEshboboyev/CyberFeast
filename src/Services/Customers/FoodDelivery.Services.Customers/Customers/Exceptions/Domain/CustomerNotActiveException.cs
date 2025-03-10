using BuildingBlocks.Core.Exception.Types;

namespace FoodDelivery.Services.Customers.Customers.Exceptions.Domain;

internal class CustomerNotActiveException(long customerId)
    : AppException($"Customer with ID: '{customerId}' is not active.")
{
    public long CustomerId { get; } = customerId;
}
