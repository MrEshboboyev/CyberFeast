using BuildingBlocks.Core.Exception.Types;

namespace FoodDelivery.Services.Customers.Customers.Exceptions.Domain;

internal class CustomerAlreadyCompletedException(long customerId)
    : AppException($"Customer with ID: '{customerId}' already completed.")
{
    public long CustomerId { get; } = customerId;
}
